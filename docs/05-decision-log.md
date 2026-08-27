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

### [Date] — Employer/JobSeeker: roles vs. separate tables
**Context:** Needed to decide whether Employer/JobSeeker are separate tables or roles on one ApplicationUser.
**Decision:**
**Why:**
**Status:**
