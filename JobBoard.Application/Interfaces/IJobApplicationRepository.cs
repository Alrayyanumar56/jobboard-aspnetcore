using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;

namespace JobBoard.Application.Interfaces
{
    public interface IJobApplicationRepository
    {
        public Task<bool> HasApplied(int jobId, string jobSeekerId);

        // Returns false (instead of throwing) when the unique (JobId, JobSeekerId) index rejects a duplicate.
        public Task<bool> TryAddApplication(JobApplication application);
        public Task<List<MyApplicationResponse>> GetApplicationsByJobSeeker(string jobSeekerId);

        // Applicants for one job (newest first). Does NOT check who owns the job: the handler does that.
        public Task<List<JobApplicantResponse>> GetApplicantsByJob(int jobId);

        // One application, or null. Does NOT check who owns it: the handler does that.
        public Task<JobApplication?> GetApplicationById(int applicationId);

        // Moves the application from `from` to `to` and records one history row, together or not at all.
        // Returns false (and writes nothing) when the application is no longer in `from`, meaning
        // another request changed it after the handler read it.
        public Task<bool> TryChangeStatus(int applicationId, ApplicationStatus from, ApplicationStatus to, string changedByUserId, DateTimeOffset changedAt);

        // Status changes of one application, oldest first. Empty when it never changed status.
        // Does NOT check who owns the application: the handler does that.
        public Task<List<ApplicationStatusHistoryResponse>> GetStatusHistory(int applicationId);
    }
}
