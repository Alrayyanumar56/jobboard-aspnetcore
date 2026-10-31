using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Jobs.CloseJob
{
    public class CloseJobHandler
    {
        private readonly IJobRepository jobRepository;

        public CloseJobHandler(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the request. Returns false when the caller may not close this job; the
        // controller turns that into a 404. Closing never deletes the job or its applications.
        public async Task<bool> Handle(int jobId, string employerId)
        {
            var job = await jobRepository.GetJobById(jobId);

            // OWNERSHIP CHECK. Missing job and someone else's job both give false (-> 404).
            if (job == null || job.EmployerId != employerId)
            {
                return false;
            }

            // Closing an already-closed job is a no-op that still succeeds (idempotent).
            if (job.ClosedAt != null)
            {
                return true;
            }

            // The repository's UPDATE also checks owner and open status. If it matches nothing,
            // another request closed the job first, so the job is closed either way.
            await jobRepository.CloseJob(jobId, employerId, DateTimeOffset.UtcNow);
            return true;
        }
    }
}
