# 04 — Project Structure Guide

Reference this whenever I forget "wait, which project/folder does this go in". Updated 2026-11-22 to match the actual code.

## JobBoard.Domain
- `Entities/` — `Job`, `JobApplication`, `JobApplicationStatusHistory`, `CandidateProfile`, `CandidateEducation`. Plain classes representing what exists.
- `Enums/` — `JobType`, `Location`, `ApplicationStatus`, `UserRole`.
- `Rules/` — `ApplicationStatusRules`: which status changes are allowed. Pure logic, unit-tested.
- Nothing in here should ever need a `using` for EF Core, ASP.NET Core, or Identity. If it does, that class belongs somewhere else.

## JobBoard.Application
- `Interfaces/` — repository and service contracts (`IJobRepository`, `IJobApplicationRepository`, `IIdentityService`, ...). Contracts, not implementations.
- `DTOs/` — response shapes and auth requests (`JobResponse`, `MyJobResponse`, `RegisterRequest`, `RegistrationResult`, ...). Never bind a raw entity to an incoming request.
- Feature folders, one folder per use case, each with its handler (and command/result when it has one):
  - `Jobs/` — `CreateJob`, `GetJobs`, `GetJobById`, `GetMyJobs`, `UpdateJob`, `CloseJob`, `ApplyToJob`, `GetMyApplications`, `GetJobApplicants`, `UpdateApplicationStatus`, `WithdrawApplication`, `GetApplicationHistory`
  - `Profiles/` — candidate profile use cases
  - `Education/` — candidate education use cases
- `Services/` — `RegistrationService`, `LoginService`.
- `DependencyInjection.cs` — `AddApplication()`: **register every new handler here.**
- Ownership checks ("is this user allowed to touch this job/application") are decided in the handlers.
- No reference to ASP.NET Core or Identity (only `Microsoft.Extensions.DependencyInjection.Abstractions` for `AddApplication`).

## JobBoard.Infrastructure
- `Data/` — `ApplicationDbContext`, `ApplicationUser`, `RoleInitializer`.
- `Migrations/` — EF Core migrations.
- `Repositories/` — EF Core implementations of Application's repository interfaces.
- `Services/` — `IdentityService` (UserManager), `JWTService` (token creation).
- `DependencyInjection.cs` — `AddInfrastructure(configuration)`: DbContext (with retry), Identity, repositories, services.

## JobBoard.Api
- `Controllers/` — `AuthController`, `JobController`, `JobApplicationController`, `CandidateProfileController`, `CandidateEducationController`. Thin: read the user id from the token → call a handler → map the result to `Ok`/`NotFound`/`Problem`. No business logic, no DbContext.
- `Program.cs` — calls `AddApplication()` / `AddInfrastructure()`, configures JWT, CORS, JSON, Swagger, ProblemDetails, health check and the middleware pipeline.
- `JobBoard.Api.http` — ready-made requests for every flow.

## JobBoard.Tests
- xUnit tests for Domain rules and Application handlers.
- `Fakes/` — in-memory repositories that follow the same contract as the EF Core ones, so tests need no database.

## A note on file-per-concept discipline (unchanged principle)
Before creating any file, I should be able to answer: "which project, which folder, and why." If a controller wants to touch `ApplicationDbContext` directly, that's the signal something belongs in Infrastructure/Application instead — the compiler will actually stop me here if project references are set up correctly, which is one of the real benefits of this structure.
