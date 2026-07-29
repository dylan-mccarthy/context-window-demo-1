---
name: C# production conventions
description: Conventions for production C# code
applyTo: "src/**/*.cs"
---

- Nullable reference types are enabled.
- Prefer async APIs for I/O.
- Pass CancellationToken through asynchronous calls.
- Do not throw exceptions for expected payment outcomes.
- Preserve existing public contracts.
