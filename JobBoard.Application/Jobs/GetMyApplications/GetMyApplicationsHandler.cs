using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Jobs.GetMyApplications
{
    public class GetMyApplicationsHandler
    {
        private readonly IJobApplicationRepository jobApplicationRepository;

        public GetMyApplicationsHandler(IJobApplicationRepository jobApplicationRepository)
        {
            this.jobApplicationRepository = jobApplicationRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the query string or body. A JobSeeker with no applications gets an empty list.
        public async Task<List<MyApplicationResponse>> Handle(string jobSeekerId)
        {
            return await jobApplicationRepository.GetApplicationsByJobSeeker(jobSeekerId);
        }
    }
}
