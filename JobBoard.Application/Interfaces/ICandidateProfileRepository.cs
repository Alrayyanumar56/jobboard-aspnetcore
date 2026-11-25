using JobBoard.Domain.Entities;

namespace JobBoard.Application.Interfaces
{
    public interface ICandidateProfileRepository
    {
        // The profile owned by this JobSeeker, or null if they haven't created one yet.
        public Task<CandidateProfile?> GetByJobSeekerId(string jobSeekerId);

        // Saves the profile and links it to skillIds, together or not at all. Sets profile.Id.
        // Returns false (instead of throwing) when the unique index on JobSeekerId rejects a second profile.
        public Task<bool> TryAddProfile(CandidateProfile profile, IReadOnlyCollection<int> skillIds);

        // Writes the editable columns and UpdatedAt in one UPDATE whose WHERE is the profile's JobSeekerId,
        // and replaces the profile's skill links with skillIds, together or not at all.
        // JobSeekerId and CreatedAt can't change. False means no row matched (and nothing was written).
        public Task<bool> UpdateProfile(CandidateProfile profile, IReadOnlyCollection<int> skillIds);
    }
}
