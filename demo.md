# Demo 1: The context diet

## Purpose

This demo shows that useful guidance becomes costly when all of it is always loaded. The same retry defect is presented twice: first with one broad repository instruction file, then with lean global guidance, path-specific instructions, a task prompt and curated file context.

The line to land is:

> We did not remove the guidance. We changed when the guidance earns its place in the window.

## Prepared states

| Branch | Purpose | Expected tests |
| --- | --- | --- |
| `demo/start` | Bloated instructions and the retry bug | 11 passed, 1 failed |
| `demo/context-diet` | Lean and scoped instructions plus the prompt; bug still present | 11 passed, 1 failed |
| `demo/complete` | Context diet plus the corrected retry policy | 12 passed |

The transition from `demo/start` to `demo/context-diet` changes only files under `.github`. The transition from `demo/context-diet` to `demo/complete` changes only [PaymentRetryPolicy.cs](src/Payments.Api/Resilience/PaymentRetryPolicy.cs).

## Before the session

1. Open the repository root in VS Code.
2. Close existing Copilot chats so no earlier discussion hints at the fix.
3. Open a terminal at the repository root.
4. Switch to the initial state and confirm the worktree is clean:

   ```shell
   git switch demo/start
   git status --short
   ```

5. Confirm the baseline when rehearsing. The non-zero exit code is intentional:

   ```shell
   dotnet test tests/Payments.Api.Tests/Payments.Api.Tests.csproj --nologo --verbosity minimal
   ```

   Expected summary:

   ```text
   total: 12, failed: 1, succeeded: 11, skipped: 0
   PaymentClient_DoesNotRetryBadRequest
   Expected: 1
   Actual:   3
   ```

6. Keep these files easy to reach:

   - [PaymentClient.cs](src/Payments.Api/Clients/PaymentClient.cs)
   - [PaymentRetryPolicy.cs](src/Payments.Api/Resilience/PaymentRetryPolicy.cs)
   - [PaymentClientTests.cs](tests/Payments.Api.Tests/PaymentClientTests.cs)

## Act 1: Show the full-context cost

1. Confirm that `demo/start` is checked out:

   ```shell
   git branch --show-current
   ```

   Expected output: `demo/start`.

2. Open [copilot-instructions.md](.github/copilot-instructions.md) and briefly scroll through it. Point out that its C#, test, Bicep, documentation and release guidance is individually reasonable.
3. Start a **new Copilot chat** in Agent mode.
4. Enter the deliberately broad prompt:

   ```text
   Fix the retry logic in this repository.
   ```

5. Open the response's **References** view before the task runs too far. The important observation is that the entire global instruction file is present.
6. Call out irrelevant guidance that occupied context for this task, including:

   - Bicep resource tags and managed identity rules
   - Mermaid and Australian English documentation rules
   - Release script and rollback requirements

7. Use this talking point:

   > The advice about Bicep tags, Mermaid diagrams and release scripts is all reasonable. It is also occupying context while we fix one HTTP condition.

8. Stop the agent before it edits the retry policy. If it already made changes, preserve them before switching states:

   ```shell
   git stash push --include-untracked -m "demo/start agent run"
   ```

## Act 2: Apply the context diet

1. Switch to the diet state:

   ```shell
   git switch demo/context-diet
   git status --short
   ```

2. Show the context transformation:

   ```shell
   git diff --stat demo/start..demo/context-diet
   git diff --name-status demo/start..demo/context-diet
   ```

   Only these context files should differ:

   ```text
   .github/copilot-instructions.md
   .github/instructions/bicep.instructions.md
   .github/instructions/csharp.instructions.md
   .github/instructions/tests.instructions.md
   .github/prompts/fix-payment-retry.prompt.md
   ```

3. Open the lean [copilot-instructions.md](.github/copilot-instructions.md).
4. Open the scoped instruction files:

   - [csharp.instructions.md](.github/instructions/csharp.instructions.md)
   - [tests.instructions.md](.github/instructions/tests.instructions.md)
   - [bicep.instructions.md](.github/instructions/bicep.instructions.md)

5. Point out their `applyTo` patterns. C# guidance targets `src/**/*.cs`, test guidance targets `tests/**/*.cs`, and Bicep guidance targets `infra/**/*.bicep`.
6. Open [fix-payment-retry.prompt.md](.github/prompts/fix-payment-retry.prompt.md). Highlight its acceptance criteria and three explicit starting files.
7. Start another **new Copilot chat**. A new chat matters because the repository context changed.
8. Invoke the prompt:

   ```text
   /fix-payment-retry
   ```

9. Open **References** again. The audience should see:

   - Lean repository instructions
   - C# production instructions
   - Test instructions
   - The `fix-payment-retry` prompt file
   - `PaymentClient.cs`
   - `PaymentRetryPolicy.cs`
   - `PaymentClientTests.cs`

10. The audience should **not** see:

    - Bicep instructions
    - Release documentation
    - A broad workspace dump

11. Land the main point:

    > We did not remove the guidance. We changed when the guidance earns its place in the window.

12. Let the agent complete the task. It should remove the broad client-error retry condition while preserving retries for network failures, HTTP 408, HTTP 429 and HTTP 500-599.
13. Confirm the agent runs the payment test project. Expected result:

    ```text
    total: 12, failed: 0, succeeded: 12, skipped: 0
    ```

## Act 3: Show the prepared result

If the live agent run completed successfully, compare it with the prepared ending:

```shell
git diff --stat demo/complete
git diff demo/complete -- src/Payments.Api/Resilience/PaymentRetryPolicy.cs
```

No output means the live result matches the prepared code state.

To move to the guaranteed prepared ending, first preserve any live edits and then switch branches:

```shell
git stash push --include-untracked -m "demo/context-diet agent run"
git switch demo/complete
dotnet test tests/Payments.Api.Tests/Payments.Api.Tests.csproj --nologo --verbosity minimal
```

Expected summary:

```text
total: 12, failed: 0, succeeded: 12, skipped: 0
```

Show the focused code change:

```shell
git diff demo/context-diet..demo/complete -- src/Payments.Api/Resilience/PaymentRetryPolicy.cs
```

The completed policy retries only:

- Transient network failures, handled by `PaymentClient`
- HTTP 408
- HTTP 429
- HTTP 500 through 599

It does not retry other `4xx` responses.

## Recovery and reset

Use these commands between rehearsals:

```shell
git stash push --include-untracked -m "demo rehearsal"
git switch demo/start
git status --short
```

`git status --short` should produce no output. Start fresh Copilot chats for both acts so prior messages do not influence References.

Review saved rehearsal edits with:

```shell
git stash list
```

Delete a saved rehearsal only after confirming it is no longer needed:

```shell
git stash drop stash@{0}
```

## Presenter checklist

- [ ] Start on `demo/start` with a clean worktree
- [ ] Confirm the 11 passed / 1 failed baseline
- [ ] Use a new chat for `Fix the retry logic in this repository.`
- [ ] Open References and identify irrelevant global guidance
- [ ] Switch to `demo/context-diet`
- [ ] Show that only `.github` context files changed
- [ ] Start another new chat
- [ ] Invoke `/fix-payment-retry`
- [ ] Open References and verify the curated context
- [ ] Confirm Bicep and release guidance are absent
- [ ] Land the context-diet line
- [ ] Finish with 12 passing tests
- [ ] Reset to `demo/start` before the next presentation
