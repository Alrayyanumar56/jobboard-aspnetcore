# 08 — Future Vision (NOT current scope)

This file holds the bigger long-term idea for JobBoard as a real product. **Nothing in this file is part of Phase 1 or Phase 2.** It exists so ideas get captured without silently expanding the current sprint. Anything here only becomes real scope after Phase 1 is fully done, tested, and I've made a conscious, logged decision to replan.

## The bigger idea
A full-scale job marketplace, not just a CRUD demo:
- Users upload a CV/resume; system auto-builds a profile from it, editable before final submission.
- Reverse direction too: build a profile manually, generate a resume from it via templates.
- Interest/field tagging at profile creation, used to drive personalized job suggestions.
- Freemium model: free accounts can apply to a limited number of jobs (e.g. 10); a small one-time payment (e.g. Rs. 500) unlocks a much higher cap (e.g. 100).
- Two modes: traditional job-seeking and freelance/contract work, switchable per user.
- Employers/recruiters post jobs for free, always — monetization is on the seeker side, not the employer side, to keep job supply high.
- AI-assisted matching between seeker profile/interests and available postings.
- Eventually: not limited to technical/software roles — a general-purpose job board across departments.

## Why this is documented separately from Phase 1/2
- Phase 1/2 exist to build *my* ability to design and ship a backend independently. This vision is 10-50x the scope of that, and conflating the two would guarantee neither gets finished.
- Real market context: Pakistan already has established players (e.g. Rozee.pk) doing parts of this, plus global players (Indeed, LinkedIn). Before treating this as an obvious market gap, I should actually use the existing competitors and note specifically what they do badly — "no one's solved this" needs to be verified, not assumed.
- This also intersects with real personal/academic decisions (FYP topic, teammates, supervisor) that shouldn't be made impulsively based on excitement about this vision — see note below.

## Guardrails for myself, written while level-headed
- Do not restructure or expand JobBoard's Phase 1/2 requirements because of this file. If an idea from here feels urgent, that's the signal to write it down here in more detail, not to start building it.
- Do not make unilateral decisions about the FYP (e.g. leaving the current group, telling the supervisor something untrue) based on this vision without going through the university's actual process for changing an FYP topic, and without an honest conversation with my current teammates and supervisor first.
- Secrecy about the idea is not a strategy on its own — execution over months is what actually protects an idea, and total secrecy costs real feedback and help. Decide deliberately who (if anyone) to loop in, rather than defaulting to telling no one out of fear.
- Cost reality check: an MVP costs close to $0 (free-tier hosting, free SQL Server tier). Real costs appear later — Pakistani payment gateway integration (JazzCash/EasyPaisa, not Stripe), eventual business registration. Money is not the current constraint; time and finishing power are.

## Revisit trigger
Come back to this file once Phase 1 (and ideally Phase 2) of JobBoard is fully done and logged as complete in `06-changelog.md`. Not before.
