using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;

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
    }
}
