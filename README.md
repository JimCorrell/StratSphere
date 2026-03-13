# StratSphere

A multi-tenant web platform for hosting Strat-O-Matic baseball simulation leagues.

## Tech Stack
- ASP.NET Core 10 MVC
- PostgreSQL 16 (two schemas: `public` for app data, `lahman` for historical stats)
- Entity Framework Core 9 + Npgsql
- ASP.NET Core Identity
- Lahman Baseball Database (v2025)

---

## Getting Started

### Prerequisites
- .NET 10 SDK
- PostgreSQL 16 (macOS: [Postgres.app](https://postgresapp.com/) recommended)

### 1. Clone and restore packages
```bash
git clone <repo>
cd StratSphere
dotnet restore
```

### 2. Create the database
```bash
psql -U postgres -c "CREATE DATABASE stratsphere_dev;"
```

Or with Postgres.app (no password, local superuser):
```bash
/Applications/Postgres.app/Contents/Versions/latest/bin/psql -c "CREATE DATABASE stratsphere_dev;"
```

### 3. Configure the connection string via user secrets

The connection string is stored in user secrets — **not** in `appsettings.json`.

```bash
cd src/Stratsphere.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=stratsphere_dev;Username=YOUR_USER;Password=YOUR_PASSWORD"
```

If using Postgres.app with your OS user as superuser (no password):
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=stratsphere_dev;Username=$(whoami)"
```

### 4. Apply EF Core migrations

Migrations run automatically on startup — no manual step required. If you want to run them manually (e.g. to inspect the SQL before applying):
```bash
# From solution root
~/.dotnet/tools/dotnet-ef database update \
  --project src/Stratsphere.Data \
  --startup-project src/Stratsphere.Web
```

### 5. Import Lahman data

Download the Lahman CSV files from [Sean Lahman's site](http://www.seanlahman.com/baseball-archive/statistics/) or SABR.

```bash
cd tools/StratSphere.DataImport
dotnet run -- \
  --source /path/to/lahman-csvs \
  --connection "Host=localhost;Port=5432;Database=stratsphere_dev;Username=YOUR_USER"
```

This is safe to re-run — it truncates and reloads the `lahman` schema only. It does not touch app data in the `public` schema.

### 6. Run the app
```bash
dotnet run --project src/Stratsphere.Web --launch-profile http
# App runs at http://localhost:5039
```

---

## Project Structure
```
src/
  StratSphere.Core/       Domain entities, interfaces, services
  StratSphere.Data/       EF Core DbContext, repositories, migrations
  StratSphere.Web/        ASP.NET Core MVC app (controllers, views, filters)
tools/
  StratSphere.DataImport/ CLI tool: Lahman CSV → PostgreSQL (lahman schema)
tests/
  StratSphere.Tests/      xUnit tests
```

---

## League Setup Workflow

1. A user **registers** and **creates a league** — they become the commissioner.
2. The commissioner **creates teams** (city, name, abbreviation) for the league.
3. Other users **join the league** via its slug, then **claim an unclaimed team**.
4. The commissioner **creates a season** with a card year (e.g. 1986) — this determines which Lahman player cards are available.
5. Commissioners and team managers **build rosters** via the team detail page — live typeahead search against the Lahman database.
6. The commissioner **enters game scores** — standings update automatically.

---

## Database Overview
```
public schema   App tables (leagues, teams, seasons, rosters, standings)
lahman schema   Read-only Lahman data (people, batting, pitching, fielding)
```

Simulated stats (`sim_batting_stats`, `sim_pitching_stats`) are scoped to
`(card_id, season_id)` — fully isolated per league. The same player card can
have independent stat lines across multiple leagues simultaneously.
