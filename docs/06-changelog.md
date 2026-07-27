# 06 — Changelog & Progress Checklist

This is the file to check FIRST when sitting down after any gap of more than a couple days — answers "where did I leave off" without needing to re-read every file.

## Phase 1 checklist
- [ ] GitHub repo created and pushed
- [ ] Domain model reasoning done in `02-domain-model.md` (open questions answered, not left blank)
- [ ] `ApplicationUser` extended, roles set up (Employer, JobSeeker)
- [ ] Registration flow lets user pick a role
- [ ] `Job` model created and reasoned through
- [ ] `JobApplication` model created and reasoned through
- [ ] `/ViewModels` folder created, `JobCreateViewModel` (or similar) in place
- [ ] `/Services` folder created, `JobService` handling ownership logic
- [ ] Employer: create job posting
- [ ] Employer: dashboard shows only own postings
- [ ] Employer: edit/delete own posting
- [ ] **Security test passed:** Employer B cannot edit Employer A's job via direct URL
- [ ] Job Seeker: public browse list (no login required)
- [ ] Job Seeker: filter by Job Type and Location
- [ ] Job Seeker: apply (logged in only)
- [ ] **Duplicate-apply test passed:** applying twice to the same job is blocked
- [ ] Job Seeker: view own applications
- [ ] Employer: view applicants per job
- [ ] **Security test passed:** Job Seeker cannot reach Employer-only pages via direct URL
- [ ] Phase 1 fully working end-to-end, committed, pushed

## Session log
(One short entry per real coding session — what I did, what I got stuck on, what I learned. Not a diary, just a breadcrumb trail.)

### [Date]
- Did:
- Stuck on:
- Resolved by:
- Concept learned:
