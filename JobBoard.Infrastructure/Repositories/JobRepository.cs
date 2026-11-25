using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Application.Jobs.GetMyJobs;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;
using JobBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext dbContext;

        public JobRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Job> AddJob(Job job, IReadOnlyCollection<int> skillIds)
        {
            // The job row and its skill links must be saved together. The DbContext retries transient
            // errors, and a retrying strategy only allows our own transaction inside ExecuteAsync, which
            // replays the WHOLE block after a failure (same pattern as TryChangeStatus).
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // A retry starts from a clean slate: forget whatever the failed attempt left tracked.
                dbContext.ChangeTracker.Clear();
                job.Id = 0;

                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                // First save gives the job its identity Id, which the link rows need (no navigation
                // properties, so EF can't fill JobId in by itself).
                dbContext.Job.Add(job);
                await dbContext.SaveChangesAsync();

                dbContext.JobSkill.AddRange(skillIds.Select(skillId => new JobSkill { JobId = job.Id, SkillId = skillId }));
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            });
            return job;
        }

        public async Task<(List<JobResponse> Items, int TotalCount)> GetOpenJobsPage(JobType? jobType, Location? location, string? search, int page, int pageSize, DateTimeOffset now)
        {
            // Public listing: closed jobs and jobs past their deadline are not shown.
            var query = dbContext.Job
                .AsNoTracking()
                .Where(j => j.ClosedAt == null)
                .Where(j => j.ApplicationDeadline == null || j.ApplicationDeadline > now);

            // Each filter is only added when it was asked for, so EF Core sends one WHERE clause
            // containing exactly the conditions in use.
            if (jobType.HasValue)
            {
                query = query.Where(j => j.JobType == jobType.Value);
            }
            if (location.HasValue)
            {
                query = query.Where(j => j.JobLocation == location.Value);
            }
            if (!string.IsNullOrEmpty(search))
            {
                // Contains becomes SQL LIKE '%...%' with the value sent as a parameter (no injection);
                // EF Core escapes %, _ and [ in it. Case-insensitive under SQL Server's default collation.
                query = query.Where(j => j.Title.Contains(search) || j.Description.Contains(search));
            }

            // Two queries: the total for the pager, then only the rows of this page.
            // Id breaks ties between jobs posted at the same moment, so pages never overlap or skip.
            var totalCount = await query.CountAsync();
            var items = await ToResponses(query
                    .OrderByDescending(j => j.PostedAt)
                    .ThenByDescending(j => j.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize))
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Job?> GetJobById(int id)
        {
            return await dbContext.Job
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<JobResponse?> GetJobDetails(int id, bool openOnly)
        {
            var query = dbContext.Job.AsNoTracking().Where(j => j.Id == id);
            if (openOnly)
            {
                query = query.Where(j => j.ClosedAt == null);
            }
            return await ToResponses(query).FirstOrDefaultAsync();
        }

        public async Task<List<MyJobResponse>> GetJobsByEmployer(string employerId, JobStatusFilter status)
        {
            var query = dbContext.Job
                .AsNoTracking()
                .Where(j => j.EmployerId == employerId);

            if (status == JobStatusFilter.Open)
            {
                query = query.Where(j => j.ClosedAt == null);
            }
            else if (status == JobStatusFilter.Closed)
            {
                query = query.Where(j => j.ClosedAt != null);
            }

            // One query: the applicant count is a correlated COUNT subquery, so the applications
            // themselves are never loaded; the skill names come from a join on the link table.
            return await query
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
                    City = j.City,
                    ExperienceLevel = j.ExperienceLevel,
                    ApplicationDeadline = j.ApplicationDeadline,
                    ClosedAt = j.ClosedAt,
                    ApplicationCount = dbContext.JobApplication.Count(a => a.JobId == j.Id),
                    Skills = (from js in dbContext.JobSkill
                              join s in dbContext.Skill on js.SkillId equals s.Id
                              where js.JobId == j.Id
                              orderby s.Name
                              select s.Name).ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateJob(Job job, IReadOnlyCollection<int> skillIds)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                dbContext.ChangeTracker.Clear();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                // One atomic UPDATE. The WHERE re-checks owner and open status at write time, so a job
                // closed (or reassigned) between the handler's read and this write is never overwritten.
                // Only the editable columns are SET: EmployerId, PostedAt and ClosedAt can't change.
                var rows = await dbContext.Job
                    .Where(j => j.Id == job.Id && j.EmployerId == job.EmployerId && j.ClosedAt == null)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(j => j.Title, job.Title)
                        .SetProperty(j => j.Description, job.Description)
                        .SetProperty(j => j.JobType, job.JobType)
                        .SetProperty(j => j.JobLocation, job.JobLocation)
                        .SetProperty(j => j.SalaryRange, job.SalaryRange)
                        .SetProperty(j => j.City, job.City)
                        .SetProperty(j => j.ExperienceLevel, job.ExperienceLevel)
                        .SetProperty(j => j.ApplicationDeadline, job.ApplicationDeadline));
                if (rows == 0)
                {
                    // Leaving without CommitAsync rolls the (empty) transaction back.
                    return false;
                }

                // Full replacement of the skill list: drop the old links, insert the new ones.
                await dbContext.JobSkill.Where(js => js.JobId == job.Id).ExecuteDeleteAsync();
                dbContext.JobSkill.AddRange(skillIds.Select(skillId => new JobSkill { JobId = job.Id, SkillId = skillId }));
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            });
        }

        public async Task<bool> CloseJob(int id, string employerId, DateTimeOffset closedAt)
        {
            var rows = await dbContext.Job
                .Where(j => j.Id == id && j.EmployerId == employerId && j.ClosedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(j => j.ClosedAt, closedAt));
            return rows > 0;
        }

        // Job -> JobResponse inside the query, so each page (or single job) is ONE SQL statement:
        // the company name is a correlated subquery on EmployerProfile (null without a profile) and the
        // skill names a join on the link table. Apply ordering and paging BEFORE calling this.
        private IQueryable<JobResponse> ToResponses(IQueryable<Job> jobs)
        {
            return jobs.Select(j => new JobResponse
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                JobType = j.JobType,
                JobLocation = j.JobLocation,
                SalaryRange = j.SalaryRange,
                EmployerId = j.EmployerId,
                PostedAt = j.PostedAt,
                City = j.City,
                ExperienceLevel = j.ExperienceLevel,
                ApplicationDeadline = j.ApplicationDeadline,
                CompanyName = dbContext.EmployerProfile
                    .Where(p => p.EmployerId == j.EmployerId)
                    .Select(p => p.CompanyName)
                    .FirstOrDefault(),
                Skills = (from js in dbContext.JobSkill
                          join s in dbContext.Skill on js.SkillId equals s.Id
                          where js.JobId == j.Id
                          orderby s.Name
                          select s.Name).ToList()
            });
        }
    }
}
