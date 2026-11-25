using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Rules;

namespace JobBoard.Application.Profiles.CreateCandidateProfile
{
    public class CreateCandidateProfileHandler
    {
        private readonly ICandidateProfileRepository candidateProfileRepository;
        private readonly ISkillRepository skillRepository;

        public CreateCandidateProfileHandler(ICandidateProfileRepository candidateProfileRepository, ISkillRepository skillRepository)
        {
            this.candidateProfileRepository = candidateProfileRepository;
            this.skillRepository = skillRepository;
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

            // Skill names -> shared Skill rows (new ones are created), the same vocabulary jobs use.
            var skills = await skillRepository.GetOrCreate(SkillNameRules.CleanList(command.Skills));

            // Two simultaneous requests can both pass the check above. The unique index on JobSeekerId
            // is the real guarantee: it rejects the second insert and TryAddProfile reports false.
            if (!await candidateProfileRepository.TryAddProfile(profile, skills.Select(s => s.Id).ToList()))
            {
                return null;
            }

            return profile.ToResponse(skills.Select(s => s.Name).Order(StringComparer.OrdinalIgnoreCase).ToList());
        }
    }
}
