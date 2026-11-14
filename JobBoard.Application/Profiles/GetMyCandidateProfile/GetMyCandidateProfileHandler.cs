using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Profiles.GetMyCandidateProfile
{
    public class GetMyCandidateProfileHandler
    {
        private readonly ICandidateProfileRepository candidateProfileRepository;

        public GetMyCandidateProfileHandler(ICandidateProfileRepository candidateProfileRepository)
        {
            this.candidateProfileRepository = candidateProfileRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request. The lookup is keyed by it, so a JobSeeker can only ever read their own profile.
        // Returns null when they haven't created one yet; the controller turns that into a 404.
        public async Task<CandidateProfileResponse?> Handle(string jobSeekerId)
        {
            var profile = await candidateProfileRepository.GetByJobSeekerId(jobSeekerId);
            return profile?.ToResponse();
        }
    }
}
