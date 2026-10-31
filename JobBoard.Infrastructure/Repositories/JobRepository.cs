using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
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
        public async Task<Job> AddJob(Job job)
        {
            dbContext.Add(job);
            await dbContext.SaveChangesAsync();
            return job;
        }
        public async Task<List<Job>> GetAllJobs()
        {
            // Public listing: closed jobs are not shown.
            return await dbContext.Job
                .AsNoTracking()
                .Where(j => j.ClosedAt == null)
                .OrderByDescending(j => j.PostedAt)
                .ToListAsync();
        }
        public async Task<Job?> GetJobById(int id)
        {
            return await dbContext.Job
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<bool> UpdateJob(Job job)
        {
            // One atomic UPDATE. The WHERE re-checks owner and open status at write time, so a job
            // closed (or reassigned) between the handler's read and this write is never overwritten.
            // Only the five editable columns are SET: EmployerId, PostedAt and ClosedAt can't change.
            var rows = await dbContext.Job
                .Where(j => j.Id == job.Id && j.EmployerId == job.EmployerId && j.ClosedAt == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(j => j.Title, job.Title)
                    .SetProperty(j => j.Description, job.Description)
                    .SetProperty(j => j.JobType, job.JobType)
                    .SetProperty(j => j.JobLocation, job.JobLocation)
                    .SetProperty(j => j.SalaryRange, job.SalaryRange));
            return rows > 0;
        }

        public async Task<bool> CloseJob(int id, string employerId, DateTimeOffset closedAt)
        {
            var rows = await dbContext.Job
                .Where(j => j.Id == id && j.EmployerId == employerId && j.ClosedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(j => j.ClosedAt, closedAt));
            return rows > 0;
        }
    }
}
