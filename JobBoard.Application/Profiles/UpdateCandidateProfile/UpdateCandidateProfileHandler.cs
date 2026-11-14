using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Profiles.UpdateCandidateProfile
{
    public class UpdateCandidateProfileHandler
    {
        private readonly ICandidateProfileRepository candidateProfileRepository;

        public UpdateCandidateProfileHandler(ICandidateProfileRepository candidateProfileRepository)
        {
            this.candidateProfileRepository = candidateProfileRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request body. The profile is looked up BY that id, so there is no other person's profile
        // to reach: ownership is built into the lookup. Returns null when they have no profile yet;
        // the controller turns that into a 404.
        public async Task<CandidateProfileResponse?> Handle(CandidateProfileCommand command, string jobSeekerId)
        {
            var profile = await candidateProfileRepository.GetByJobSeekerId(jobSeekerId);
            if (profile == null)
            {
                return null;
            }

            // GetByJobSeekerId is AsNoTracking, so this object is only a carrier of the new values.
            // JobSeekerId, Id and CreatedAt are never taken from the command.
            profile.ApplyCommand(command);
            profile.UpdatedAt = DateTimeOffset.UtcNow;

            // The repository's UPDATE is keyed by JobSeekerId too. False means the row vanished
            // after we read it.
            if (!await candidateProfileRepository.UpdateProfile(profile))
            {
                return null;
            }

            return profile.ToResponse();
        }
    }
}
