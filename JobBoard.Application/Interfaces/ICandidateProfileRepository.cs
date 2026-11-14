using JobBoard.Domain.Entities;

namespace JobBoard.Application.Interfaces
{
    public interface ICandidateProfileRepository
    {
        // The profile owned by this JobSeeker, or null if they haven't created one yet.
        public Task<CandidateProfile?> GetByJobSeekerId(string jobSeekerId);

        // Returns false (instead of throwing) when the unique index on JobSeekerId rejects a second profile.
        public Task<bool> TryAddProfile(CandidateProfile profile);

        // Writes FullName, Headline, Summary, City, Country and UpdatedAt in one UPDATE whose WHERE is
        // the profile's JobSeekerId. JobSeekerId and CreatedAt can't change. False means no row matched.
        public Task<bool> UpdateProfile(CandidateProfile profile);
    }
}
