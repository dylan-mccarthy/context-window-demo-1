# Operations

## Health

Use `GET /health` for platform health checks. A healthy instance returns HTTP 200 with a small JSON response.

## Payment provider failures

The client retries transient provider and network failures up to three total attempts. Persistent failures are returned to the caller or surfaced as network exceptions after the retry limit.

Inspect Application Insights dependency telemetry and App Service logs in the shared Log Analytics workspace when provider failure rates rise.

## Rollback

Redeploy the previous known-good application package using the release pipeline, then confirm `/health` and one non-production payment request.
