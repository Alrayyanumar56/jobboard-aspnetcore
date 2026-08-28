using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext dbContext;

        public JobRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Job AddJob(Job job)
        {
            dbContext.Add(job);
            dbContext.SaveChanges();
            return job;
        }
    }
}
