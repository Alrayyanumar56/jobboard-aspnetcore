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
        // Saves the job and links it to skillIds, together or not at all. Sets job.Id.
        public Task<Job> AddJob(Job job, IReadOnlyCollection<int> skillIds);

        // One page of listable jobs, newest first, plus the total number of matching listable jobs.
        // Listable = open and with no deadline or a deadline after `now`.
        // A null filter means "any"; search matches Title or Description. page starts at 1.
        public Task<(List<JobResponse> Items, int TotalCount)> GetOpenJobsPage(JobType? jobType, Location? location, string? search, int page, int pageSize, DateTimeOffset now);

        // The entity, for ownership and state checks. Closed jobs included.
        public Task<Job?> GetJobById(int id);

        // The full public shape (company name, skills). openOnly = false also returns a closed job.
        // Null when there is no such job (or it is closed and openOnly is true).
        public Task<JobResponse?> GetJobDetails(int id, bool openOnly);

        // The jobs this employer posted (open, closed or both), newest first, each with its applicant count.
        public Task<List<MyJobResponse>> GetJobsByEmployer(string employerId, JobStatusFilter status);

        // Writes the editable columns of job.Id in one UPDATE that also requires the same EmployerId and
        // an open job, and replaces the job's skill links with skillIds, together or not at all.
        // False means no row matched (and nothing was written).
        public Task<bool> UpdateJob(Job job, IReadOnlyCollection<int> skillIds);

        // Sets ClosedAt on an open job owned by employerId. False means no open job of theirs matched.
        public Task<bool> CloseJob(int id, string employerId, DateTimeOffset closedAt);
    }
}
