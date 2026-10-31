using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Jobs.GetJobApplicants
{
    public class GetJobApplicantsHandler
    {
        private readonly IJobRepository jobRepository;
        private readonly IJobApplicationRepository jobApplicationRepository;

        public GetJobApplicantsHandler(IJobRepository jobRepository, IJobApplicationRepository jobApplicationRepository)
        {
            this.jobRepository = jobRepository;
            this.jobApplicationRepository = jobApplicationRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the request. Returns null when the caller may not see this job's applicants;
        // the controller turns that into a 404.
        public async Task<List<JobApplicantResponse>?> Handle(int jobId, string employerId)
        {
            var job = await jobRepository.GetJobById(jobId);

            // OWNERSHIP CHECK. "No such job" and "someone else's job" deliberately give the same
            // result (null -> 404), so an employer can't use this endpoint to discover which job
            // ids exist or who owns them.
            if (job == null || job.EmployerId != employerId)
            {
                return null;
            }

            // The employer owns the job. No applicants gives an empty list, not null.
            return await jobApplicationRepository.GetApplicantsByJob(jobId);
        }
    }
}
