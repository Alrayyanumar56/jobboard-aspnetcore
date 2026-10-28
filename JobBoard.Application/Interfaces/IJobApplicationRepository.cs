using JobBoard.Domain.Entities;

namespace JobBoard.Application.Interfaces
{
    public interface IJobApplicationRepository
    {
        public Task<bool> HasApplied(int jobId, string jobSeekerId);

        // Returns false (instead of throwing) when the unique (JobId, JobSeekerId) index rejects a duplicate.
        public Task<bool> TryAddApplication(JobApplication application);
    }
}
