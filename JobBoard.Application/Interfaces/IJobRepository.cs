using System;
using System.Collections.Generic;
using System.Text;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.Interfaces
{
    public interface IJobRepository
    {
        public Task<Job> AddJob(Job job);
        public Task<List<Job>> GetAllJobs();
        public Task<Job?> GetJobById(int id);

        // Writes Title, Description, JobType, JobLocation and SalaryRange of job.Id in one UPDATE that
        // also requires the same EmployerId and an open job. False means no row matched.
        public Task<bool> UpdateJob(Job job);

        // Sets ClosedAt on an open job owned by employerId. False means no open job of theirs matched.
        public Task<bool> CloseJob(int id, string employerId, DateTimeOffset closedAt);
    }
}
