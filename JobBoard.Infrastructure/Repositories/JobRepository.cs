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
            return await dbContext.Job
                .AsNoTracking()
                .OrderByDescending(j => j.PostedAt)
                .ToListAsync();
        }
        public async Task<Job?> GetJobById(int id)
        {
            return await dbContext.Job
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);
        }
    }
}
