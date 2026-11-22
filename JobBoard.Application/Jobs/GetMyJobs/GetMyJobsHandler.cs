using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Jobs.GetMyJobs
{
    public class GetMyJobsHandler
    {
        private readonly IJobRepository jobRepository;

        public GetMyJobsHandler(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the request. An employer with no jobs gets an empty list.
        public async Task<List<MyJobResponse>> Handle(string employerId)
        {
            return await jobRepository.GetJobsByEmployer(employerId);
        }
    }
}
