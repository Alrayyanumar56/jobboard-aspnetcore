using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Enums;
using JobBoard.Domain.Rules;

namespace JobBoard.Application.Jobs.UpdateApplicationStatus
{
    public class UpdateApplicationStatusHandler
    {
        private readonly IJobApplicationRepository jobApplicationRepository;
        private readonly IJobRepository jobRepository;

        public UpdateApplicationStatusHandler(IJobApplicationRepository jobApplicationRepository, IJobRepository jobRepository)
        {
            this.jobApplicationRepository = jobApplicationRepository;
            this.jobRepository = jobRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the request body.
        public async Task<ChangeApplicationStatusResult> Handle(int applicationId, UpdateApplicationStatusCommand command, string employerId)
        {
            var application = await jobApplicationRepository.GetApplicationById(applicationId);
            if (application == null)
            {
                return ChangeApplicationStatusResult.NotFound();
            }

            // OWNERSHIP CHECK. The application belongs to a job; that job must belong to this employer.
            // A missing application, a missing job and someone else's job all give the same result
            // (404), so an employer can't probe which application ids exist or whose they are.
            // A CLOSED job is still the employer's job, so its applications can still be managed.
            var job = await jobRepository.GetJobById(application.JobId);
            if (job == null || job.EmployerId != employerId)
            {
                return ChangeApplicationStatusResult.NotFound();
            }

            var from = application.Status;
            var to = command.Status!.Value;

            if (!ApplicationStatusRules.CanEmployerMove(from, to))
            {
                return ChangeApplicationStatusResult.InvalidTransition(DescribeInvalidMove(from, to));
            }

            var changedAt = DateTimeOffset.UtcNow;

            // The repository only applies the change if the application is STILL in `from`.
            // False means another request moved it first; our validation was based on stale data.
            if (!await jobApplicationRepository.TryChangeStatus(application.Id, from, to, employerId, changedAt))
            {
                return ChangeApplicationStatusResult.InvalidTransition(
                    "The application's status changed while this request was being processed. Reload it and try again.");
            }

            return ChangeApplicationStatusResult.Success(new ApplicationStatusResponse
            {
                Id = application.Id,
                JobId = application.JobId,
                Status = to,
                StatusChangedAt = changedAt
            });
        }

        private static string DescribeInvalidMove(ApplicationStatus from, ApplicationStatus to)
        {
            var allowed = ApplicationStatusRules.EmployerNextStatuses(from);
            return allowed.Count == 0
                ? $"Cannot change an application from {from} to {to}: {from} is a final status."
                : $"Cannot change an application from {from} to {to}. Allowed next statuses: {string.Join(", ", allowed)}.";
        }
    }
}
