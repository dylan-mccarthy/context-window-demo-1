---
name: Test conventions
description: Conventions for automated tests
applyTo: "tests/**/*.cs"
---

- Use xUnit.
- Name tests using Method_Condition_ExpectedResult.
- Use deterministic hand-written fakes.
- Cover success, failure and cancellation behaviour where relevant.
- Run the relevant test project after changing production code.
