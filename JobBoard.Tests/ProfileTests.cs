using JobBoard.Application.EmployerProfiles;
using JobBoard.Application.EmployerProfiles.CreateEmployerProfile;
using JobBoard.Application.EmployerProfiles.GetMyEmployerProfile;
using JobBoard.Application.EmployerProfiles.UpdateEmployerProfile;
using JobBoard.Application.Profiles;
using JobBoard.Application.Profiles.CreateCandidateProfile;
using JobBoard.Application.Profiles.GetMyCandidateProfile;
using JobBoard.Application.Profiles.UpdateCandidateProfile;
using JobBoard.Domain.Enums;
using JobBoard.Tests.Fakes;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Tests
{
    // Employer (company) profile, and the new candidate-profile fields and skills.
    public class ProfileTests
    {
        private const string EmployerA = "employer-a";
        private const string EmployerB = "employer-b";
        private const string SeekerA = "seeker-a";

        private readonly FakeEmployerProfileRepository employers = new();
        private readonly FakeSkillRepository skills = new();
        private readonly FakeCandidateProfileRepository candidates;

        public ProfileTests()
        {
            candidates = new FakeCandidateProfileRepository(skills);
        }

        private static List<ValidationResult> Validate(object command)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(command, new ValidationContext(command), results, validateAllProperties: true);
            return results;
        }

        private static EmployerProfileCommand Company(string name = "  Arbisoft  ") => new()
        {
            CompanyName = name,
            Website = "https://arbisoft.com",
            City = "Lahore"
        };

        // ---------- Employer profile ----------

        [Fact]
        public async Task Employer_creates_one_profile_with_trimmed_name()
        {
            var created = await new CreateEmployerProfileHandler(employers).Handle(Company(), EmployerA);

            Assert.Equal("Arbisoft", created!.CompanyName);
            Assert.Equal(EmployerA, employers.Profiles.Single().EmployerId);
        }

        [Fact]
        public async Task Second_employer_profile_is_rejected()
        {
            var handler = new CreateEmployerProfileHandler(employers);
            await handler.Handle(Company(), EmployerA);

            var second = await handler.Handle(Company("Another"), EmployerA);

            Assert.Null(second);
            Assert.Single(employers.Profiles);
        }

        [Fact]
        public async Task Each_employer_only_reads_and_updates_their_own_profile()
        {
            await new CreateEmployerProfileHandler(employers).Handle(Company(), EmployerA);

            var readByB = await new GetMyEmployerProfileHandler(employers).Handle(EmployerB);
            var updateByB = await new UpdateEmployerProfileHandler(employers).Handle(Company("Hijacked"), EmployerB);

            Assert.Null(readByB);
            Assert.Null(updateByB);
            Assert.Equal("Arbisoft", employers.Profiles.Single().CompanyName);
        }

        [Fact]
        public async Task Owner_update_replaces_fields_and_blank_clears_optional_ones()
        {
            await new CreateEmployerProfileHandler(employers).Handle(Company(), EmployerA);

            var updated = await new UpdateEmployerProfileHandler(employers)
                .Handle(new EmployerProfileCommand { CompanyName = "Arbisoft Pvt Ltd", Website = "  " }, EmployerA);

            Assert.Equal("Arbisoft Pvt Ltd", updated!.CompanyName);
            Assert.Null(updated.Website);
            Assert.Null(updated.City);
        }

        [Theory]
        [InlineData("ftp://example.com")]
        [InlineData("example.com")]
        [InlineData("javascript:alert(1)")]
        public void Website_must_be_http_or_https(string website)
        {
            var command = Company();
            command.Website = website;

            Assert.Contains(Validate(command), e => e.MemberNames.Contains("Website"));
        }

        // ---------- Candidate profile additions ----------

        [Fact]
        public async Task Candidate_profile_saves_new_fields_and_skills()
        {
            var command = new CandidateProfileCommand
            {
                FullName = "Ayesha Khan",
                CvUrl = "https://drive.example.com/cv.pdf",
                YearsOfExperience = 3,
                ExperienceLevel = ExperienceLevel.Mid,
                Skills = ["SQL", "C#", "sql"]
            };
            await new CreateCandidateProfileHandler(candidates, skills).Handle(command, SeekerA);

            var mine = await new GetMyCandidateProfileHandler(candidates, skills).Handle(SeekerA);

            Assert.Equal("https://drive.example.com/cv.pdf", mine!.CvUrl);
            Assert.Equal(3, mine.YearsOfExperience);
            Assert.Equal(ExperienceLevel.Mid, mine.ExperienceLevel);
            Assert.Equal(["C#", "SQL"], mine.Skills);
        }

        [Fact]
        public async Task Candidate_update_replaces_skills()
        {
            await new CreateCandidateProfileHandler(candidates, skills)
                .Handle(new CandidateProfileCommand { FullName = "Ayesha Khan", Skills = ["C#", "SQL"] }, SeekerA);

            var updated = await new UpdateCandidateProfileHandler(candidates, skills)
                .Handle(new CandidateProfileCommand { FullName = "Ayesha Khan", Skills = ["React"] }, SeekerA);

            Assert.Equal(["React"], updated!.Skills);
            Assert.Equal(["React"], (await new GetMyCandidateProfileHandler(candidates, skills).Handle(SeekerA))!.Skills);
        }

        [Fact]
        public void CvUrl_must_be_http_or_https()
        {
            var command = new CandidateProfileCommand { FullName = "Ayesha Khan", CvUrl = "file:///C:/cv.pdf" };

            Assert.Contains(Validate(command), e => e.MemberNames.Contains("CvUrl"));
        }

        [Fact]
        public void Years_of_experience_must_be_between_0_and_60()
        {
            var command = new CandidateProfileCommand { FullName = "Ayesha Khan", YearsOfExperience = 61 };

            Assert.Contains(Validate(command), e => e.MemberNames.Contains("YearsOfExperience"));
        }
    }
}
