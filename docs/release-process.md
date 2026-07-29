# Release process

1. Run `dotnet test Payments.sln`.
2. Run `./scripts/release.ps1 -Version <version>` from PowerShell.
3. Publish the generated package through the deployment pipeline.
4. Confirm the health endpoint and payment-provider dependency telemetry.

For rollback, redeploy the preceding package version and repeat the health checks.
