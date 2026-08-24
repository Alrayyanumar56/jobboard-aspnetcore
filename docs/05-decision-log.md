# 05 — Decision Log

Every time I make a real architectural or design choice (not just "I wrote a for loop this way" — actual structural decisions), log it here in 3-4 lines. This is what lets me explain my reasoning months later in an interview, instead of just saying "AI suggested it" or "I don't remember why."

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
**Status:** Active


### Example (delete once I have my own real entries)
**Context:** Needed to decide whether Employer/JobSeeker are separate tables or roles on one ApplicationUser.
**Decision:** Used Identity's built-in Roles system, single ApplicationUser table.
**Why:** Phase 1 has no fields unique to one role that the other doesn't also roughly need (name, email). Adding a second table now would be speculative complexity with no current requirement driving it.
**Status:** Active
