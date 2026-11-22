using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Enums;

namespace JobBoard.Application.Jobs.GetJobs
{
    public class GetJobsHandler
    {
        private readonly IJobRepository jobRepository;

        public GetJobsHandler(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        // Both filters are optional; null means "any".
        public async Task<List<JobResponse>> Handle(JobType? jobType = null, Location? location = null)
        {
            var jobs = await jobRepository.GetAllJobs(jobType, location);
            return jobs.Select(j => j.ToResponse()).ToList();
        }
    }
}
