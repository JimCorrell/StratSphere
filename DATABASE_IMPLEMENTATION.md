# PostgreSQL Unified Database Architecture

## Overview

StratSphere now uses a **single PostgreSQL database** for all operations. This simplified architecture eliminates complexity while providing enterprise-grade reliability and scalability.

## Key Changes

✅ **Removed:** SQL Server dependency  
✅ **Removed:** SQLite for production builds  
✅ **Removed:** Separate PlayerDbContext  
✅ **Removed:** Database configuration complexity  
✅ **Kept:** All functionality and features

## Single Database Design

### StratSphere PostgreSQL Database

All data lives in one PostgreSQL instance:

**League & Multi-tenant Data**

- Users, Leagues, LeagueMembers
- Teams, Rosters, RosterEntries
- Drafts, DraftOrders, DraftPicks
- ScoutingReports, Transactions
- Seasons, GameResults, StandingsEntries

**Player Data**

- Players (league-specific)
- PlayerStats
- LahmanPlayer (historical reference)
- LahmanPlayerStats (career statistics)

## Configuration

### Development

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=stratsphere;Username=postgres;Password=postgres;Port=5432"
  }
}
```

### Production

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-db.example.com;Database=stratsphere;Username=prod_user;Password=SECURE_PASSWORD;Port=5432;SSL Mode=Require"
  }
}
```

## Database Setup

### Docker (Recommended for Development)

```bash
docker run --name stratsphere-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=stratsphere \
  -p 5432:5432 \
  -d postgres:latest
```

### Homebrew (macOS)

```bash
brew install postgresql
brew services start postgresql
createdb stratsphere
```

### Initialize Application

```bash
cd src/StratSphere.Api
dotnet run
```

The app automatically:

1. Creates all tables from migrations
2. Seeds 5 Hall of Famers on first run (development only)
3. Listens on http://localhost:5151

## Architecture Benefits

| Feature               | Before                      | Now                               |
| --------------------- | --------------------------- | --------------------------------- |
| **Databases**         | 2 (SQL Server + SQLite)     | 1 (PostgreSQL)                    |
| **Configuration**     | Complex (multiple DB types) | Simple (single connection string) |
| **Migrations**        | Multiple contexts           | Single context                    |
| **Deployment**        | Platform-specific           | Universal (PostgreSQL everywhere) |
| **Data Consistency**  | Across 2 databases          | Single source of truth            |
| **Development Setup** | Docker + LocalDB            | Docker only                       |

## Components

### StratSphereDbContext

- Single unified DbContext for all entities
- Includes LahmanPlayer and LahmanPlayerStats
- Uses PostgreSQL exclusively

### PlayerRepository

- Data access for Lahman historical data
- Search, filtering, stats retrieval
- Methods: SearchPlayersAsync, GetPlayersByPositionAsync, GetHallOfFameAsync, etc.

### PlayerSeeder

- Runs on application startup
- Applies migrations automatically
- Seeds sample data in development

### PlayerDatabaseController

- REST API for player lookups
- Routes: `/api/player-database/*`

## Migration System

### Create Migrations

```bash
cd src/StratSphere.Infrastructure

# New migration
dotnet ef migrations add YourMigrationName --startup-project ../StratSphere.Api

# Apply migrations
dotnet ef database update --startup-project ../StratSphere.Api
```

### View Migrations

```bash
dotnet ef migrations list
```

## NuGet Dependencies

Simplified package requirements:

```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.0" />
<PackageReference Include="Microsoft.IdentityModel.Tokens" Version="8.0.0" />
```

## Performance Optimizations

### Built-in Indexes

Automatic indexes on frequently queried columns:

- LahmanPlayer: PlayerCode, LastName, FirstName, Debut, IsHallOfFame
- LahmanPlayerStats: (LahmanPlayerId, Year), Year, TeamCode

### Connection Pooling

EntityFrameworkCore's built-in pooling handles most cases. For high-concurrency production:

```bash
# Optional: pgbouncer for connection pooling
brew install pgbouncer
```

### Query Performance

- LeagueId filters leveraged with indexes
- Player searches use LastName index
- Composite indexes for common multi-column queries

## API Endpoints

### Player Database

```http
GET /api/player-database/search?searchTerm=ruth
GET /api/player-database/by-position?position=RF
GET /api/player-database/hall-of-fame
GET /api/player-database/{id}
GET /api/player-database/{id}/stats
GET /api/player-database/code/{playerCode}
```

## Troubleshooting

### Connection Issues

```bash
# Test connection
psql -h localhost -U postgres -d stratsphere

# Docker logs
docker logs stratsphere-db
```

### Reset Database (Development)

```bash
dropdb stratsphere
createdb stratsphere
dotnet ef database update --startup-project ../StratSphere.Api
```

### Build Errors

If you see `PlayerDbContext` errors, make sure you've:

1. Updated Program.cs imports
2. Deleted old PlayerDbContext file
3. Run `dotnet clean`

## Files Modified

**Removed:**

- PlayerDbContext.cs
- All PlayerDbContext migrations

**Updated:**

- StratSphereDbContext (added LahmanPlayer/Stats)
- Program.cs (single database registration)
- appsettings.json (single connection string)
- Infrastructure.csproj (removed SQL Server, SQLite)
- PlayerRepository (uses StratSphereDbContext)
- PlayerSeeder (uses StratSphereDbContext)

**Created:**

- PostgreSQLInitial migration

## Next Steps

### Import Full Lahman Data

1. Download from https://www.seanlahman.com/baseball-archive/statistics/
2. Create import endpoint with CSV parsing
3. Use PlayerRepository for batch inserts

### Production Deployment

1. Set up PostgreSQL instance (Azure, AWS, DigitalOcean, etc.)
2. Update connection string in appsettings.json
3. Run migrations in production: `dotnet ef database update`
4. Deploy application

### Monitoring

- Enable PostgreSQL logs: `log_statement = 'all'`
- Monitor connection count
- Track query performance with `EXPLAIN ANALYZE`

## Benefits Summary

✨ **Simpler** - One database to manage  
🚀 **Faster** - No context switching overhead  
🔒 **Safer** - Single source of truth  
📈 **Scalable** - PostgreSQL grows with you  
🌍 **Universal** - Works on any platform
