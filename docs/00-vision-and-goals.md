# 00 — Vision & Goals

## Why this project exists
I followed the Kudvenkat ASP.NET Core playlist up to video 53 by typing along with his project (Employee Management System). I can recognize the vocabulary — controllers, models, DI, Identity, EF Core — but I cannot yet open a blank folder and build something on my own. That gap is the actual problem this project solves.

JobBoard is not "a job board app." It's a forcing function to convert passive tutorial knowledge into independent backend engineering ability.

## Personal motive
- Long-term goal: Junior ASP.NET Core / C# backend developer role, ideally remote/part-time alongside 5th semester, within ~6 months.
- Real production experience so far is in PHP/Laravel/PostgreSQL at the Pace Technologies internship — valuable for general engineering skills, but not the stack I'm targeting.
- I need a project that is 100% mine — every architectural decision, every bug, every fix — so I can speak about it in interviews without hesitation, unlike a tutorial project where I'd be describing someone else's decisions.

## What "done" looks like for Phase 1
- I can explain, without looking at code, why every folder and file exists.
- I can walk someone through the request lifecycle of "an Employer posts a job" from browser to database and back, from memory.
- The two ownership/authorization tests (Employer can't touch another Employer's job; Job Seeker can't reach Employer pages) both fail correctly when I try to break them, and I know *why* they fail correctly.
- Every commit in the Git history was written by my hand, even if AI or the playlist explained a concept behind it.

## Rules of engagement (short version — full version in ai-collaboration-rules.md)
- Playlist = reference for concepts, not a script to copy.
- AI = the "chota" in the shop. I'm the owner. AI does grunt work I explicitly assign or explains a concept — it does not design or write my features for me.
- If I catch myself pasting code I don't understand, that's the signal to stop and go back to first principles, not to keep moving.

## How to use this docs folder
Read this file first whenever you sit down after a break of more than a few days, to re-anchor on why this project exists before touching code. Then check `06-changelog.md` to see where you left off.
