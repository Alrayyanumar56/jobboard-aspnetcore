using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;

namespace JobBoard.Tests.Fakes
{
    // In-memory ICandidateEducationRepository. Ownership goes through the profile, as in the real one:
    // a record belongs to a JobSeeker when its CandidateProfileId is that JobSeeker's profile.
    public class FakeCandidateEducationRepository : ICandidateEducationRepository
    {
        private readonly FakeCandidateProfileRepository profiles;

        public FakeCandidateEducationRepository(FakeCandidateProfileRepository profiles)
        {
            this.profiles = profiles;
        }

        public List<CandidateEducation> Records { get; } = new();

        private IEnumerable<CandidateEducation> OwnedBy(string jobSeekerId)
        {
            var profile = profiles.Profiles.FirstOrDefault(p => p.JobSeekerId == jobSeekerId);
            return profile == null ? [] : Records.Where(e => e.CandidateProfileId == profile.Id);
        }

        public Task<List<CandidateEducation>> GetAllForJobSeeker(string jobSeekerId) =>
            Task.FromResult(OwnedBy(jobSeekerId)
                .OrderByDescending(e => e.StartDate)
                .ThenByDescending(e => e.Id)
                .ToList());

        public Task<CandidateEducation?> GetByIdForJobSeeker(string jobSeekerId, int educationId) =>
            Task.FromResult(OwnedBy(jobSeekerId).FirstOrDefault(e => e.Id == educationId));

        public Task AddEducation(CandidateEducation education)
        {
            education.Id = Records.Count == 0 ? 1 : Records.Max(e => e.Id) + 1;
            Records.Add(education);
            return Task.CompletedTask;
        }

        public Task<bool> UpdateEducation(string jobSeekerId, CandidateEducation education)
        {
            var stored = OwnedBy(jobSeekerId).FirstOrDefault(e => e.Id == education.Id);
            if (stored == null)
            {
                return Task.FromResult(false);
            }
            stored.Institution = education.Institution;
            stored.Degree = education.Degree;
            stored.FieldOfStudy = education.FieldOfStudy;
            stored.StartDate = education.StartDate;
            stored.EndDate = education.EndDate;
            stored.Description = education.Description;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteEducation(string jobSeekerId, int educationId)
        {
            var stored = OwnedBy(jobSeekerId).FirstOrDefault(e => e.Id == educationId);
            return Task.FromResult(stored != null && Records.Remove(stored));
        }
    }
}
