# Repository overview

This repository contains a .NET 9 payments API, its tests, Azure infrastructure, operational documentation and release scripts. The production application is under `src`, automated tests are under `tests`, infrastructure is under `infra`, and operator-facing material is under `docs`. Keep changes focused on the requested behaviour and preserve the separation between these areas.

# C# conventions

- Use nullable reference types and address nullable warnings rather than suppressing them.
- Prefer sealed classes where inheritance is not intended.
- Use async APIs for I/O and avoid blocking with `.Result`, `.Wait()` or synchronous wrappers.
- Pass `CancellationToken` through all asynchronous calls, including outbound HTTP requests.
- Use primary constructors where they improve clarity without hiding important dependencies.
- Do not throw exceptions for expected payment failures; represent expected provider outcomes explicitly.
- Prefer records for immutable transport objects and value-like results.
- Add XML comments to public APIs when their purpose or contract is not already obvious.
- Keep HTTP transport concerns in client classes and retry decisions in the resilience layer.
- Dispose request and response objects correctly, while allowing injected `HttpClient` instances to be reused.
- Preserve public method signatures unless the task explicitly requires a contract change.
- Do not introduce new NuGet packages without an architecture decision record explaining the need and alternatives.

# HTTP and resilience conventions

- Treat retries as part of the observable payment workflow, not as a way to conceal permanent failures.
- Bound every retry policy by a clear maximum number of attempts.
- Retry only failures known to be transient according to the provider contract and HTTP semantics.
- Ensure request payloads can be sent again safely before enabling retries for an operation.
- Preserve cancellation and do not translate caller cancellation into a payment failure.
- Avoid real delays in unit tests; retry timing belongs in integration or performance validation.
- Never log card numbers, security codes, access tokens or full payment payloads.
- Include payment correlation identifiers in diagnostics when they are safe to retain.

# Testing conventions

- Use xUnit for automated tests.
- Name tests using `Method_Condition_ExpectedResult`.
- Use deterministic hand-written fakes rather than mocking frameworks.
- Organise non-trivial tests into clear Arrange, Act and Assert sections.
- Cover success, failure and cancellation behaviour when changing request handling.
- Assert the number of outbound requests when testing retries so accidental extra calls are visible.
- Avoid timing-dependent tests, network access and reliance on test execution order.
- Keep fake HTTP responses small and dispose resources owned by the test where appropriate.
- Add focused regression coverage for every corrected production defect.
- Run the complete test suite before finishing and report the result accurately.

# Bicep conventions

- Use existing Azure Verified Modules where they meet the requirement.
- All resources require `owner`, `service` and `environment` tags.
- Use managed identities rather than application secrets wherever the target service supports them.
- Diagnostic settings must send supported logs and metrics to the shared Log Analytics workspace.
- Do not hard-code globally unique resource names; derive them predictably from parameters and deployment scope.
- Use symbolic references instead of manually constructing resource IDs.
- Keep environment-specific values in parameters and provide safe defaults only for local or development use.
- Use current stable API versions and validate templates before changing deployment workflows.
- Mark sensitive parameters with `@secure()` and never emit secrets as outputs.
- Keep resource modules cohesive and expose only outputs required by their callers.

# Documentation conventions

- Update `docs/architecture.md` when introducing components, dependencies or changed system boundaries.
- Update `docs/operations.md` when runtime behaviour, failure handling, telemetry or recovery changes.
- Use Australian English in prose, including words such as "behaviour" and "centralised".
- Include Mermaid diagrams for architectural changes and keep diagrams readable in GitHub rendering.
- Document operator actions as concrete steps with observable success and failure signals.
- Do not repeat implementation details that can be read directly from code.

# Release conventions

- Use conventional commits with a concise scope where appropriate.
- Update the changelog for user-visible behaviour changes.
- Do not alter `scripts/release.ps1` without updating `docs/release-process.md` in the same change.
- Production changes require an associated rollback instruction that identifies a known-good state.
- Release scripts must stop on failed builds, tests or package generation.
- Do not embed credentials, subscription identifiers or environment-specific endpoints in scripts.
- Keep generated build and release artefacts out of source control.

# Security and dependency conventions

- Treat payment identifiers as sensitive operational data even when they are not cardholder data.
- Validate untrusted input at the API boundary and encode values used in URLs.
- Use platform configuration providers for secrets and external endpoints.
- Prefer framework capabilities over additional dependencies for small, well-understood behaviours.
- When a dependency update is necessary, review release notes and retain the existing public contract.
- Avoid returning internal exception details or provider credentials in HTTP responses.

# Completion requirements

- Explain what changed and why the change addresses the requested behaviour.
- List modified files.
- Run the relevant focused tests followed by the complete test suite.
- Report commands run, pass and fail counts, and any validation that could not be performed.
- Report remaining risks, assumptions and follow-up work without claiming unrelated guarantees.
- Keep unrelated formatting, infrastructure, documentation and release files unchanged.