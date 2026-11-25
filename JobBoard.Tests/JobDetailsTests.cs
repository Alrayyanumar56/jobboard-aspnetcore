using JobBoard.Application.Jobs.ApplyToJob;
using JobBoard.Application.Jobs.CreateJob;
using JobBoard.Application.Jobs.GetApplicantProfile;
using JobBoard.Application.Jobs.GetJobById;
using JobBoard.Application.Jobs.GetJobs;
using JobBoard.Application.Jobs.UpdateJob;
using JobBoard.Application.Profiles;
using JobBoard.Application.Profiles.CreateCandidateProfile;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;
using JobBoard.Tests.Fakes;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Tests
{
    // Batch 2 job rules: application deadline, skills, company name and cover letter.
    public class JobDetailsTests
    {
        private const string EmployerA = "employer-a";
        private const string EmployerB = "employer-b";
        private const string SeekerA = "seeker-a";

        private readonly FakeSkillRepository skills = new();
        private readonly FakeEmployerProfileRepository employers = new();
        private readonly FakeJobRepository jobs;
        private readonly FakeJobApplicationRepository applications = new();
        private readonly FakeCandidateProfileRepository profiles;

        public JobDetailsTests()
        {
            jobs = new FakeJobRepository(skills, employers);
            profiles = new FakeCandidateProfileRepository(skills);
        }

        private Task<Job> Post(DateTimeOffset? deadline = null, string title = "Backend Developer") =>
            jobs.AddJob(new Job
            {
                Title = title,
                Description = "Build APIs.",
                JobType = JobType.FullTime,
                JobLocation = Location.Hybrid,
                City = "Lahore",
                ExperienceLevel = ExperienceLevel.Junior,
                ApplicationDeadline = deadline,
                EmployerId = EmployerA,
                PostedAt = DateTimeOffset.UtcNow
            });

        private static CreateJobCommand ValidCreate(params string[] skillNames) => new()
        {
            Title = "Backend Developer",
            Description = "Build APIs.",
            JobType = JobType.FullTime,
            JobLocation = Location.Hybrid,
            City = " Karachi ",
            ExperienceLevel = ExperienceLevel.Mid,
            ApplicationDeadline = DateTimeOffset.UtcNow.AddDays(14),
            Skills = skillNames.ToList()
        };

        private static List<ValidationResult> Validate(object command)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), results, validateAllProperties: true);
            return results;
        }

        // ---------- Deadline ----------

        [Fact]
        public async Task Job_past_its_deadline_is_hidden_from_listing_and_total()
        {
            await Post(deadline: DateTimeOffset.UtcNow.AddMinutes(-1), title: "Expired");
            await Post(deadline: DateTimeOffset.UtcNow.AddDays(1), title: "Still open");
            await Post(deadline: null, title: "No deadline");

            var result = await new GetJobsHandler(jobs).Handle(new GetJobsQuery());

            Assert.Equal(2, result.TotalCount);
            Assert.DoesNotContain(result.Items, j => j.Title == "Expired");
        }

        [Fact]
        public async Task Job_past_its_deadline_can_still_be_opened_by_id()
        {
            var job = await Post(deadline: DateTimeOffset.UtcNow.AddMinutes(-1));

            var result = await new GetJobByIdHandler(jobs).Handle(job.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Applying_after_the_deadline_is_rejected()
        {
            var job = await Post(deadline: DateTimeOffset.UtcNow.AddMinutes(-1));

            var result = await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA);

            Assert.Equal(ApplyToJobStatus.DeadlinePassed, result.Status);
            Assert.Empty(applications.Applications);
        }

        [Fact]
        public async Task Applying_before_the_deadline_works()
        {
            var job = await Post(deadline: DateTimeOffset.UtcNow.AddDays(1));

            var result = await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA);

            Assert.Equal(ApplyToJobStatus.Created, result.Status);
        }

        [Fact]
        public void Create_command_with_past_deadline_is_invalid()
        {
            var command = ValidCreate();
            command.ApplicationDeadline = DateTimeOffset.UtcNow.AddHours(-1);

            var errors = Validate(command);

            Assert.Contains(errors, e => e.MemberNames.Contains("ApplicationDeadline"));
        }

        [Fact]
        public void Create_command_without_experience_level_is_invalid()
        {
            var command = ValidCreate();
            command.ExperienceLevel = null;

            Assert.Contains(Validate(command), e => e.MemberNames.Contains("ExperienceLevel"));
        }

        [Fact]
        public void Create_command_with_more_than_20_skills_is_invalid()
        {
            var command = ValidCreate(Enumerable.Range(1, 21).Select(i => $"Skill {i}").ToArray());

            Assert.Contains(Validate(command), e => e.MemberNames.Contains("Skills"));
        }

        [Fact]
        public void Valid_create_command_passes()
        {
            Assert.Empty(Validate(ValidCreate("C#", "SQL")));
        }

        // ---------- Skills and company name ----------

        [Fact]
        public async Task Created_job_returns_cleaned_fields_skills_and_company_name()
        {
            await employers.TryAddProfile(new EmployerProfile { EmployerId = EmployerA, CompanyName = "Systems Ltd" });

            var result = await new CreateJobHandler(jobs, skills).Handle(ValidCreate("SQL", " c# ", "C#"), EmployerA);

            Assert.Equal("Karachi", result.City);
            Assert.Equal(ExperienceLevel.Mid, result.ExperienceLevel);
            Assert.Equal("Systems Ltd", result.CompanyName);
            Assert.Equal(["c#", "SQL"], result.Skills);
        }

        [Fact]
        public async Task Job_of_employer_without_profile_has_null_company_name()
        {
            var job = await Post();

            var result = await new GetJobByIdHandler(jobs).Handle(job.Id);

            Assert.Null(result!.CompanyName);
        }

        [Fact]
        public async Task Same_skill_from_a_job_and_a_candidate_is_one_shared_row()
        {
            await new CreateJobHandler(jobs, skills).Handle(ValidCreate("ASP.NET Core"), EmployerA);
            await new CreateCandidateProfileHandler(profiles, skills).Handle(
                new CandidateProfileCommand { FullName = "Ayesha Khan", Skills = ["asp.net   core"] }, SeekerA);

            var skill = Assert.Single(skills.Skills);
            Assert.Equal("ASP.NET Core", skill.Name);
            Assert.Equal(skill.Id, Assert.Single(skills.JobSkills).SkillId);
            Assert.Equal(skill.Id, Assert.Single(skills.CandidateSkills).SkillId);
        }

        [Fact]
        public async Task Updating_a_job_replaces_its_skills()
        {
            var created = await new CreateJobHandler(jobs, skills).Handle(ValidCreate("C#", "SQL"), EmployerA);
            var update = new UpdateJobCommand
            {
                Title = "Backend Developer",
                Description = "Build APIs.",
                JobType = JobType.FullTime,
                JobLocation = Location.Remote,
                ExperienceLevel = ExperienceLevel.Senior,
                Skills = ["Azure"]
            };

            var result = await new UpdateJobHandler(jobs, skills).Handle(created.Id, update, EmployerA);

            Assert.Equal(UpdateJobStatus.Updated, result.Status);
            Assert.Equal(["Azure"], result.Job!.Skills);
            Assert.Null(result.Job.ApplicationDeadline);
        }

        [Fact]
        public async Task Employer_B_cannot_change_skills_of_employer_A_job()
        {
            var created = await new CreateJobHandler(jobs, skills).Handle(ValidCreate("C#"), EmployerA);
            var update = new UpdateJobCommand
            {
                Title = "Hijacked",
                Description = "x",
                JobType = JobType.FullTime,
                JobLocation = Location.Remote,
                ExperienceLevel = ExperienceLevel.Senior,
                Skills = ["Azure"]
            };

            var result = await new UpdateJobHandler(jobs, skills).Handle(created.Id, update, EmployerB);

            Assert.Equal(UpdateJobStatus.JobNotFound, result.Status);
            Assert.Equal(["C#"], skills.JobSkillNames(created.Id));
        }

        // ---------- Cover letter ----------

        [Fact]
        public async Task Cover_letter_is_saved_trimmed_and_shown_to_the_job_owner_only()
        {
            var job = await Post();
            var applied = await new ApplyToJobHandler(jobs, applications)
                .Handle(job.Id, SeekerA, new ApplyToJobCommand { CoverLetter = "  I'd love to join.  " });
            var education = new FakeCandidateEducationRepository(profiles);
            var handler = new GetApplicantProfileHandler(applications, jobs, profiles, education, skills);

            var forOwner = await handler.Handle(applied.Application!.Id, EmployerA);
            var forOther = await handler.Handle(applied.Application.Id, EmployerB);

            Assert.Equal("I'd love to join.", applied.Application.CoverLetter);
            Assert.Equal("I'd love to join.", forOwner!.CoverLetter);
            Assert.Null(forOther);
        }

        [Fact]
        public async Task Applying_without_a_body_stores_no_cover_letter()
        {
            var job = await Post();

            var applied = await new ApplyToJobHandler(jobs, applications).Handle(job.Id, SeekerA, null);

            Assert.Null(applications.Applications.Single().CoverLetter);
            Assert.Null(applied.Application!.CoverLetter);
        }
    }
}
