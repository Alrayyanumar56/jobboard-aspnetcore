using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Education.DeleteCandidateEducation
{
    public class DeleteCandidateEducationHandler
    {
        private readonly ICandidateEducationRepository candidateEducationRepository;

        public DeleteCandidateEducationHandler(ICandidateEducationRepository candidateEducationRepository)
        {
            this.candidateEducationRepository = candidateEducationRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller). Returns false
        // when the record doesn't exist OR belongs to someone else (same answer for both, so ids can't be
        // probed); the controller turns false into a 404. The delete itself requires ownership, so there
        // is no read-then-delete gap to exploit.
        public async Task<bool> Handle(int educationId, string jobSeekerId)
        {
            return await candidateEducationRepository.DeleteEducation(jobSeekerId, educationId);
        }
    }
}
