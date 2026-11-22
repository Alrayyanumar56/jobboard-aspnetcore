using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;

namespace JobBoard.Tests.Fakes
{
    // In-memory ICandidateProfileRepository: at most one profile per JobSeeker, like the unique index.
    public class FakeCandidateProfileRepository : ICandidateProfileRepository
    {
        public List<CandidateProfile> Profiles { get; } = new();

        public Task<CandidateProfile?> GetByJobSeekerId(string jobSeekerId) =>
            Task.FromResult(Profiles.FirstOrDefault(p => p.JobSeekerId == jobSeekerId));

        public Task<bool> TryAddProfile(CandidateProfile profile)
        {
            if (Profiles.Any(p => p.JobSeekerId == profile.JobSeekerId))
            {
                return Task.FromResult(false);
            }
            profile.Id = Profiles.Count == 0 ? 1 : Profiles.Max(p => p.Id) + 1;
            Profiles.Add(profile);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateProfile(CandidateProfile profile)
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
            stored.UpdatedAt = profile.UpdatedAt;
            return Task.FromResult(true);
        }
    }
}
