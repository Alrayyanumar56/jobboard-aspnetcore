using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Education.GetMyCandidateEducation
{
    public class GetMyCandidateEducationHandler
    {
        private readonly ICandidateEducationRepository candidateEducationRepository;

        public GetMyCandidateEducationHandler(ICandidateEducationRepository candidateEducationRepository)
        {
            this.candidateEducationRepository = candidateEducationRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request. The repository only returns records on THAT user's profile, so there is no way
        // to read anyone else's. No education (or no profile yet) is an empty list, not an error.
        public async Task<List<CandidateEducationResponse>> Handle(string jobSeekerId)
        {
            var records = await candidateEducationRepository.GetAllForJobSeeker(jobSeekerId);
            return records.Select(e => e.ToResponse()).ToList();
        }
    }
}
