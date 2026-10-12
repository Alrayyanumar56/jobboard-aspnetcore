# 07 — Learning Journal

Rule: entries here come from MY bugs and MY confusion, not from re-transcribing tutorial videos. If I catch myself writing a note that's basically "what the video said," that's not useful — I'll forget it again in a week, same as before. A note earned by actually getting stuck sticks.

## Format
```
[Date] — Concept: one-line plain-English statement of what I now understand, written in my own words, prompted by a real problem I hit.
```

## Entries

[23-Aug-2026] — Concept: `.gitignore` only stops Git from tracking *new* files — it does not untrack files that were already committed. Had to manually remove `obj/`/`bin/` from Git's index after adding the ignore rules, because GitHub Desktop kept showing them as changes.

[23-Aug-2026] — Concept: A class needs to be `public`, not `internal`, if other projects in the solution need to reference it — `internal` restricts visibility to the same assembly/project only. Hit this when `Job` in Domain wasn't visible from Application/Infrastructure.

[11-Oct-2026] — Concept: JWT authentication flow — ASP.NET Identity verifies the user's email and password, retrieves their ID and role, and `JWTService` generates a signed JWT containing claims, issuer, audience, and expiration. The login endpoint returns the token with the user's email and role on success (`200 OK`), or `401 Unauthorized` for invalid credentials. Generating a JWT is separate from validating it on subsequent protected requests.

[12-Oct-2026] — Concept: Generating a token (login) and validating one (middleware on every request) are separate jobs. Authentication = who you are (valid token); authorization = what you're allowed to do (role/ownership). A 401 means unauthenticated, a 403 means authenticated but not permitted. I only trust a protected endpoint after testing it both with and without a token.
### Example (delete once I have more real ones)
[Date] — Concept: Ownership check = compare `job.EmployerId` to the logged-in user's Id before allowing edit — enforced server-side, not just hidden in UI. Got here because I tried pasting another employer's edit URL and it worked when it shouldn't have.
