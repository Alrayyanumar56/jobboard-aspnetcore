using System;
using System.Collections.Generic;
using System.Text;
using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;

namespace JobBoard.Application.Interfaces
{
    public interface IJobRepository
    {
        public Task<Job> AddJob(Job job);
        // Open jobs only, newest first. A null filter means "any".
        public Task<List<Job>> GetAllJobs(JobType? jobType, Location? location);
        public Task<Job?> GetJobById(int id);

        // Every job this employer posted, open AND closed, newest first, each with its applicant count.
        public Task<List<MyJobResponse>> GetJobsByEmployer(string employerId);

        // Writes Title, Description, JobType, JobLocation and SalaryRange of job.Id in one UPDATE that
        // also requires the same EmployerId and an open job. False means no row matched.
        public Task<bool> UpdateJob(Job job);

        // Sets ClosedAt on an open job owned by employerId. False means no open job of theirs matched.
        public Task<bool> CloseJob(int id, string employerId, DateTimeOffset closedAt);
    }
}
