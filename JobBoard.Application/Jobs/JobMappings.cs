using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.Jobs
{
    // Single place for Job -> JobResponse so the read handlers don't each repeat the mapping.
    internal static class JobMappings
    {
        public static JobResponse ToResponse(this Job job)
        {
            return new JobResponse
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                JobType = job.JobType,
                JobLocation = job.JobLocation,
                SalaryRange = job.SalaryRange,
                EmployerId = job.EmployerId,
                PostedAt = job.PostedAt
            };
        }
    }
}
