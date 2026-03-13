# StratSphere — Pipeline & Deployment Status

---

## Current State

| Concern | Status |
|---|---|
| Local dev server | `dotnet run` via `.claude/launch.json` (port 5039) |
| Database | Postgres.app on macOS, connection string in user secrets |
| Migrations | **Auto-applied on startup** via `MigrateAsync()` in `Program.cs` |
| Lahman import | Manual: `dotnet run` in `tools/StratSphere.DataImport/` |
| Tests | 141 tests — services, repositories, filter, middleware |
| CI | GitHub Actions: build → test → docker build+push on main |
| Container image | Published to GHCR on every merge to main |
| Hosting | Not yet provisioned |

---

## CI Pipeline

`.github/workflows/ci.yml` — single job, no separate publish step.

- **On every push/PR:** restore → build (Release) → test → Docker build (no push)
- **On merge to `main`:** same steps, then push image to `ghcr.io` with `:latest` and `:<sha>` tags
- **Layer caching:** GitHub Actions cache (`type=gha`) — subsequent builds only re-layer what changed
- **Tests use EF InMemory** — no postgres service required in CI

---

## Containerization

### Dockerfile
- Multi-stage build: SDK image for compile/publish, aspnet runtime image for the final layer
- Runs as non-root (`appuser`) — files owned via `--chown`
- Published output copied from build stage; no SDK in final image

### docker-compose.yml (local Docker dev)
- `db` service: PostgreSQL 16 with healthcheck (`pg_isready`)
- `web` service: waits for `db: condition: service_healthy` before starting
- Migrations run automatically when the web container starts

### .dockerignore
Excludes `tests/`, `**/.git/`, `**/bin/`, `**/obj/`, `.github/`, `docker-compose*.yml`, `.claude/`, `**/*.md`, and editor config directories.

---

## Migration Strategy

EF Core migrations are applied automatically via `MigrateAsync()` at app startup (idempotent — no-op if already applied). This is appropriate for single-instance deployments.

> **If/when moving to multi-instance or blue/green deployments**, move migrations to a pre-deploy step to avoid race conditions. Options:
> - Run `dotnet ef database update` in CI before updating the container
> - Add a `migrator` Docker stage and run it as a one-off job before rolling the web container

---

## Remaining Before First Production Deploy

| Task | Priority |
|---|---|
| Choose hosting provider (Fly.io or Railway recommended) | High |
| Provision managed Postgres and set `ConnectionStrings__DefaultConnection` env var | High |
| Persist ASP.NET Core Data Protection keys (see below) | High |
| Provision Lahman import: store CSVs in object storage, run DataImport CLI once | High |
| Add `/health` endpoint for platform health checks | Medium |
| Structured logging (Serilog or similar) | Low |

---

## Data Protection Keys

By default, ASP.NET Core generates ephemeral data protection keys — they reset on container restart, logging all users out. Before going to production, persist keys:

```csharp
// Program.cs
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<StratSphereDbContext>(); // requires DataProtectionKeys table
```

Or use the host's native secrets manager (Fly secrets, Railway variables, etc.).

---

## Hosting Options

| Option | Notes |
|---|---|
| **Fly.io** | Free tier generous; built-in Postgres; native Docker deploy |
| **Railway** | One-click Postgres; auto-deploys from GitHub |
| **DigitalOcean App Platform** | Docker support; managed Postgres add-on |
| **Azure App Service** | Best if already in Azure ecosystem |
| **Self-hosted VPS** | Full control; requires Nginx + TLS + Docker Compose |

Recommended for V1: **Fly.io** or **Railway** — both handle TLS automatically and have managed Postgres at low cost.

---

## Lahman Import

The Lahman import is not part of the regular deploy pipeline. It runs:
- **Once** on initial server setup
- **Annually** when a new Lahman release is available

```bash
cd tools/StratSphere.DataImport
dotnet run -- \
  --source /path/to/lahman-csvs \
  --connection "Host=...;Database=stratsphere;Username=...;Password=..."
```

Truncates and reloads `lahman.*` schema only. Never touches `public.*` app data. Safe to re-run.
