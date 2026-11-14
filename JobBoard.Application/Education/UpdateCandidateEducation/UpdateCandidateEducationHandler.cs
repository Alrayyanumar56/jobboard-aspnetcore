using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Education.UpdateCandidateEducation
{
    public class UpdateCandidateEducationHandler
    {
        private readonly ICandidateEducationRepository candidateEducationRepository;

        public UpdateCandidateEducationHandler(ICandidateEducationRepository candidateEducationRepository)
        {
            this.candidateEducationRepository = candidateEducationRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request body. Returns null when the record doesn't exist OR belongs to someone else: the
        // two cases are deliberately indistinguishable, so nobody can probe which ids exist. The
        // controller turns null into a 404.
        public async Task<CandidateEducationResponse?> Handle(int educationId, CandidateEducationCommand command, string jobSeekerId)
        {
            // Ownership is part of the lookup: only records on the caller's own profile can be found.
            var education = await candidateEducationRepository.GetByIdForJobSeeker(jobSeekerId, educationId);
            if (education == null)
            {
                return null;
            }

            // GetByIdForJobSeeker is AsNoTracking, so this object only carries the new values.
            // Id and CandidateProfileId are never taken from the command.
            education.ApplyCommand(command);

            // The repository's UPDATE requires the owner too. False means the row vanished after we read it.
            if (!await candidateEducationRepository.UpdateEducation(jobSeekerId, education))
            {
                return null;
            }

            return education.ToResponse();
        }
    }
}
