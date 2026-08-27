# 00 — Vision & Goals

## Why this project exists
I followed the Kudvenkat ASP.NET Core playlist up to video 53 by typing along with his project (Employee Management System). I could recognize the vocabulary — controllers, models, DI, Identity, EF Core — but couldn't yet open a blank folder and build something on my own. That gap is the actual problem this project solves.

JobBoard is not "a job board app." It's a forcing function to convert passive tutorial knowledge into independent backend engineering ability — now specifically: ability to design and build REST APIs with Clean Architecture.

## Personal motive
- Long-term goal: Junior ASP.NET Core / C# backend developer role, ideally remote/part-time alongside 5th semester, within ~6 months.
- Real production experience so far is in PHP/Laravel/PostgreSQL at the Pace Technologies internship — valuable for general engineering skills, but not the stack I'm targeting.
- I need a project that is 100% mine — every architectural decision, every bug, every fix — so I can speak about it in interviews without hesitation, unlike a tutorial project where I'd be describing someone else's decisions.

## Current architecture direction (see 05-decision-log.md for the full reasoning)
- **Pivoted from MVC/Razor to ASP.NET Core Web API**, documented via Swagger/OpenAPI, intended to be consumed by a React frontend (frontend build assisted by AI — my focus stays backend).
- **Adopted full Clean Architecture from the start** (Domain / Application / Infrastructure / API as separate projects in one solution) — this reverses an earlier "defer it" decision. Reasoning: repeatedly delaying more advanced structure has, historically, meant never doing it. Better to learn it now, deliberately, while the project is still small.

## What this project is also being used for
Beyond personal skill-building, JobBoard's backend (the Web API layer) is intended to double as:
- The API layer for a university mobile application project.
- The basis for a web programming coursework project this semester.
Both of these are downstream consumers of the same API — they do not change what gets built in the API itself. Scope for the API stays governed by `03-requirements.md`, not by whatever either course project needs on a given week.

## What "done" looks like for Phase 1
- I can explain, without looking at code, why every project and folder exists, including why Domain doesn't depend on anything else.
- I can walk someone through the request lifecycle of "an Employer posts a job" from HTTP request to database and back, from memory.
- The two ownership/authorization tests (Employer can't touch another Employer's job; Job Seeker can't reach Employer-only endpoints) both fail correctly when I try to break them, and I know *why*.
- Every commit in the Git history was written by my hand, even if AI or the playlist explained a concept behind it.
- Swagger correctly documents every endpoint and I can exercise the full flow (post → browse → apply → view applicants) through it, without a frontend.

## Rules of engagement (short version — full version in ai-collaboration-rules.md)
- Playlist = reference for concepts, not a script to copy.
- AI = the "chota" in the shop. I'm the owner. AI does grunt work I explicitly assign or explains a concept — it does not design or write my features for me.
- If I catch myself pasting code I don't understand, that's the signal to stop and go back to first principles, not to keep moving.

## A note on scope discipline
I have a larger long-term product vision for this idea (see `08-future-vision.md`) — CV parsing, AI-based matching, paid tiers, freelance mode, etc. That vision is deliberately kept in its own file and is NOT part of Phase 1 or Phase 2 requirements. Every time a new feature idea comes up mid-build, it goes into that file, not into the current sprint, unless I've consciously decided to replan and logged why.

## How to use this docs folder
Read this file first whenever I sit down after a break of more than a few days, to re-anchor on why this project exists before touching code. Then check `06-changelog.md` to see where I left off.
