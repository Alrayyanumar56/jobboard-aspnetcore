using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;

namespace JobBoard.Tests.Fakes
{
    // In-memory IJobApplicationRepository. TryAddApplication enforces the (JobId, JobSeekerId)
    // uniqueness and TryChangeStatus does the same compare-and-swap as the real repository.
    public class FakeJobApplicationRepository : IJobApplicationRepository
    {
        public List<JobApplication> Applications { get; } = new();
        public List<JobApplicationStatusHistory> History { get; } = new();

        public Task<bool> HasApplied(int jobId, string jobSeekerId) =>
            Task.FromResult(Applications.Any(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId));

        public Task<bool> TryAddApplication(JobApplication application)
        {
            if (Applications.Any(a => a.JobId == application.JobId && a.JobSeekerId == application.JobSeekerId))
            {
                return Task.FromResult(false);
            }
            application.Id = Applications.Count == 0 ? 1 : Applications.Max(a => a.Id) + 1;
            Applications.Add(application);
            return Task.FromResult(true);
        }

        public Task<List<MyApplicationResponse>> GetApplicationsByJobSeeker(string jobSeekerId) =>
            Task.FromResult(Applications
                .Where(a => a.JobSeekerId == jobSeekerId)
                .Select(a => new MyApplicationResponse
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    ApplicationDate = a.ApplicationDate,
                    Status = a.Status,
                    StatusChangedAt = a.StatusChangedAt
                })
                .ToList());

        public Task<List<JobApplicantResponse>> GetApplicantsByJob(int jobId) =>
            Task.FromResult(Applications
                .Where(a => a.JobId == jobId)
                .Select(a => new JobApplicantResponse
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    ApplicantEmail = a.JobSeekerId + "@example.com",
                    ApplicationDate = a.ApplicationDate,
                    Status = a.Status,
                    StatusChangedAt = a.StatusChangedAt
                })
                .ToList());

        public Task<JobApplication?> GetApplicationById(int applicationId)
        {
            var a = Applications.FirstOrDefault(x => x.Id == applicationId);
            JobApplication? copy = a == null ? null : new JobApplication
            {
                Id = a.Id,
                JobId = a.JobId,
                JobSeekerId = a.JobSeekerId,
                ApplicationDate = a.ApplicationDate,
                Status = a.Status,
                StatusChangedAt = a.StatusChangedAt,
                CoverLetter = a.CoverLetter
            };
            return Task.FromResult(copy);
        }

        public Task<bool> TryChangeStatus(int applicationId, ApplicationStatus from, ApplicationStatus to, string changedByUserId, DateTimeOffset changedAt)
        {
            var stored = Applications.FirstOrDefault(a => a.Id == applicationId && a.Status == from);
            if (stored == null)
            {
                return Task.FromResult(false);
            }
            stored.Status = to;
            stored.StatusChangedAt = changedAt;
            History.Add(new JobApplicationStatusHistory
            {
                JobApplicationId = applicationId,
                FromStatus = from,
                ToStatus = to,
                ChangedAt = changedAt,
                ChangedByUserId = changedByUserId
            });
            return Task.FromResult(true);
        }

        public Task<List<ApplicationStatusHistoryResponse>> GetStatusHistory(int applicationId) =>
            Task.FromResult(History
                .Where(h => h.JobApplicationId == applicationId)
                .Select(h => new ApplicationStatusHistoryResponse
                {
                    FromStatus = h.FromStatus,
                    ToStatus = h.ToStatus,
                    ChangedAt = h.ChangedAt,
                    ChangedByUserId = h.ChangedByUserId
                })
                .ToList());
    }
}
