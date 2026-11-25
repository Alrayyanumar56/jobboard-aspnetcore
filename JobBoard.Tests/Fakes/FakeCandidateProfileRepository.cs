using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;

namespace JobBoard.Tests.Fakes
{
    // In-memory ICandidateProfileRepository: at most one profile per JobSeeker, like the unique index.
    public class FakeCandidateProfileRepository : ICandidateProfileRepository
    {
        private readonly FakeSkillRepository skills;

        public FakeCandidateProfileRepository(FakeSkillRepository? skills = null)
        {
            this.skills = skills ?? new FakeSkillRepository();
        }

        public List<CandidateProfile> Profiles { get; } = new();

        // Test convenience: a profile without skills.
        public Task<bool> TryAddProfile(CandidateProfile profile) => TryAddProfile(profile, []);

        public Task<CandidateProfile?> GetByJobSeekerId(string jobSeekerId) =>
            Task.FromResult(Profiles.FirstOrDefault(p => p.JobSeekerId == jobSeekerId));

        public Task<bool> TryAddProfile(CandidateProfile profile, IReadOnlyCollection<int> skillIds)
        {
            if (Profiles.Any(p => p.JobSeekerId == profile.JobSeekerId))
            {
                return Task.FromResult(false);
            }
            profile.Id = Profiles.Count == 0 ? 1 : Profiles.Max(p => p.Id) + 1;
            Profiles.Add(profile);
            skills.ReplaceCandidateSkills(profile.Id, skillIds);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateProfile(CandidateProfile profile, IReadOnlyCollection<int> skillIds)
        {
            var stored = Profiles.FirstOrDefault(p => p.JobSeekerId == profile.JobSeekerId);
            if (stored == null)
            {
                return Task.FromResult(false);
            }
            stored.FullName = profile.FullName;
            stored.Headline = profile.Headline;
            stored.Summary = profile.Summary;
            stored.City = profile.City;
            stored.Country = profile.Country;
            stored.CvUrl = profile.CvUrl;
            stored.YearsOfExperience = profile.YearsOfExperience;
            stored.ExperienceLevel = profile.ExperienceLevel;
            stored.UpdatedAt = profile.UpdatedAt;
            skills.ReplaceCandidateSkills(stored.Id, skillIds);
            return Task.FromResult(true);
        }
    }
}
