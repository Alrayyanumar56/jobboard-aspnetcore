# 06 — Changelog & Progress Checklist

Check this FIRST when sitting down after any gap of more than a couple days.

## Phase 1 checklist (updated for Web API + Clean Architecture) — re-verified against the code on 2026-11-22
- [x] GitHub repo created and pushed
- [x] Domain model reasoning in `02-domain-model.md` (decisions 1, 2 and 4 recorded; 3 still open)
- [x] Solution restructured into four projects: Domain, Application, Infrastructure, Api
- [x] Project references configured (Api → Application + Infrastructure; Infrastructure → Application + Domain; Application → Domain; Domain → nothing)
- [x] `.gitignore` corrected and previously-tracked `obj/`/`bin/`/`.vs/` removed from Git index (root-level leftovers removed 2026-11-22)
- [x] `Job` entity created in Domain (public, not internal)
- [x] `JobType` and `Location` enums created in Domain — **overlap between the two still needs resolving (see 02-domain-model.md item 3)**
- [x] `IJobRepository` interface (Application)
- [x] `JobRepository` implementation (Infrastructure)
- [x] `ApplicationUser` in Infrastructure, roles seeded at startup (Admin, Employer, JobSeeker)
- [x] `ApplicationDbContext` created, EF Core wired to SQL Server
- [x] First migration run
- [x] Registration/login endpoints (JWT-based)
- [x] Swagger set up and confirmed working
- [x] `JobApplication` entity created and reasoned through
- [x] `/DTOs` in Application, request/response shapes in place
- [x] Employer: create job posting
- [x] Employer: get only own postings — `GET /api/Job/mine` (added 2026-11-22; was ticked before it existed)
- [x] Employer: edit/close own posting (close = soft delete, applications kept)
- [x] **Security test:** Employer B cannot edit Employer A's job — covered by `OwnershipTests`; re-check manually on the live API
- [x] Job Seeker: public browse endpoint (no auth required)
- [x] Job Seeker: filter by Job Type and Location — `GET /api/Job?jobType=&location=` (added 2026-11-22)
- [x] Job Seeker: apply (auth required)
- [x] **Duplicate-apply test** — 409, unique index; covered by `OwnershipTests`
- [x] Job Seeker: view own applications
- [x] Employer: view applicants per job (email + name from candidate profile)
- [ ] **Security test:** Job Seeker cannot reach Employer-only endpoints — verify manually on the live API (403)
- [ ] Phase 1 fully working end-to-end on the deployed API

## Phase 2 / backend finishing
- [x] Application status workflow with transition rules and history
- [x] Candidate profile and candidate education
- [x] Azure deployment (App Service F1 + Azure SQL), all 8 migrations applied
- [x] Registration validation fixes and real error messages
- [x] `GET /api/Auth/me`
- [x] CORS, string enums, ProblemDetails errors, `/health`
- [x] Startup config validation + SQL retry on failure
- [x] Unit test project (`JobBoard.Tests`)
- [ ] Redeploy to Azure and run the full flow on the live URL
- [ ] Decide: employer/company profile (company name on job cards) — needs a migration
- [ ] Decide: keep or drop the unused `Admin` role
- [ ] Out of scope for now: resume upload, email notifications, refresh tokens, pagination

## Session log

### [2-Aug-2026]
- Did: Paused Job model build
- Stuck on: ASP.NET Core Identity
- Resolved by: watching videos 65-68, 77, 78, 82 before creating ApplicationUser
- Concept learned: ASP.NET Core Identity and ApplicationUser concept

### [22-Aug-2026]
- Did: Decided to pivot MVC → Web API + Swagger, consumed by React. Logged decision.
- Stuck on: Whether to also do Clean Architecture now or later
- Resolved by: Initially deferred, then reversed the next day
- Concept learned: Naming the "delay becomes permanent" pattern explicitly, and choosing against it deliberately

### [23-Aug-2026]
- Did: Reversed Clean Architecture deferral. Created four-project solution (Domain, Application, Infrastructure, Api) with correct project references. Moved projects into proper repo folder, fixed broken references. Fixed `.gitignore`/tracked-file issue. Created `Job` entity (public) and `JobType`/`Location` enums in Domain.
- Stuck on: Git tracking generated `obj/`/`bin/` files despite `.gitignore`
- Resolved by: Learned `.gitignore` doesn't untrack already-committed files; removed them from the index manually
- Concept learned: `.gitignore` only prevents new tracking, doesn't retroactively untrack. Also: declined an AI-generated `IJobRepository` to preserve understanding — noted as a real win, not just a checklist item.

### [27-Aug-2026]
- Did: - Did: Created IJobRepository, JobRepository, and ApplicationDbContext. Configured LocalDB connection string and began registering EF Core/SQL Server and repository dependencies through DI in the API. Fixed `.gitignore`/tracked-file issue. Created `Job` entity (public) and `JobType`/`Location` enums in Domain.
- Stuck on: Understanding how the DbContext, repository, and SQL Server connection fit together.
- Resolved by: Learned that DbContext tracks entity changes while SaveChanges persists them; understood that Program.cs configures DI and the database connection; fixed SQL Server provider/package and namespace issues.
- Concept learned: A repository interface defines the contract while the Infrastructure repository implements it. EF Core's DbContext sits between the repository and database.

### [2026-08-28]

