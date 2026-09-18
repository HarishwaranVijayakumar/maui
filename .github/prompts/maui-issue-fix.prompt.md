---
description: "Fix a MAUI issue with the minimum-credits workflow: narrow search, root cause first, minimal regression test, small patch, focused validation."
---

# MAUI issue fix prompt

Fix this MAUI issue using the lowest-cost workflow possible.

## Critical concerns

- Do not force test-first behavior. Fix the root cause first with the smallest safe change, then ask whether the user wants a regression test added.
- Do not run build or test commands unless the user explicitly asks for it. Suggest the exact local validation command instead.
- Keep the investigation narrow and avoid broad repository exploration.

## Rules

- Work on one issue at a time.
- Start with the most likely component and platform.
- Use one targeted search before editing.
- Read only the exact file and function likely involved.
- Do not broaden scope, refactor unrelated code, or scan the whole repo.
- Identify the root cause before patching.
- Fix the issue first with the smallest safe change.
- Do not add a test before the fix unless the user explicitly asks for test-first validation.
- After the fix explanation, ask manually whether the user wants the smallest regression test added.
- Keep the fix minimal and explicit.
- Do not run build or test commands unless the user explicitly asks for it.
- Prefer to suggest the exact validation command for the user to run locally.
- Validate with the narrowest relevant command.
- Return brief, evidence-based results.

## Output expectations

1. Likely component and root cause
2. Files to inspect and why
3. Minimal fix approach
4. Smallest validation command
5. Result summary with concrete evidence

Use the repository conventions in the MAUI instructions and avoid broad, expensive exploration.
