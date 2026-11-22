using JobBoard.Application.Education.CreateCandidateEducation;
using JobBoard.Application.Education.DeleteCandidateEducation;
using JobBoard.Application.Education.GetMyCandidateEducation;
using JobBoard.Application.Education.UpdateCandidateEducation;
using JobBoard.Application.Interfaces;
using JobBoard.Application.Jobs.ApplyToJob;
using JobBoard.Application.Jobs.CloseJob;
using JobBoard.Application.Jobs.CreateJob;
using JobBoard.Application.Jobs.GetApplicationHistory;
using JobBoard.Application.Jobs.GetJobApplicants;
using JobBoard.Application.Jobs.GetJobById;
using JobBoard.Application.Jobs.GetJobs;
using JobBoard.Application.Jobs.GetMyApplications;
using JobBoard.Application.Jobs.GetMyJobs;
using JobBoard.Application.Jobs.UpdateApplicationStatus;
using JobBoard.Application.Jobs.UpdateJob;
using JobBoard.Application.Jobs.WithdrawApplication;
using JobBoard.Application.Profiles.CreateCandidateProfile;
using JobBoard.Application.Profiles.GetMyCandidateProfile;
using JobBoard.Application.Profiles.UpdateCandidateProfile;
using JobBoard.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobBoard.Application
{
    // Everything the Application layer contributes to the DI container, in one place.
    // Program.cs calls builder.Services.AddApplication() instead of listing every handler itself.
    // A new handler is registered HERE, next to the other handlers of its layer.
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Auth use cases
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<ILoginService, LoginService>();

            // Jobs
            services.AddScoped<CreateJobHandler>();
            services.AddScoped<GetJobsHandler>();
            services.AddScoped<GetJobByIdHandler>();
            services.AddScoped<GetMyJobsHandler>();
            services.AddScoped<UpdateJobHandler>();
            services.AddScoped<CloseJobHandler>();

            // Job applications
            services.AddScoped<ApplyToJobHandler>();
            services.AddScoped<GetMyApplicationsHandler>();
            services.AddScoped<GetJobApplicantsHandler>();
            services.AddScoped<UpdateApplicationStatusHandler>();
            services.AddScoped<WithdrawApplicationHandler>();
            services.AddScoped<GetApplicationHistoryHandler>();

            // Candidate profile
            services.AddScoped<GetMyCandidateProfileHandler>();
            services.AddScoped<CreateCandidateProfileHandler>();
            services.AddScoped<UpdateCandidateProfileHandler>();

            // Candidate education
            services.AddScoped<CreateCandidateEducationHandler>();
            services.AddScoped<GetMyCandidateEducationHandler>();
            services.AddScoped<UpdateCandidateEducationHandler>();
            services.AddScoped<DeleteCandidateEducationHandler>();

            return services;
        }
    }
}
