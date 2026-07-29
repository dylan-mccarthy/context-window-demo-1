# Architecture

The Payments API is a small .NET 9 HTTP service that forwards payment requests to an external payment provider. `PaymentClient` owns provider communication and applies the repository retry policy before returning a transport-safe result.

```mermaid
flowchart LR
    Caller --> API[Payments API]
    API --> Client[PaymentClient]
    Client --> Provider[Payment provider]
    API --> Insights[Application Insights]
```

Azure App Service hosts the API. Application Insights and a Log Analytics workspace provide telemetry and centralised diagnostics.
