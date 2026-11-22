# 05 — Decision Log

Every time I make a real architectural or design choice, log it here in 3-4 lines. This is what lets me explain my reasoning months later in an interview, instead of just saying "AI suggested it" or "I don't remember why."

## Format
```
### [Date] — Decision title
**Context:** what problem or question prompted this
**Decision:** what I chose
**Why:** the reasoning, including what I considered and rejected
**Status:** Active / Superseded by [link to later entry]
```

## Entries

### [2026-08-22] — Pivot from MVC/Razor to ASP.NET Core Web API
**Context:** The project was originally planned with Razor Views, but my goal is to focus on backend development and build APIs consumed by a React frontend.
**Decision:** Convert JobBoard from an MVC/Razor application into an ASP.NET Core Web API, using Swagger/OpenAPI for API documentation and testing.
**Why:** This better matches my goal of becoming a backend developer and gives me practical experience building REST APIs.
**Status:** Active

### [2026-08-22] — Defer Clean Architecture / Multi-Project Structure
**Context:** I considered splitting JobBoard into separate Domain, Application, Infrastructure, and API projects.
**Decision:** Do not adopt the full multi-project Clean Architecture structure yet. Start with a simpler structure and reconsider the split after Phase 1.
**Why:** Clean Architecture introduces additional concepts and complexity that aren't necessary to learn the API fundamentals first. I want to understand the boundaries before introducing more architectural layers.
**Status:** Superseded by 2026-08-23 entry below.

### [2026-08-23] — Reverse the deferral: adopt Clean Architecture from the start
**Context:** After finishing the Identity portion of the playlist, I reconsidered the deferral decision above. I recognized a personal pattern: when I defer an advanced structure "until later," it tends to mean I never do it, because by the time "later" arrives I've already built things in the simpler shape and rebuilding feels like wasted work.
**Decision:** Set up JobBoard as four projects (Domain, Application, Infrastructure, Api) from the very start, before writing feature code.
**Why:** I have almost nothing built yet, so the cost of the more complex structure is low right now and will only grow. I'd rather learn project boundaries deliberately on a near-empty project than retrofit them onto a working one later.
**Status:** Active

### [12-Oct-2026] — Shift to AI-assisted implementation
**Context:** Hand-writing every line (DTOs, repository methods, controllers) was slowing progress, and I can now trace the full request flow myself.
**Decision:** AI may generate implementation code in chat (not as an autonomous agent). I review and trace each feature, then test success and failure cases.
**Why:** Faster delivery, with my learning shifting from typing code to reviewing, debugging and testing it.
**Exception:** Ownership/authorization checks and their negative tests are written or verified by hand.
**Status:** Active

### [2026-11-14] — Employer/JobSeeker: roles vs. separate tables
**Context:** Needed to decide whether Employer/JobSeeker are separate tables or roles on one ApplicationUser.
**Decision:** One `ApplicationUser` (Identity) table with Identity roles for Employer/JobSeeker. Role-specific data lives in its own table keyed by the user id — `CandidateProfile` (one per JobSeeker, unique index) with `CandidateEducation` under it.
**Why:** Identity's role system already handles authorization (`[Authorize(Roles = ...)]`, role claim in the JWT). Keeping profile data out of the Identity table means login data and profile data change independently, and a user without a profile is still a valid user.
**Status:** Active

### [2026-11-22] — Each layer registers its own services
**Context:** `Program.cs` listed ~30 `AddScoped` calls, growing by about four lines per feature.
**Decision:** `AddApplication()` in `JobBoard.Application/DependencyInjection.cs` and `AddInfrastructure(configuration)` in `JobBoard.Infrastructure/DependencyInjection.cs`. `Program.cs` calls the two.
**Why:** A new handler is registered next to its own layer, and `Program.cs` only shows the HTTP pipeline. Standard Clean Architecture layout.
**Status:** Active

### [2026-11-22] — Keep ASP.NET Core Identity out of the Application layer
**Context:** `IRegistrationService` returned `IdentityResult`, which forced `JobBoard.Application` to reference the whole ASP.NET Core framework.
**Decision:** Application returns its own `RegistrationResult`; only Infrastructure (`IdentityService`) knows about Identity. The `Microsoft.AspNetCore.App` framework reference moved to Infrastructure.
**Why:** The dependency rule — Application must not depend on a framework/infrastructure concern. Now the compiler stops Identity types leaking into Application.
**Status:** Active

### [2026-11-22] — All enums serialized as strings
**Context:** `ApplicationStatus` was sent as text but `JobType`, `Location` and `UserRole` as numbers.
**Decision:** Register `JsonStringEnumConverter` globally in `Program.cs`. Numbers are still accepted on input; database storage is unchanged (still int).
**Why:** A frontend (and Swagger) reads `"Remote"` without a lookup table, and a renumbered enum can't silently change meaning in the client.
**Status:** Active

### [2026-11-22] — Fail fast on configuration, retry the database
**Context:** On Azure the API failed with HTTP 500.30, which hides the real exception. A missing `Jwt:SecretKey` made `Convert.FromBase64String(null)` throw; the free Azure SQL database also auto-pauses and rejects the first connection while waking.
**Decision:** Validate `Jwt:*` settings and the connection string at startup with messages naming the setting and its App Service name. Enable `EnableRetryOnFailure`; the one explicit transaction (status change + history) runs inside `CreateExecutionStrategy().ExecuteAsync(...)`, which a retrying strategy requires. Role seeding failures are logged, not fatal.
**Why:** A configuration mistake should say exactly what is missing; a transient database error should be a short delay, not a dead app.
**Status:** Active

### [2026-11-22] — CORS origins from configuration; `/health` without a database check
**Context:** The React frontend runs on a different origin, so the browser blocks requests without CORS. Azure needs something to ping.
**Decision:** One CORS policy whose origins come from `Cors:AllowedOrigins` (dev servers in `appsettings.Development.json`, the deployed frontend as `Cors__AllowedOrigins__0` on Azure). No credentials, because the JWT is sent in the `Authorization` header. `/health` is liveness only.
**Why:** No code change or redeploy when the frontend URL changes. A health check that queried the database would keep waking the auto-pausing free database and use up its free monthly allowance.
**Status:** Active
