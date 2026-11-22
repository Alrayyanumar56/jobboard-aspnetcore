using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Globalization;
using System.Threading.RateLimiting;

namespace JobBoard.Api.RateLimiting
{
    // Rate limiting is an HTTP concern, so it lives in the Api project (built into ASP.NET Core,
    // no extra package). Endpoints opt in with [EnableRateLimiting(RateLimitPolicies.Auth)].
    public static class RateLimitPolicies
    {
        // Login, register and change-password: the endpoints worth guessing passwords against.
        public const string Auth = "auth";

        private const int AuthPermitsPerWindow = 10;
        private static readonly TimeSpan AuthWindow = TimeSpan.FromMinutes(1);

        public static IServiceCollection AddJobBoardRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Each client IP gets its own budget of 10 requests per minute on the auth endpoints,
                // shared across them. No queue: request 11 is refused at once.
                // On Azure App Service, RemoteIpAddress may be a front-end address unless forwarded
                // headers are processed; then the limit applies to more callers at once (stricter, not looser).
                options.AddPolicy(Auth, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = AuthPermitsPerWindow,
                            Window = AuthWindow,
                            QueueLimit = 0
                        }));

                // Refused requests get 429 with a Retry-After header and the same ProblemDetails body
                // as every other error.
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
                    }

                    var problemDetailsService = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                    await problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context.HttpContext,
                        ProblemDetails = new ProblemDetails
                        {
                            Status = StatusCodes.Status429TooManyRequests,
                            Title = "Too many requests",
                            Detail = "Too many attempts. Please wait a minute and try again."
                        }
                    });
                };
            });

            return services;
        }
    }
}
