using JobBoard.Application.Jobs;
using JobBoard.Application.Jobs.ApplyToJob;
using JobBoard.Application.Jobs.CloseJob;
using JobBoard.Application.Jobs.GetJobApplicants;
using JobBoard.Application.Jobs.UpdateApplicationStatus;
using JobBoard.Application.Jobs.UpdateJob;
using JobBoard.Application.Jobs.WithdrawApplication;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;
using JobBoard.Tests.Fakes;

namespace JobBoard.Tests
{
    // The security rules from docs/03-requirements.md: every mutating endpoint checks ownership, and
    // "someone else's resource" looks exactly like "no such resource" (404), never like success.
    public class OwnershipTests
    {
        private const string EmployerA = "employer-a";
        private const string EmployerB = "employer-b";
        private const string SeekerA = "seeker-a";
        private const string SeekerB = "seeker-b";

        private readonly FakeJobRepository jobs = new();
        private readonly FakeJobApplicationRepository applications = new();
        private readonly FakeSkillRepository skills = new();

        private async Task<Job> PostJobAsEmployerA()
        {
            return await jobs.AddJob(new Job
            {
                Title = "Junior .NET Developer",
                Description = "Build APIs.",
                JobType = JobType.FullTime,
                JobLocation = Location.OnSite,
                EmployerId = EmployerA,
                PostedAt = DateTimeOffset.UtcNow
            });
        }

        private static UpdateJobCommand ValidUpdate() => new()
        {
            Title = "Edited title",
            Description = "Edited description",
            JobType = JobType.Contract,
            JobLocation = Location.Remote,
            ExperienceLevel = ExperienceLevel.Mid
        };

        [Fact]
        public async Task Employer_B_cannot_edit_employer_A_job()
        {
            var job = await PostJobAsEmployerA();

            var result = await new UpdateJobHandler(jobs, skills).Handle(job.Id, ValidUpdate(), EmployerB);

            Assert.Equal(UpdateJobStatus.JobNotFound, result.Status);
            Assert.Equal("Junior .NET Developer", jobs.Jobs.Single().Title);
        }

        [Fact]
        public async Task Owner_can_edit_own_open_job()
        {
            var job = await PostJobAsEmployerA();

            var result = await new UpdateJobHandler(jobs, skills).Handle(job.Id, ValidUpdate(), EmployerA);

            Assert.Equal(UpdateJobStatus.Updated, result.Status);
            Assert.Equal("Edited title", jobs.Jobs.Single().Title);
        }

        [Fact]
        public async Task Closed_job_cannot_be_edited_even_by_its_owner()
        {
            var job = await PostJobAsEmployerA();
            await new CloseJobHandler(jobs).Handle(job.Id, EmployerA);

            var result = await new UpdateJobHandler(jobs, skills).Handle(job.Id, ValidUpdate(), EmployerA);

            Assert.Equal(UpdateJobStatus.JobClosed, result.Status);
        }

        [Fact]
        public async Task Employer_B_cannot_close_employer_A_job()
        {
            var job = await PostJobAsEmployerA();

            var closed = await new CloseJobHandler(jobs).Handle(job.Id, EmployerB);

            Assert.False(closed);
            Assert.Null(jobs.Jobs.Single().ClosedAt);
        }

        [Fact]
        public async Task Employer_B_cannot_see_applicants_of_employer_A_job()
        {
            var job = await PostJobAsEmployerA();
            await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA);

            var applicants = await new GetJobApplicantsHandler(jobs, applications).Handle(job.Id, EmployerB);

            Assert.Null(applicants);
        }

        [Fact]
        public async Task Applying_twice_to_the_same_job_is_rejected()
        {
            var job = await PostJobAsEmployerA();
            var handler = new ApplyToJobHandler(jobs, applications);

            var first = await handler.Handle(job.Id, SeekerA);
            var second = await handler.Handle(job.Id, SeekerA);

            Assert.Equal(ApplyToJobStatus.Created, first.Status);
            Assert.Equal(ApplyToJobStatus.AlreadyApplied, second.Status);
            Assert.Single(applications.Applications);
        }

        [Fact]
        public async Task Cannot_apply_to_a_closed_job()
        {
            var job = await PostJobAsEmployerA();
            await new CloseJobHandler(jobs).Handle(job.Id, EmployerA);

            var result = await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA);

            Assert.Equal(ApplyToJobStatus.JobNotFound, result.Status);
        }

        [Fact]
        public async Task Seeker_B_cannot_withdraw_seeker_A_application()
        {
            var job = await PostJobAsEmployerA();
            var applied = await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA);

            var result = await new WithdrawApplicationHandler(applications).Handle(applied.Application!.Id, SeekerB);

            Assert.Equal(ChangeApplicationStatusOutcome.NotFound, result.Outcome);
            Assert.Equal(ApplicationStatus.Applied, applications.Applications.Single().Status);
        }

        [Fact]
        public async Task Employer_B_cannot_change_status_on_employer_A_job()
        {
            var job = await PostJobAsEmployerA();
            var applied = await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA);
            var command = new UpdateApplicationStatusCommand { Status = ApplicationStatus.UnderReview };

            var result = await new UpdateApplicationStatusHandler(applications, jobs).Handle(applied.Application!.Id, command, EmployerB);

            Assert.Equal(ChangeApplicationStatusOutcome.NotFound, result.Outcome);
            Assert.Empty(applications.History);
        }

        [Fact]
        public async Task Owner_status_change_is_applied_and_recorded_in_history()
        {
            var job = await PostJobAsEmployerA();
            var applied = await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA);
            var command = new UpdateApplicationStatusCommand { Status = ApplicationStatus.UnderReview };

            var result = await new UpdateApplicationStatusHandler(applications, jobs).Handle(applied.Application!.Id, command, EmployerA);

            Assert.Equal(ChangeApplicationStatusOutcome.Changed, result.Outcome);
            var history = Assert.Single(applications.History);
            Assert.Equal(ApplicationStatus.Applied, history.FromStatus);
            Assert.Equal(ApplicationStatus.UnderReview, history.ToStatus);
            Assert.Equal(EmployerA, history.ChangedByUserId);
        }

        [Fact]
        public async Task Owner_cannot_skip_stages()
        {
            var job = await PostJobAsEmployerA();
            var applied = await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA);
            var command = new UpdateApplicationStatusCommand { Status = ApplicationStatus.Hired };

            var result = await new UpdateApplicationStatusHandler(applications, jobs).Handle(applied.Application!.Id, command, EmployerA);

            Assert.Equal(ChangeApplicationStatusOutcome.InvalidTransition, result.Outcome);
        }
    }
}
