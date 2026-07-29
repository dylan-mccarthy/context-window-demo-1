# Payments Context Demo

A compact .NET 9 payments API with tests, Azure infrastructure, operational documentation and a release script.

## Run locally

```shell
dotnet restore Payments.sln
dotnet run --project src/Payments.Api/Payments.Api.csproj
```

## Test

```shell
dotnet test Payments.sln
```

The service exposes `POST /payments/{paymentId}` and `GET /health`.
