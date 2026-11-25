using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;

namespace JobBoard.Application.Jobs.ApplyToJob
{
    public class ApplyToJobHandler
    {
        private readonly IJobRepository jobRepository;
        private readonly IJobApplicationRepository jobApplicationRepository;

        public ApplyToJobHandler(IJobRepository jobRepository, IJobApplicationRepository jobApplicationRepository)
        {
            this.jobRepository = jobRepository;
            this.jobApplicationRepository = jobApplicationRepository;
        }

        // jobSeekerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the request. command is the optional body (cover letter); null means no body.
        public async Task<ApplyToJobResult> Handle(int jobId, string jobSeekerId, ApplyToJobCommand? command = null)
        {
            var job = await jobRepository.GetJobById(jobId);
            // A closed job no longer exists as far as applicants are concerned.
            if (job == null || job.ClosedAt != null)
            {
                return ApplyToJobResult.JobNotFound();
            }

            // Every application starts as Applied, and that moment is also its first status timestamp.
            var appliedAt = DateTimeOffset.UtcNow;

            // After the deadline the job still exists (it can be viewed), but takes no new applications.
            if (job.ApplicationDeadline.HasValue && appliedAt >= job.ApplicationDeadline.Value)
            {
                return ApplyToJobResult.DeadlinePassed();
            }

            // Friendly early check for the normal duplicate case.
            if (await jobApplicationRepository.HasApplied(jobId, jobSeekerId))
            {
                return ApplyToJobResult.AlreadyApplied();
            }

            var application = new JobApplication
            {
                JobId = jobId,
                JobSeekerId = jobSeekerId,
                ApplicationDate = appliedAt,
                Status = ApplicationStatus.Applied,
                StatusChangedAt = appliedAt,
                CoverLetter = string.IsNullOrWhiteSpace(command?.CoverLetter) ? null : command.CoverLetter.Trim()
            };

            // Two simultaneous requests can both pass the check above. The unique index in the
            // database is the real guarantee: it rejects the second insert and TryAddApplication
            // reports that as false.
            if (!await jobApplicationRepository.TryAddApplication(application))
            {
                return ApplyToJobResult.AlreadyApplied();
            }

            return ApplyToJobResult.Success(new JobApplicationResponse
            {
                Id = application.Id,
                JobId = application.JobId,
                JobSeekerId = application.JobSeekerId,
                ApplicationDate = application.ApplicationDate,
                CoverLetter = application.CoverLetter
            });
        }
    }
}
