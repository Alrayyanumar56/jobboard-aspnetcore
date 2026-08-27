# 04 — Project Structure Guide

Reference this whenever I forget "wait, which project/folder does this go in" — same purpose as before, updated for four projects instead of one.

## JobBoard.Domain
- `Entities/` — `Job.cs`, `JobApplication.cs`. Plain classes representing what exists, no EF Core attributes, no framework dependencies.
- `Enums/` — `JobType.cs`, `Location.cs` (pending the overlap question in `02-domain-model.md`).
- Nothing in here should ever need a `using` for EF Core, ASP.NET Core, or Identity. If it does, that class belongs somewhere else.

## JobBoard.Application
- `Interfaces/` — `IJobRepository`, `IJobService`, etc. Contracts, not implementations.
- `DTOs/` — request/response shapes for the API (e.g. `JobCreateRequest`, `JobResponse`). Same over-posting reasoning as the old ViewModels: never bind the raw entity directly to an incoming request.
- `Services/` (or `UseCases/`) — business logic, ownership checks, orchestration. This is where "is this user allowed to edit this job" actually gets decided.

## JobBoard.Infrastructure
- `Data/` — `ApplicationDbContext`, EF Core migrations.
- `Repositories/` — concrete implementations of Application's interfaces (e.g. `JobRepository : IJobRepository`).
- `Identity/` — `ApplicationUser`, Identity configuration if it needs to live outside Api.

## JobBoard.Api
- `Controllers/` — `JobsController`, `ApplicationsController`, `AccountController` (or Identity-scaffolded auth endpoints). Thin: receive request → call Application interface → return `Ok`/`CreatedAtAction`/`NotFound`/etc. No business logic, no direct DbContext calls.
- `Program.cs` — DI wiring (registering interfaces to implementations across projects), Swagger setup, middleware pipeline.
- No `/Views`, no `wwwroot` for pages — this project only returns data and hosts Swagger's UI.

## A note on file-per-concept discipline (unchanged principle)
Before creating any file, I should be able to answer: "which project, which folder, and why." If a controller wants to touch `ApplicationDbContext` directly, that's the signal something belongs in Infrastructure/Application instead — the compiler will actually stop me here if project references are set up correctly, which is one of the real benefits of this structure.
