using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.Education.CreateCandidateEducation
{
    public class CreateCandidateEducationHandler
    {
        private readonly ICandidateProfileRepository candidateProfileRepository;
        private readonly ICandidateEducationRepository candidateEducationRepository;

        public CreateCandidateEducationHandler(ICandidateProfileRepository candidateProfileRepository, ICandidateEducationRepository candidateEducationRepository)
        {
            this.candidateProfileRepository = candidateProfileRepository;
            this.candidateEducationRepository = candidateEducationRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request body. Returns null when this JobSeeker has no profile yet; the controller turns
        // that into a 409.
        public async Task<CandidateEducationResponse?> Handle(CandidateEducationCommand command, string jobSeekerId)
        {
            // The profile is found BY the caller's id. Its Id is the only profile id that ever ends up
            // on the new record, so a record can only be attached to the caller's own profile.
            var profile = await candidateProfileRepository.GetByJobSeekerId(jobSeekerId);
            if (profile == null)
            {
                return null;
            }

            var education = new CandidateEducation { CandidateProfileId = profile.Id };
            education.ApplyCommand(command);

            await candidateEducationRepository.AddEducation(education);

            // Id was filled in by the database during the insert.
            return education.ToResponse();
        }
    }
}
