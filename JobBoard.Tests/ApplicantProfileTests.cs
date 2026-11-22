using JobBoard.Application.Jobs.ApplyToJob;
using JobBoard.Application.Jobs.CloseJob;
using JobBoard.Application.Jobs.GetApplicantProfile;
using JobBoard.Application.Jobs.WithdrawApplication;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;
using JobBoard.Tests.Fakes;

namespace JobBoard.Tests
{
    // An employer may see an applicant's full profile and education only through an application on
    // one of THEIR jobs. Anything else looks exactly like "no such application" (null -> 404).
    public class ApplicantProfileTests
    {
        private const string EmployerA = "employer-a";
        private const string EmployerB = "employer-b";
        private const string SeekerA = "seeker-a";

        private readonly FakeJobRepository jobs = new();
        private readonly FakeJobApplicationRepository applications = new();
        private readonly FakeCandidateProfileRepository profiles = new();
        private readonly FakeCandidateEducationRepository education;

        public ApplicantProfileTests()
        {
            education = new FakeCandidateEducationRepository(profiles);
        }

        private GetApplicantProfileHandler Handler() => new(applications, jobs, profiles, education);

        private async Task<int> SeekerAAppliesToEmployerAJob()
        {
            var job = await jobs.AddJob(new Job
            {
                Title = "Junior .NET Developer",
                Description = "Build APIs.",
                JobType = JobType.FullTime,
                JobLocation = Location.OnSite,
                EmployerId = EmployerA,
                PostedAt = DateTimeOffset.UtcNow
            });
            var applied = await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA);
            return applied.Application!.Id;
        }

        private async Task GiveSeekerAProfileWithEducation()
        {
            await profiles.TryAddProfile(new CandidateProfile { JobSeekerId = SeekerA, FullName = "Ayesha Khan" });
            var profileId = profiles.Profiles.Single().Id;
            await education.AddEducation(new CandidateEducation
            {
                CandidateProfileId = profileId,
                Institution = "NUST",
                Degree = "BS",
                FieldOfStudy = "Computer Science",
                StartDate = new DateOnly(2019, 9, 1),
                EndDate = new DateOnly(2023, 6, 30)
            });
        }

        [Fact]
        public async Task Owner_sees_applicant_profile_and_education()
        {
            var applicationId = await SeekerAAppliesToEmployerAJob();
            await GiveSeekerAProfileWithEducation();

            var result = await Handler().Handle(applicationId, EmployerA);

            Assert.NotNull(result);
            Assert.Equal(applicationId, result.ApplicationId);
            Assert.Equal("Ayesha Khan", result.Profile!.FullName);
            Assert.Equal("NUST", Assert.Single(result.Education).Institution);
        }

        [Fact]
        public async Task Employer_B_cannot_see_applicant_on_employer_A_job()
        {
            var applicationId = await SeekerAAppliesToEmployerAJob();
            await GiveSeekerAProfileWithEducation();

            var result = await Handler().Handle(applicationId, EmployerB);

            Assert.Null(result);
        }

        [Fact]
        public async Task Missing_application_gives_null()
        {
            var result = await Handler().Handle(999, EmployerA);

            Assert.Null(result);
        }

        [Fact]
        public async Task Applicant_without_profile_gives_empty_result_not_null()
        {
            var applicationId = await SeekerAAppliesToEmployerAJob();

            var result = await Handler().Handle(applicationId, EmployerA);

            Assert.NotNull(result);
            Assert.Null(result.Profile);
            Assert.Empty(result.Education);
        }

        [Fact]
        public async Task Owner_still_sees_applicant_after_job_closed_and_application_withdrawn()
        {
            var applicationId = await SeekerAAppliesToEmployerAJob();
            await GiveSeekerAProfileWithEducation();
            await new WithdrawApplicationHandler(applications).Handle(applicationId, SeekerA);
            await new CloseJobHandler(jobs).Handle(jobs.Jobs.Single().Id, EmployerA);

            var result = await Handler().Handle(applicationId, EmployerA);

            Assert.NotNull(result);
            Assert.Equal("Ayesha Khan", result.Profile!.FullName);
        }
    }
}
