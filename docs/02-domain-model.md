# 02 — Domain Model

This is architecture-level thinking (what exists and how it relates), not code. Writing the actual C# classes, migrations, and validation logic is still mine to do. Entities below live in `JobBoard.Domain`.

## Core entities

**ApplicationUser** (extends `IdentityUser`, lives in Infrastructure or a shared Identity area — decide and log which)
- Identity already gives you Id, Email, PasswordHash, etc.
- Add a `Role` distinction via ASP.NET Core Identity's built-in Roles system (`Employer` role, `JobSeeker` role, assigned at registration) rather than a custom column.

**Job** (`JobBoard.Domain.Entities.Job`)
- Belongs to exactly one Employer (an `ApplicationUser` in the Employer role).
- Fields as currently implemented: Id, Title, Description, JobType (enum), JobLocation, EmployerId, PostedAt (`DateTimeOffset`), SalaryRange.
- Owner reference: `EmployerId`, a foreign key to the Employer's ApplicationUser Id. This is the field the ownership check compares against.
- **Open item to resolve:** `JobType` enum currently includes `FullTime/PartTime/Contract/HourBased`, and a separate `Location` enum includes `OnSite/Remote`. Check for overlap — is "remote" a job type, a location attribute, or does it legitimately need to exist as both a work arrangement (remote vs onsite) and a broader location field (city/region) once real geography matters? Resolve and log the reasoning before this causes confusion in filtering logic later.

**JobApplication** (careful with naming — avoid confusion with the .NET "Application" project-level term; `JobApplication` as a class name is correct)
- Belongs to exactly one Job.
- Belongs to exactly one Job Seeker (an `ApplicationUser` in the JobSeeker role).
- A given (Job, JobSeeker) pair should be unique — this is where the "can't apply twice" rule lives. Decide whether this is enforced via a unique database constraint, an application-level check before insert, or both.
- Fields (Phase 1): ApplicationDate (server-side).
- Phase 2 adds a Status field (Pending/Reviewed/Rejected/Accepted).

## Relationships (ER, conceptual)
```mermaid
erDiagram
    APPLICATIONUSER ||--o{ JOB : "posts (as Employer)"
    APPLICATIONUSER ||--o{ JOBAPPLICATION : "submits (as JobSeeker)"
    JOB ||--o{ JOBAPPLICATION : "receives"

    APPLICATIONUSER {
        string Id
        string Email
        string Role
    }
    JOB {
        int Id
        string Title
        string Description
        string JobType
        string JobLocation
        string EmployerId
        datetimeoffset PostedAt
        string SalaryRange
    }
    JOBAPPLICATION {
        int Id
        int JobId
        string JobSeekerId
        datetime ApplicationDate
    }
```

## Open design questions (update Decision: lines as resolved)

1. **Employer/JobSeeker as roles on one ApplicationUser table, vs. separate profile tables per role?**
   - Decision: Both, for different things. Roles on the one Identity user table decide *what a user may do*; role-specific *data* lives in separate tables keyed by the user id (`CandidateProfile`, `CandidateEducation`). See decision log 2026-11-14.

2. **Where exactly does the "already applied" duplicate check live (Application layer service vs. database constraint vs. both)?**
   - Decision: Both. `ApplyToJobHandler` checks first for a friendly 409; the unique index on (JobId, JobSeekerId) is the real guarantee when two requests race, and the repository turns that violation into the same 409.

3. **JobType vs Location enum overlap (see note under Job above) — how do these two concepts actually relate?**
   - Decision: They are different things. `JobType` is the contract (FullTime, PartTime, Contract, HourBased); `Location` is the work arrangement (OnSite, Remote, Hybrid). Real geography is a separate free-text `Job.City`. See decision log 2026-11-22.

4. **Where does `ApplicationUser` physically live in the Clean Architecture split — Domain, or Infrastructure (since Identity is a framework/infrastructure concern)?**
   - Decision: Infrastructure (`JobBoard.Infrastructure/Data/ApplicationUser.cs`). Domain entities refer to users only by their string id (`EmployerId`, `JobSeekerId`), and since 2026-11-22 the Application layer has no Identity types at all.

## Implemented since this file was first written
- `JobApplication.Status` + `StatusChangedAt`, with `JobApplicationStatusHistory` recording every change (who, from, to, when). Allowed transitions live in `Domain/Rules/ApplicationStatusRules.cs`.
- `Job.ClosedAt`: closing a job is a soft delete — hidden from browsing, no new applications, existing applications kept.
- `CandidateProfile` (one per JobSeeker) and `CandidateEducation` (many per profile).
- `EmployerProfile` (one per Employer): CompanyName (shown on jobs), Website, About, City.
- `Job`: City, `ExperienceLevel` (Entry/Junior/Mid/Senior/Lead, shared with candidates), `ApplicationDeadline`; `Location` gained Hybrid.
- `JobApplication.CoverLetter`. `CandidateProfile`: CvUrl, YearsOfExperience, ExperienceLevel.
- `Skill` (shared vocabulary, unique `NormalizedName`) with link tables `JobSkill (JobId, SkillId)` and `CandidateProfileSkill (CandidateProfileId, SkillId)`: many-to-many, no navigation properties.

```mermaid
erDiagram
    APPLICATIONUSER ||--o| EMPLOYERPROFILE : "has (as Employer)"
    APPLICATIONUSER ||--o| CANDIDATEPROFILE : "has (as JobSeeker)"
    JOB ||--o{ JOBSKILL : "asks for"
    SKILL ||--o{ JOBSKILL : "used by"
    CANDIDATEPROFILE ||--o{ CANDIDATEPROFILESKILL : "has"
    SKILL ||--o{ CANDIDATEPROFILESKILL : "used by"
    CANDIDATEPROFILE ||--o{ CANDIDATEEDUCATION : "has"
```

## What NOT to model yet
- Resume file storage
- Notifications
- Anything from `08-future-vision.md` (payments, AI matching, freelance mode, CV parsing) — that file is explicitly out of scope until Phase 1/2 are done and I've consciously replanned
