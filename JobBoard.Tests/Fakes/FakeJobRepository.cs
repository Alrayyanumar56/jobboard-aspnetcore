using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Application.Jobs.GetMyJobs;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;

namespace JobBoard.Tests.Fakes
{
    // In-memory IJobRepository. Mirrors the real repository's rules (UpdateJob and CloseJob only match
    // the owner's OPEN job) so handler tests exercise the same contract as production.
    public class FakeJobRepository : IJobRepository
    {
        public List<Job> Jobs { get; } = new();

        public Task<Job> AddJob(Job job)
        {
            job.Id = Jobs.Count == 0 ? 1 : Jobs.Max(j => j.Id) + 1;
            Jobs.Add(job);
            return Task.FromResult(job);
        }

        public Task<(List<Job> Items, int TotalCount)> GetOpenJobsPage(JobType? jobType, Location? location, string? search, int page, int pageSize)
        {
            var matching = Jobs
                .Where(j => j.ClosedAt == null)
                .Where(j => !jobType.HasValue || j.JobType == jobType.Value)
                .Where(j => !location.HasValue || j.JobLocation == location.Value)
                .Where(j => string.IsNullOrEmpty(search)
                    || j.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || j.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(j => j.PostedAt)
                .ThenByDescending(j => j.Id)
                .ToList();
            var items = matching.Skip((page - 1) * pageSize).Take(pageSize).Select(Copy).ToList();
            return Task.FromResult((items, matching.Count));
        }

        // Returns a COPY, like AsNoTracking: changing the result must not change the stored job.
        public Task<Job?> GetJobById(int id)
        {
            var job = Jobs.FirstOrDefault(j => j.Id == id);
            return Task.FromResult(job == null ? null : Copy(job));
        }

        public Task<List<MyJobResponse>> GetJobsByEmployer(string employerId, JobStatusFilter status)
        {
            var result = Jobs
                .Where(j => j.EmployerId == employerId)
                .Where(j => status == JobStatusFilter.All
                    || (status == JobStatusFilter.Open && j.ClosedAt == null)
                    || (status == JobStatusFilter.Closed && j.ClosedAt != null))
                .OrderByDescending(j => j.PostedAt)
                .Select(j => new MyJobResponse
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    JobType = j.JobType,
                    JobLocation = j.JobLocation,
                    SalaryRange = j.SalaryRange,
                    PostedAt = j.PostedAt,
                    ClosedAt = j.ClosedAt
                })
                .ToList();
            return Task.FromResult(result);
        }

        public Task<bool> UpdateJob(Job job)
        {
            var stored = Jobs.FirstOrDefault(j => j.Id == job.Id && j.EmployerId == job.EmployerId && j.ClosedAt == null);
            if (stored == null)
            {
                return Task.FromResult(false);
            }
            stored.Title = job.Title;
            stored.Description = job.Description;
            stored.JobType = job.JobType;
            stored.JobLocation = job.JobLocation;
            stored.SalaryRange = job.SalaryRange;
            return Task.FromResult(true);
        }

        public Task<bool> CloseJob(int id, string employerId, DateTimeOffset closedAt)
        {
            var stored = Jobs.FirstOrDefault(j => j.Id == id && j.EmployerId == employerId && j.ClosedAt == null);
            if (stored == null)
            {
                return Task.FromResult(false);
            }
            stored.ClosedAt = closedAt;
            return Task.FromResult(true);
        }

        private static Job Copy(Job j) => new()
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            JobType = j.JobType,
            JobLocation = j.JobLocation,
            EmployerId = j.EmployerId,
            PostedAt = j.PostedAt,
            SalaryRange = j.SalaryRange,
            ClosedAt = j.ClosedAt
        };
    }
}
