using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Enums;
using JobBoard.Domain.Rules;

namespace JobBoard.Application.Jobs.WithdrawApplication
{
    public class WithdrawApplicationHandler
    {
        private readonly IJobApplicationRepository jobApplicationRepository;

        public WithdrawApplicationHandler(IJobApplicationRepository jobApplicationRepository)
        {
            this.jobApplicationRepository = jobApplicationRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the request. Withdrawing is the ONLY status change a JobSeeker can make.
        public async Task<ChangeApplicationStatusResult> Handle(int applicationId, string jobSeekerId)
        {
            var application = await jobApplicationRepository.GetApplicationById(applicationId);

            // OWNERSHIP CHECK. "No such application" and "someone else's application" give the same
            // result (404), so a JobSeeker can't probe which application ids exist.
            if (application == null || application.JobSeekerId != jobSeekerId)
            {
                return ChangeApplicationStatusResult.NotFound();
            }

            var from = application.Status;
            if (!ApplicationStatusRules.CanWithdraw(from))
            {
                return ChangeApplicationStatusResult.InvalidTransition(
                    $"Cannot withdraw an application that is {from}: it has already ended.");
            }

            var changedAt = DateTimeOffset.UtcNow;

            // Only applied if the application is STILL in `from`; false means another request moved it first.
            if (!await jobApplicationRepository.TryChangeStatus(application.Id, from, ApplicationStatus.Withdrawn, jobSeekerId, changedAt))
            {
                return ChangeApplicationStatusResult.InvalidTransition(
                    "The application's status changed while this request was being processed. Reload it and try again.");
            }

            return ChangeApplicationStatusResult.Success(new ApplicationStatusResponse
            {
                Id = application.Id,
                JobId = application.JobId,
                Status = ApplicationStatus.Withdrawn,
                StatusChangedAt = changedAt
            });
        }
    }
}