* Did: Implemented the first `JobRepository` method using `ApplicationDbContext` dependency injection.
* Did: Created `ApplicationDbContext` in Infrastructure and connected it to the local SQL Server database through the connection string in the API configuration.
* Did: Configured EF Core SQL Server support and EF Core tooling after resolving missing package/tooling issues.
* Stuck on: `Add-Migration` initially wasn't recognized, then EF Core reported that `ApplicationDbContext` only had a parameterless constructor.
* Resolved by: Understanding that the context must receive `DbContextOptions<ApplicationDbContext>` and pass those options to the base `DbContext`, allowing the configuration from `AddDbContextPool` to reach EF Core.
* Did: Created the `InitialCreate` EF Core migration and applied it with `Update-Database`.
* Result: Local `JobBoardDB` now contains the `Job` table generated from the `Job` entity.
* Concept learned: EF Core uses the application's configured `DbContext` and migrations to translate the C# domain model into database schema changes; `Update-Database` applies those changes to the actual database.
* Next decision: Resolve the Identity/ApplicationUser ID design before continuing with employer ownership and authentication.

### [2026-09-21]

* Did: Completed user registration using ASP.NET Core Identity through the Web API.
* Did: Created `RegisterRequest` DTO with email, password, and `UserRole`.
* Did: Implemented the registration flow through `IRegistrationService` / `RegistrationService` and `IIdentityService` / `IdentityService`.
* Did: Used `UserManager<ApplicationUser>` to create users and assign Identity roles.
* Did: Tested registration through Swagger with both `Employer` and `JobSeeker` roles.
* Did: Verified duplicate email registration is rejected.
* Did: Verified an invalid password is rejected by ASP.NET Core Identity validation.
* Did: Verified successful registration returns `201 Created`.
* Did: Verified `AspNetUsers` contains the newly registered users.
* Did: Verified `AspNetUserRoles` correctly maps each user to the selected `Employer` or `JobSeeker` role.
* Concept learned: `UserManager<ApplicationUser>` handles Identity user creation and password hashing, while Identity stores role membership separately through `AspNetUserRoles`.
* Result: Registration milestone completed and verified end-to-end through Swagger and SQL Server.
* Next step: Implement JWT-based login and understand the authentication flow before writing the implementation.

### [12-Oct-2026]
- Did: Completed registration, login, and JWT generation. Configured JWT Bearer validation (issuer, audience, signing key, lifetime) with Identity coexisting. Added Swagger Bearer support. Secrets in User Secrets.
- Tested: valid login → 200 + JWT; protected endpoint without token → 401; with token → 200; token removed → 401.
- Next: Employer creates job.

### [2026-11-22]
* Did: Repo cleanup — removed duplicate migration files, old `bin/` and `Properties/` at the repo root, and Office lock files in `docs/`; tidied `.gitignore`.
* Did: Fixed registration: `Role` is now nullable + `[Required]` + `[EnumDataType]` (omitted role was silently Employer; an undefined value created a user with no role, whose login then crashed). Role assignment failure now removes the user again. Registration errors are returned as a `ValidationProblem` with Identity's messages.
* Did: Removed `IdentityResult` from the Application layer (`RegistrationResult` instead), so Application no longer references the ASP.NET Core framework.
* Did: Added `GET /api/Job/mine`, `jobType`/`location` filters on `GET /api/Job`, applicant name in the applicants list, `JobIsClosed` in my applications, and `GET /api/Auth/me`.
* Did: Startup validation of JWT settings and the connection string (clear error instead of an unexplained 500.30), `EnableRetryOnFailure` for Azure SQL (status-change transaction moved inside the execution strategy), CORS from configuration, string enums everywhere, ProblemDetails for errors, `/health`.
* Did: Moved DI registrations into `AddApplication()` / `AddInfrastructure()`.
* Did: Added `JobBoard.Tests` (xUnit): status rules and ownership/duplicate checks against in-memory fake repositories.
* Next: Build, run `dotnet test`, retest in Swagger, redeploy to Azure, add `Cors__AllowedOrigins__0` once the frontend has a URL.

### [2026-11-22] — Backend gaps, batch 1 (no migration)
* Did: `GET /api/Job/mine?status=open|closed|all` (default `all`).
* Did: Paging and keyword search on `GET /api/Job` (`page`, `pageSize` max 50, `search`), response is now `{items, totalCount, page, pageSize}`.
* Did: `GET /api/JobApplication/{id}/candidate`: the job owner sees the applicant's candidate profile and education (404 for anyone else).
* Did: Rate limiting on login/register/change-password (10/min per IP, 429), Identity lockout (5 failures -> 15 minutes, same 401), `POST /api/Auth/change-password`.
* Did: Tests for the applicant-profile ownership rules, paging/search, `/mine` status filter and change-password.
* Next: Retest in Swagger (429 after 10 logins in a minute, lockout after 5 wrong passwords), then batch 2.

### [2026-11-22] — Backend gaps, batch 2 (one migration)
* Did: `EmployerProfile` (`/api/EmployerProfile`), company name on job responses.
* Did: Job: `Hybrid` location, `City`, `ExperienceLevel` (required on create/update), `ApplicationDeadline` (hidden from the listing after it, apply returns 409).
* Did: Optional cover letter on apply, shown to the job owner in `/candidate`.
* Did: Candidate profile: `CvUrl`, `YearsOfExperience`, `ExperienceLevel`.
* Did: Normalized `Skill` table with `JobSkill` / `CandidateProfileSkill` links, skills on job and profile commands/responses, `GET /api/Skill?search=`.
* Did: Tests for skill normalization, deadline rules, skills/company name, cover letter, employer-profile ownership, URL/experience validation.
* Next: Generate and apply the migration, retest in Swagger, then batch 3 (demo data + seeder).
