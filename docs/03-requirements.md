# 03 — Functional Requirements

Scope for the actual API. The bigger product vision (`08-future-vision.md`) does not change this file unless I consciously replan and log why.

## Phase 1 (MVP) — build this and only this first

### Authentication
- Employer and Job Seeker roles, chosen at registration, fixed after registration in Phase 1.
- Passwords hashed via Identity — never logged or stored in plain text.
- Auth exposed as API endpoints (register, login) — token-based (JWT) rather than cookie-based, since this is now an API consumed by React/mobile, not a server-rendered app. This is a gap the playlist doesn't cover (it teaches cookie-based Identity for MVC) — research separately when I get here.

### Employer endpoints
- Create a job posting: Title, Description, Location, Salary Range (optional), Job Type, PostedAt (auto, server-side).
- Get only their own postings.
- Edit/delete a posting only if they own it. **Test explicitly:** call the edit endpoint for Employer A's job while authenticated as Employer B. Must return 403/404, not succeed.
- Get applicants (name + email) per job posting they own.

### Job Seeker endpoints
- Get all postings from all employers — public endpoint, no auth required.
- Filter by Job Type and Location via query parameters.
- Apply to a job — requires auth.
- Not apply twice to the same job — duplicate attempt should return a clear error, not a silent success or a crash.
- Get their own list of applications.

### Explicit exclusions from Phase 1
No resume upload. No messaging. No email notifications. No admin role. No keyword search. No pagination unless a list gets unreasonably long during testing.

### Non-negotiable security rules
- Server-side role + ownership enforcement on every relevant endpoint — never trust the frontend to hide a button as the only protection.
- Every endpoint that mutates data checks both: (a) is this user authorized for this action at all (role), and (b) do they own this specific resource.

## Phase 2 (only after Phase 1 fully works, tested, and committed)
- Resume upload on application.
- Employer changes application status: Pending → Reviewed → Rejected → Accepted.
- Job Seeker sees status updates.
- Basic email notification on status change.

## Definition of done for Phase 1
All boxes in the Phase 1 checklist in `06-changelog.md` are checked, both manual security tests pass via Swagger, and the whole flow (post → browse → apply → view applicants) works end-to-end through API calls alone, without me needing to check code I don't understand.
