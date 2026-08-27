# 07 — Learning Journal

Rule: entries here come from MY bugs and MY confusion, not from re-transcribing tutorial videos. If I catch myself writing a note that's basically "what the video said," that's not useful — I'll forget it again in a week, same as before. A note earned by actually getting stuck sticks.

## Format
```
[Date] — Concept: one-line plain-English statement of what I now understand, written in my own words, prompted by a real problem I hit.
```

## Entries

[23-Aug-2026] — Concept: `.gitignore` only stops Git from tracking *new* files — it does not untrack files that were already committed. Had to manually remove `obj/`/`bin/` from Git's index after adding the ignore rules, because GitHub Desktop kept showing them as changes.

[23-Aug-2026] — Concept: A class needs to be `public`, not `internal`, if other projects in the solution need to reference it — `internal` restricts visibility to the same assembly/project only. Hit this when `Job` in Domain wasn't visible from Application/Infrastructure.

### Example (delete once I have more real ones)
[Date] — Concept: Ownership check = compare `job.EmployerId` to the logged-in user's Id before allowing edit — enforced server-side, not just hidden in UI. Got here because I tried pasting another employer's edit URL and it worked when it shouldn't have.
