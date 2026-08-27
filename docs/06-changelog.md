# 06 — Changelog & Progress Checklist

Check this FIRST when sitting down after any gap of more than a couple days.

## Phase 1 checklist (updated for Web API + Clean Architecture)
- [x] GitHub repo created and pushed
- [x] Domain model reasoning started in `02-domain-model.md` (open questions partially answered — recheck before continuing)
- [x] Solution restructured into four projects: Domain, Application, Infrastructure, Api
- [x] Project references configured (Api → Application + Infrastructure; Infrastructure → Application + Domain; Application → Domain; Domain → nothing)
- [x] `.gitignore` corrected and previously-tracked `obj/`/`bin/`/`.vs/` removed from Git index
- [x] `Job` entity created in Domain (public, not internal)
- [x] `JobType` and `Location` enums created in Domain — **overlap between the two needs resolving (see 02-domain-model.md item 3)**
- [x] `IJobRepository` interface (Application) — intentionally not created yet; declined an AI-generated version to write it myself
- [ ] `JobRepository` implementation (Infrastructure)
- [ ] `ApplicationUser` extended, roles set up (Employer, JobSeeker) — decide where it physically lives (Infrastructure vs shared Identity area)
- [x] `ApplicationDbContext` created, EF Core wired to SQL Server
- [ ] First migration run
- [ ] Registration/login endpoints (JWT-based, not cookie-based — playlist doesn't cover this, research separately)
- [ ] Swagger set up and confirmed working (`/swagger` loads, at least one endpoint visible)
- [ ] `JobApplication` entity created and reasoned through
- [ ] `/DTOs` in Application, request/response shapes in place
- [ ] Employer: create job posting (via API)
- [ ] Employer: get only own postings
- [ ] Employer: edit/delete own posting
- [ ] **Security test passed:** Employer B cannot edit Employer A's job via direct API call
- [ ] Job Seeker: public browse endpoint (no auth required)
- [ ] Job Seeker: filter by Job Type and Location
- [ ] Job Seeker: apply (auth required)
- [ ] **Duplicate-apply test passed**
- [ ] Job Seeker: view own applications
- [ ] Employer: view applicants per job
- [ ] **Security test passed:** Job Seeker cannot reach Employer-only endpoints
- [ ] Phase 1 fully working end-to-end via Swagger, committed, pushed

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