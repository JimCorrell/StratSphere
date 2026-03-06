# Stratsphere

A multi-tenant web platform for hosting Strat-O-Matic baseball simulation leagues.

## Tech Stack
- ASP.NET Core 8 MVC
- PostgreSQL 16 (two schemas: `public` for app data, `lahman` for historical stats)
- Entity Framework Core 8 + Npgsql
- ASP.NET Core Identity
- Lahman Baseball Database (v2025)

## Getting Started

### 1. Prerequisites
- .NET 8 SDK
- PostgreSQL 16
- (Optional) pgAdmin or psql

### 2. Restore packages
```bash
dotnet restore
```

### 3. Configure database connection
Edit `src/Stratsphere.Web/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=stratsphere_dev;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

### 4. Run EF Core migrations (creates public schema tables)
```bash
cd src/Stratsphere.Web
dotnet ef migrations add InitialCreate --project ../Stratsphere.Data
dotnet ef database update
```

### 5. Import Lahman data (creates lahman schema tables)
Download the latest Lahman CSVs from https://sabr.org/lahman-database/

```bash
cd tools/Stratsphere.DataImport
dotnet run -- \
  --source /path/to/lahman-csvs \
  --connection "Host=localhost;Port=5432;Database=stratsphere_dev;Username=postgres;Password=YOUR_PASSWORD"
```

### 6. Run the app
```bash
cd src/Stratsphere.Web
dotnet run
```

## Project Structure
```
src/
  Stratsphere.Core/       Domain entities, interfaces, services
  Stratsphere.Data/       EF Core DbContext, repositories, migrations
  Stratsphere.Web/        ASP.NET Core MVC app
tools/
  Stratsphere.DataImport/ CLI tool: Lahman CSV -> PostgreSQL
tests/
  Stratsphere.Tests/      xUnit tests
```

## Database Overview
```
public schema   App tables (leagues, teams, rosters, standings, sim stats...)
lahman schema   Read-only Lahman data (people, batting, pitching, fielding)
```

Simulated stats (`sim_batting_stats`, `sim_pitching_stats`) are scoped to
`(card_id, season_id)` — fully isolated per league. The same player card can
have independent stat lines across multiple leagues simultaneously.
