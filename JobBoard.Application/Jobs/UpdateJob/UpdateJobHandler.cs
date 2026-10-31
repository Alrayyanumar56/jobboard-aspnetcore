using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Jobs.UpdateJob
{
    public class UpdateJobHandler
    {
        private readonly IJobRepository jobRepository;

        public UpdateJobHandler(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the request body.
        public async Task<UpdateJobResult> Handle(int jobId, UpdateJobCommand command, string employerId)
        {
            var job = await jobRepository.GetJobById(jobId);

            // OWNERSHIP CHECK. "No such job" and "someone else's job" deliberately give the same
            // result (404), so an employer can't probe which job ids exist or who owns them.
            if (job == null || job.EmployerId != employerId)
            {
                return UpdateJobResult.JobNotFound();
            }

            // A closed job is read-only history.
            if (job.ClosedAt != null)
            {
                return UpdateJobResult.JobClosed();
            }

            // GetJobById is AsNoTracking, so this object is only a carrier of the new values.
            // EmployerId, PostedAt and ClosedAt are never taken from the command.
            job.Title = command.Title.Trim();
            job.Description = command.Description.Trim();
            job.JobType = command.JobType!.Value;
            job.JobLocation = command.JobLocation!.Value;
            job.SalaryRange = string.IsNullOrWhiteSpace(command.SalaryRange) ? null : command.SalaryRange.Trim();

            // The repository's UPDATE re-checks owner and open status. False here means the job was
            // closed by another request after we read it.
            if (!await jobRepository.UpdateJob(job))
            {
                return UpdateJobResult.JobClosed();
            }

            return UpdateJobResult.Success(job.ToResponse());
        }
    }
}
