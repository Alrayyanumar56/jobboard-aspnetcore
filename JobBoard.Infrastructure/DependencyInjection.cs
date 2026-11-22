using JobBoard.Application.Interfaces;
using JobBoard.Infrastructure.Data;
using JobBoard.Infrastructure.Repositories;
using JobBoard.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobBoard.Infrastructure
{
    // Everything the Infrastructure layer contributes to the DI container: the database, Identity,
    // repositories and the services that implement Application interfaces.
    // Program.cs calls builder.Services.AddInfrastructure(builder.Configuration).
    public static class DependencyInjection
    {
        public const string ConnectionStringName = "JobBoardDBConnection";

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Fail at startup with a message that says exactly what is missing, instead of a vague
            // error on the first database call (or an HTTP 500.30 on Azure with no explanation).
            var connectionString = configuration.GetConnectionString(ConnectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Missing connection string '{ConnectionStringName}'. Set it in User Secrets locally, " +
                    $"or as the App Service setting 'ConnectionStrings__{ConnectionStringName}' on Azure.");
            }

            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, sql =>
                    // Azure SQL drops connections now and then, and the free serverless database
                    // auto-pauses: the first connection after a pause fails while it wakes up.
                    // Retrying transient errors turns those into a short delay instead of a crash.
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 6,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Repositories
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<ICandidateProfileRepository, CandidateProfileRepository>();
            services.AddScoped<ICandidateEducationRepository, CandidateEducationRepository>();

            // Implementations of Application interfaces
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IJWTService, JWTService>();

            services.AddScoped<RoleInitializer>();

            return services;
        }
    }
}
