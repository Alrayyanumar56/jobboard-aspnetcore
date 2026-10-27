using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Jobs.GetJobs
{
    public class GetJobsHandler
    {
        private readonly IJobRepository jobRepository;

        public GetJobsHandler(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        public async Task<List<JobResponse>> Handle()
        {
            var jobs = await jobRepository.GetAllJobs();
            return jobs.Select(j => j.ToResponse()).ToList();
        }
    }
}
