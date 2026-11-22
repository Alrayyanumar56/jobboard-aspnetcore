using System;
using System.Collections.Generic;
using System.Text;
using JobBoard.Application.DTOs;
using JobBoard.Application.Jobs.GetMyJobs;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;

namespace JobBoard.Application.Interfaces
{
    public interface IJobRepository
    {
        public Task<Job> AddJob(Job job);
        // One page of open jobs, newest first, plus the total number of matching open jobs.
        // A null filter means "any"; search matches Title or Description. page starts at 1.
        public Task<(List<Job> Items, int TotalCount)> GetOpenJobsPage(JobType? jobType, Location? location, string? search, int page, int pageSize);
        public Task<Job?> GetJobById(int id);

        // The jobs this employer posted (open, closed or both), newest first, each with its applicant count.
        public Task<List<MyJobResponse>> GetJobsByEmployer(string employerId, JobStatusFilter status);

        // Writes Title, Description, JobType, JobLocation and SalaryRange of job.Id in one UPDATE that
        // also requires the same EmployerId and an open job. False means no row matched.
        public Task<bool> UpdateJob(Job job);

        // Sets ClosedAt on an open job owned by employerId. False means no open job of theirs matched.
        public Task<bool> CloseJob(int id, string employerId, DateTimeOffset closedAt);
    }
}
