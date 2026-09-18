---
applyTo:
  - "src/**/*.cs"
  - "src/**/*.xaml"
  - "src/**/*.xml"
  - ".github/**/*.md"
---

# MAUI low-cost fix workflow

Use this when fixing a MAUI issue or bug in this repo. Optimize for the lowest credit usage while still producing a correct fix.

## Core rules

1. Work on one issue at a time.
2. Start with the likely component, not a repo-wide search.
3. Do one targeted search or symbol lookup first.
4. Read only the exact file and function likely involved.
5. Identify the root cause before editing.
6. Favor the smallest possible fix.
7. Add or update the smallest relevant regression test.
8. Run the smallest validation command that checks the changed behavior.
9. Do not broaden scope, refactor, or clean unrelated code.
10. Do not run broad solution-level validation unless the issue truly requires it.

## Decision workflow

### Before patching

- Confirm the issue is in the correct MAUI layer: Controls, Core, Essentials, or platform-specific code.
- Prefer the exact file path implied by the issue and platform.
- If the issue is platform-specific, respect the MAUI naming conventions:
  - `.android.cs`, `.ios.cs`, `.maccatalyst.cs`, `.windows.cs`
- If a public API is involved, avoid bypassing analyzers; add the correct API entries if needed.

### Test strategy

- Use the smallest test that proves the fix.
- Prefer unit tests for logic, XAML tests for XAML parsing/compilation, and UI/device tests only when the bug behavior is interaction or platform-specific.
- Do not create broad test sweeps.

### Fix strategy

- Fix the root cause, not the symptom.
- Keep the patch minimal and explicit.
- Do not copy a random existing PR without understanding it.
- Only compare against existing PRs after you have your own fix and a clear reason to consider alternatives.

### Validation strategy

- Prefer one filtered test, one targeted project build, or one reproduction check.
- Avoid full repo builds unless absolutely required.
- If CI/test issues arise, investigate the specific failing area rather than broad automation.

## Anti-patterns to avoid

- Broad repo exploration with no narrow hypothesis
- Reading many files before identifying a likely root cause
- Large refactors unrelated to the bug
- Broad suite runs as a default step
- Asking the model to “fix everything around this issue”
- Copying an existing PR without verifying the root cause
- Committing generated or unrelated files

## Success criteria

A good low-cost fix should be:

- minimal in scope
- rooted in the actual cause
- covered by a small targeted regression test
- validated by a focused command
- easy to review without extra cleanup

Use this workflow whenever the goal is to keep AI credit usage low without sacrificing correctness.
