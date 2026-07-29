---
name: Bicep conventions
description: Standards for Azure infrastructure
applyTo: "infra/**/*.bicep"
---

- Use existing verified modules.
- Use managed identities rather than secrets.
- Apply the repository's standard resource tags.
- Send diagnostic settings to the shared workspace.
