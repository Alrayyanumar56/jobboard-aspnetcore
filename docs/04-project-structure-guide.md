# 04 — Project Structure Guide

Reference this whenever I forget "wait, where does this kind of file even go" — this is exactly the kind of thing that made me have to re-open every file after a break.

## Folders and what belongs in each

**`/Models`**
Domain entities that map (via EF Core) to database tables — `Job`, `JobApplication`, and Identity extends `ApplicationUser` here too (or in a `/Models/Identity` subfolder if it gets crowded). These represent *what exists in the database*, not what a specific form or page needs.

**`/ViewModels`** (create this folder — it doesn't exist yet in what I've built, and it should)
Shapes tailored to a specific view or form, which may combine fields from multiple entities or omit fields a raw entity has. Example: a `JobCreateViewModel` might exclude `EmployerId` and `PostedDate` entirely, because those are set by the server, not typed by the user — the form should never even offer a field for something the user shouldn't control.
Why this matters: binding a raw entity directly to a form is a classic beginner mistake (over-posting vulnerability) — worth understanding *why*, not just doing it because "that's the convention."

**`/Controllers`**
One controller per major resource area: `JobsController`, `ApplicationsController` (or `ApplicationController`, careful about the naming collision noted in 02-domain-model.md), `AccountController` (Identity may scaffold this, or a slimmer custom one). Controllers should stay thin — receive request, call a service, return a view or redirect. If a controller action is doing database queries directly, that's a signal it should be delegated to the service layer instead.

**`/Services`** (create this folder)
Business logic that doesn't belong in a controller: `IJobService`/`JobService`, containing things like "get all jobs owned by this employer id" or "check whether this user already applied to this job." This is where the Repository pattern concept from the playlist (video 49) actually gets applied, rather than just watched.

**`/Data`**
`ApplicationDbContext`, EF Core migrations folder. Nothing else lives here.

**`/Views`**
Razor views, organized by controller (`/Views/Jobs/Create.cshtml`, etc.), plus `/Views/Shared` for layout and partials.

**`wwwroot/`**
Static CSS/JS/images. This is explicitly the zone where AI-generated Bootstrap markup or styling is fine to lean on — the requirements doc already flagged frontend polish as lower priority than backend understanding.

## A note on file-per-concept discipline
Every time I'm about to create a file, I should be able to answer: "which of the folders above does this belong in, and why." If I can't answer that in one sentence, that's a sign I don't yet understand what the file is for — which is worth stopping and resolving before creating it, not after.
