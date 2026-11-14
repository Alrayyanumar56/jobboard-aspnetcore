using JobBoard.Domain.Entities;

namespace JobBoard.Application.Interfaces
{
    public interface ICandidateEducationRepository
    {
        // Every education record on this JobSeeker's profile, most recent StartDate first.
        // Empty when they have none (or no profile yet).
        public Task<List<CandidateEducation>> GetAllForJobSeeker(string jobSeekerId);

        // One record, but only if it sits on THIS JobSeeker's profile. Null means "missing or not
        // yours": callers can't tell which, so ids of other people's records are never revealed.
        public Task<CandidateEducation?> GetByIdForJobSeeker(string jobSeekerId, int educationId);

        // The caller must have set CandidateProfileId from the profile they looked up via the JWT.
        public Task AddEducation(CandidateEducation education);

        // One UPDATE whose WHERE requires both the record id AND ownership by jobSeekerId.
        // Only the editable columns are set; Id and CandidateProfileId can't change. False means no row matched.
        public Task<bool> UpdateEducation(string jobSeekerId, CandidateEducation education);

        // One DELETE whose WHERE requires both the record id AND ownership by jobSeekerId. False means no row matched.
        public Task<bool> DeleteEducation(string jobSeekerId, int educationId);
    }
}
