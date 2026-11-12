using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Jobs.GetApplicationHistory
{
    public class GetApplicationHistoryHandler
    {
        private readonly IJobApplicationRepository jobApplicationRepository;
        private readonly IJobRepository jobRepository;

        public GetApplicationHistoryHandler(IJobApplicationRepository jobApplicationRepository, IJobRepository jobRepository)
        {
            this.jobApplicationRepository = jobApplicationRepository;
            this.jobRepository = jobRepository;
        }

        // userId and isEmployer come from the authenticated user's JWT (resolved in the controller),
        // never from the request. Returns null when the caller may not see this application's history;
        // the controller turns that into a 404.
        public async Task<List<ApplicationStatusHistoryResponse>?> Handle(int applicationId, string userId, bool isEmployer)
        {
            var application = await jobApplicationRepository.GetApplicationById(applicationId);
            if (application == null)
            {
                return null;
            }

            // OWNERSHIP CHECK, by role. "No such application" and "not yours" deliberately give the same
            // result (null -> 404), so nobody can probe which application ids exist.
            if (isEmployer)
            {
                // The application's job must belong to this employer. A closed job still counts.
                var job = await jobRepository.GetJobById(application.JobId);
                if (job == null || job.EmployerId != userId)
                {
                    return null;
                }
            }
            else if (application.JobSeekerId != userId)
            {
                // A JobSeeker may only see the history of their own application.
                return null;
            }

            // Oldest first. An application that never changed status has no rows: an empty list, not null.
            return await jobApplicationRepository.GetStatusHistory(applicationId);
        }
    }
}
