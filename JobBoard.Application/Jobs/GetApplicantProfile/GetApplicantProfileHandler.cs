using JobBoard.Application.DTOs;
using JobBoard.Application.Education;
using JobBoard.Application.Interfaces;
using JobBoard.Application.Profiles;

namespace JobBoard.Application.Jobs.GetApplicantProfile
{
    public class GetApplicantProfileHandler
    {
        private readonly IJobApplicationRepository jobApplicationRepository;
        private readonly IJobRepository jobRepository;
        private readonly ICandidateProfileRepository candidateProfileRepository;
        private readonly ICandidateEducationRepository candidateEducationRepository;

        public GetApplicantProfileHandler(
            IJobApplicationRepository jobApplicationRepository,
            IJobRepository jobRepository,
            ICandidateProfileRepository candidateProfileRepository,
            ICandidateEducationRepository candidateEducationRepository)
        {
            this.jobApplicationRepository = jobApplicationRepository;
            this.jobRepository = jobRepository;
            this.candidateProfileRepository = candidateProfileRepository;
            this.candidateEducationRepository = candidateEducationRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request. Returns null when the caller may not see this applicant; the controller turns
        // that into a 404.
        public async Task<ApplicantProfileResponse?> Handle(int applicationId, string employerId)
        {
            var application = await jobApplicationRepository.GetApplicationById(applicationId);
            if (application == null)
            {
                return null;
            }

            // OWNERSHIP CHECK. The application's job must belong to this employer. "No such
            // application" and "an application on someone else's job" deliberately give the same
            // result (null -> 404), so nobody can probe which application ids exist.
            // A closed job still counts, and so does a withdrawn application: the employer keeps
            // seeing who applied to their job.
            var job = await jobRepository.GetJobById(application.JobId);
            if (job == null || job.EmployerId != employerId)
            {
                return null;
            }

            // The seeker is identified by the APPLICATION (server data), never by anything in the request.
            var profile = await candidateProfileRepository.GetByJobSeekerId(application.JobSeekerId);
            var education = await candidateEducationRepository.GetAllForJobSeeker(application.JobSeekerId);

            return new ApplicantProfileResponse
            {
                ApplicationId = application.Id,
                Profile = profile?.ToResponse(),
                Education = education.Select(e => e.ToResponse()).ToList()
            };
        }
    }
}
