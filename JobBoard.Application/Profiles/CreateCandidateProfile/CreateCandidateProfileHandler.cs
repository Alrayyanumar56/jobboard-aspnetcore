using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.Profiles.CreateCandidateProfile
{
    public class CreateCandidateProfileHandler
    {
        private readonly ICandidateProfileRepository candidateProfileRepository;

        public CreateCandidateProfileHandler(ICandidateProfileRepository candidateProfileRepository)
        {
            this.candidateProfileRepository = candidateProfileRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request body. Returns null when this JobSeeker already has a profile; the controller
        // turns that into a 409.
        public async Task<CandidateProfileResponse?> Handle(CandidateProfileCommand command, string jobSeekerId)
        {
            // Friendly early check for the normal "already created" case.
            if (await candidateProfileRepository.GetByJobSeekerId(jobSeekerId) != null)
            {
                return null;
            }

            var now = DateTimeOffset.UtcNow;
            var profile = new CandidateProfile
            {
                JobSeekerId = jobSeekerId,
                CreatedAt = now,
                UpdatedAt = now
            };
            profile.ApplyCommand(command);

            // Two simultaneous requests can both pass the check above. The unique index on JobSeekerId
            // is the real guarantee: it rejects the second insert and TryAddProfile reports false.
            if (!await candidateProfileRepository.TryAddProfile(profile))
            {
                return null;
            }

            return profile.ToResponse();
        }
    }
}
