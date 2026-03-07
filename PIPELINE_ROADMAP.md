# StratSphere — Pipeline & Deployment Roadmap

This document describes the steps to automate builds, containerize the application, and establish a repeatable deployment pipeline. Steps are ordered from foundational to advanced.

---

## Current State

| Concern | Status |
|---|---|
| Local dev server | `dotnet run` via `.claude/launch.json` |
| Database | Postgres.app on macOS, connection string in user secrets |
| Migrations | Manual: `scripts/ef.sh database update` |
| Lahman import | Manual: `dotnet run` in `tools/Stratsphere.DataImport/` |
| Tests | Placeholder only (`UnitTest1.cs`) |
| CI/CD | None |
| Hosting | None |

---

## Phase 1 — Test Coverage (prerequisite to meaningful CI)

Before automating builds, the test suite needs to cover the parts most likely to break.

### Priority test targets

1. **`RosterService`** — add/drop uniqueness rules; the `InvalidOperationException` on duplicate add
2. **`StandingsService`** — win/loss recalculation from game results
3. **`LeagueService`** — slug uniqueness on create; join idempotency
4. **`LahmanRepository.SearchCardsAsync`** — batter/pitcher split, limit, name matching

### Setup

```bash
cd tests/Stratsphere.Tests
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Moq
```

Use EF In-Memory provider for repository-layer tests. Mock `ILahmanRepository` for service-layer tests that don't need real SQL.

### CI gate

No PR merges to `main` unless `dotnet test` passes. This is the critical prerequisite before Phase 2.

---

## Phase 2 — Containerization

### `Dockerfile` (multi-stage)

Place at solution root. The multi-stage build keeps the final image small — only the published output, not the SDK.

```dockerfile
# ── Build stage ────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (layer-cached unless .csproj files change)
COPY ["src/Stratsphere.Web/Stratsphere.Web.csproj",   "src/Stratsphere.Web/"]
COPY ["src/StratSphere.Core/StratSphere.Core.csproj", "src/StratSphere.Core/"]
COPY ["src/Stratsphere.Data/Stratsphere.Data.csproj", "src/Stratsphere.Data/"]
RUN dotnet restore "src/Stratsphere.Web/Stratsphere.Web.csproj"

# Copy everything else and publish
COPY . .
RUN dotnet publish "src/Stratsphere.Web/Stratsphere.Web.csproj" \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Stratsphere.Web.dll"]
```

### `.dockerignore`

```
**/bin/
**/obj/
**/.git/
**/.vs/
**/.idea/
**/node_modules/
tools/Stratsphere.DataImport/lahman-csvs/
.claude/settings.local.json
```

### `docker-compose.yml` (local dev with Docker)

```yaml
services:
  db:
    image: postgres:16
    environment:
      POSTGRES_DB: stratsphere_dev
      POSTGRES_USER: stratsphere
      POSTGRES_PASSWORD: localdev
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

  web:
    build: .
    ports:
      - "8080:8080"
    environment:
      ConnectionStrings__DefaultConnection: >
        Host=db;Port=5432;Database=stratsphere_dev;
        Username=stratsphere;Password=localdev
      ASPNETCORE_ENVIRONMENT: Development
    depends_on:
      - db

volumes:
  pgdata:
```

**Note:** The Lahman import is NOT part of `docker-compose` — it's a one-time setup step run manually (see Phase 5).

---

## Phase 3 — CI Pipeline (GitHub Actions)

Create `.github/workflows/ci.yml`:

```yaml
name: CI

on:
  push:
    branches: [main, feature/**]
  pull_request:
    branches: [main]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    services:
      postgres:
        image: postgres:16
        env:
          POSTGRES_DB: stratsphere_test
          POSTGRES_USER: stratsphere
          POSTGRES_PASSWORD: testpass
        ports:
          - 5432:5432
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore -c Release

      - name: Test
        run: dotnet test --no-build -c Release --verbosity normal
        env:
          ConnectionStrings__DefaultConnection: >
            Host=localhost;Port=5432;Database=stratsphere_test;
            Username=stratsphere;Password=testpass

      - name: Docker build check
        run: docker build -t stratsphere:ci .
```

### On PR merge to `main`, additionally:

```yaml
  publish:
    needs: build-and-test
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Log in to GitHub Container Registry
        uses: docker/login-action@v3
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Build and push image
        uses: docker/build-push-action@v5
        with:
          push: true
          tags: ghcr.io/${{ github.repository }}:latest,ghcr.io/${{ github.repository }}:${{ github.sha }}
```

---

## Phase 4 — Migration Strategy for Deployment

