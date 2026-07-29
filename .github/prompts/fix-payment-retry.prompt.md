---
name: fix-payment-retry
description: Correct and validate transient HTTP retry behaviour
agent: agent
---

Correct the retry behaviour in the payment HTTP client.

Acceptance criteria:

1. Retry transient network failures.
2. Retry HTTP 408 and HTTP 429.
3. Retry HTTP 500 through 599.
4. Do not retry other HTTP 400-series responses.
5. Preserve the public API.
6. Update or add focused tests where necessary.
7. Run the payment API test project.
8. Summarise the files changed and validation performed.

Start with these files:

- [PaymentClient](../../src/Payments.Api/Clients/PaymentClient.cs)
- [Retry policy](../../src/Payments.Api/Resilience/PaymentRetryPolicy.cs)
- [Tests](../../tests/Payments.Api.Tests/PaymentClientTests.cs)