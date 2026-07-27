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

### Example (delete once I have my own real entries)
**Context:** Needed to decide whether Employer/JobSeeker are separate tables or roles on one ApplicationUser.
**Decision:** Used Identity's built-in Roles system, single ApplicationUser table.
**Why:** Phase 1 has no fields unique to one role that the other doesn't also roughly need (name, email). Adding a second table now would be speculative complexity with no current requirement driving it.
**Status:** Active
