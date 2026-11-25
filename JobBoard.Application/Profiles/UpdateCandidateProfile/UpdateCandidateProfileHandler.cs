using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Rules;

namespace JobBoard.Application.Profiles.UpdateCandidateProfile
{
    public class UpdateCandidateProfileHandler
    {
        private readonly ICandidateProfileRepository candidateProfileRepository;
        private readonly ISkillRepository skillRepository;

        public UpdateCandidateProfileHandler(ICandidateProfileRepository candidateProfileRepository, ISkillRepository skillRepository)
        {
            this.candidateProfileRepository = candidateProfileRepository;
            this.skillRepository = skillRepository;
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

            // Full replacement: the skills in the command become the profile's whole skill list.
            var skills = await skillRepository.GetOrCreate(SkillNameRules.CleanList(command.Skills));

            // The repository's UPDATE is keyed by JobSeekerId too. False means the row vanished
            // after we read it.
            if (!await candidateProfileRepository.UpdateProfile(profile, skills.Select(s => s.Id).ToList()))
            {
                return null;
            }

            return profile.ToResponse(skills.Select(s => s.Name).Order(StringComparer.OrdinalIgnoreCase).ToList());
        }
    }
}
