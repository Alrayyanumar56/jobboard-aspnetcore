# 03 — Functional Requirements

## Phase 1 (MVP) — build this and only this first

### Authentication
- Employer and Job Seeker roles, chosen at registration, fixed after registration in Phase 1.
- Passwords hashed via Identity — never logged or stored in plain text.

### Employer can
- Create a job posting: Title, Company Name, Description, Location, Salary Range (optional), Job Type (Full-time/Part-time/Remote), Posted Date (auto, server-side).
- View a dashboard of ONLY their own postings.
- Edit/delete a posting only if they own it. **Test explicitly:** log in as Employer A, grab the edit URL of one of their jobs, log in as Employer B, paste that URL directly. Must fail.
- View applicants (name + email) per job posting they own.

### Job Seeker can
- Browse all postings from all employers, publicly, without logging in.
- Filter by Job Type and Location.
- Apply only while logged in — one click + confirm, no cover letter/resume in Phase 1.
- Not apply twice to the same job.
- View their own list of applications.

### Explicit exclusions from Phase 1
No resume upload. No messaging. No email notifications. No admin role. No keyword search. No pagination unless the list gets unreasonably long during testing.

### Non-negotiable security rules
- Server-side enforcement (not just hidden UI) that Job Seekers can never reach Employer-only pages.
- Ownership check on every edit/delete action.

## Phase 2 (only after Phase 1 fully works, tested, and committed)
- Resume upload on application (port understanding from the PHP file-upload work at the internship).
- Employer changes application status: Pending → Reviewed → Rejected → Accepted.
- Job Seeker sees status updates on their own application list.
- Basic email notification on status change (candidate topic to learn background jobs / simple SMTP).

## Definition of done for Phase 1
All boxes in the Phase 1 checklist in `06-changelog.md` are checked, both manual security tests pass, and the whole flow (post → browse → apply → view applicant) works end-to-end without me needing to check code I don't understand.