EF Core migrations must run before the new app version handles requests. **Never rely on the app auto-migrating at startup** — it causes race conditions when running multiple instances and hides migration failures.

### Option A — Migration container (recommended for most hosts)

Add a second stage to the Dockerfile:

```dockerfile
FROM build AS migrator
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT ["dotnet", "ef", "database", "update", \
  "--project", "src/Stratsphere.Data", \
  "--startup-project", "src/Stratsphere.Web"]
```

In CI/CD, run the migrator container as a pre-deploy job before updating the web container.

### Option B — `migrate.sh` script (simpler, single-server deploys)

```bash
#!/usr/bin/env bash
set -euo pipefail
dotnet ef database update \
  --project src/Stratsphere.Data \
  --startup-project src/Stratsphere.Web \
  --connection "$CONNECTION_STRING"
```

Run this in the deploy step before restarting the web process.

### What migrations cover

EF migrations manage the `public` schema only. The `lahman` schema is separate (see Phase 5).

---

## Phase 5 — Lahman Import Pipeline

The Lahman import is not part of the regular deploy pipeline. It runs:
- **Once** on initial server setup
- **Annually** when a new Lahman release comes out

It is intentionally kept manual and separate from the app deploy because:
- It takes ~2-5 minutes and loads ~200k rows
- It truncates and reloads `lahman.*` — safe but slow
- It never touches `public.*` app data

### Recommended approach

1. Store Lahman CSVs in a private S3 bucket or similar object storage.
2. Create a one-off job (GitHub Actions `workflow_dispatch`, or a cron task on the server) that:
   - Downloads the CSVs from object storage
   - Runs the DataImport CLI
3. Document the import command clearly in `README.md` so it can be run manually when needed.

---

## Phase 6 — Hosting Options

| Option | Best for | Notes |
|---|---|---|
| **Fly.io** | Low-cost, hobby/small leagues | Free tier generous; built-in Postgres; supports Docker natively |
| **Railway** | Fast setup, minimal ops | One-click Postgres; automatic deploys from GitHub |
| **Azure App Service + Azure Database for PostgreSQL** | Enterprise, team projects | More config; best if already in Azure ecosystem |
| **DigitalOcean App Platform** | Mid-tier, predictable pricing | Docker support; managed Postgres add-on |
| **Self-hosted VPS** | Full control | Requires Nginx reverse proxy, TLS setup, systemd or Docker Compose |

### Recommended for V1: Fly.io or Railway

Both support deploying from a `Dockerfile`, provide managed Postgres, handle TLS automatically, and have free or very low-cost tiers suitable for a small league platform.

---

## Phase 7 — Secrets Management in Production

Never use user secrets or `appsettings.*.json` files in production.

| Secret | Production approach |
|---|---|
| DB connection string | Environment variable: `ConnectionStrings__DefaultConnection` |
| Identity data protection keys | Persist to DB or blob storage (ASP.NET Core Data Protection API) |
| Any future API keys | Environment variables or host-native secrets manager |

### Data protection keys

By default, ASP.NET Core generates ephemeral data protection keys — they reset on container restart, logging all users out. For production, persist keys:

```csharp
// Program.cs
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<StratSphereDbContext>(); // requires DataProtectionKeys table
```

Or use a cloud key store (Azure Key Vault, AWS Secrets Manager).

---

## Phase 8 — Health Checks & Monitoring

### Health check endpoint

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddNpgsql(connectionString);

app.MapHealthChecks("/health");
```

Hosting platforms use this to know when the container is ready and when to restart it.

### Logging

Structured logging with Serilog is the standard choice for ASP.NET Core in production:

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.Seq   # optional, for a log UI
```

For V1, console logging (captured by the host's log aggregator) is sufficient.

---

## Summary Checklist

| Phase | Task | Priority |
|---|---|---|
| 1 | Write tests for RosterService, StandingsService, LeagueService | Before CI |
| 2 | Write `Dockerfile` (multi-stage) and `.dockerignore` | Before hosting |
| 2 | Write `docker-compose.yml` for local Docker dev | Low — optional |
| 3 | GitHub Actions CI (build + test on PR) | High |
| 3 | GitHub Actions CD (push image on merge to main) | After hosting chosen |
| 4 | Migration step in deploy pipeline | Before first production deploy |
| 5 | Lahman CSV storage + one-off import job | Before launch |
| 6 | Choose hosting provider and provision environment | Before launch |
| 7 | Production secrets via environment variables | Before launch |
| 7 | Data protection key persistence | Before launch |
| 8 | `/health` endpoint | Before launch |
| 8 | Structured logging (Serilog) | Nice-to-have |
