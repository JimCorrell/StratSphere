# MLB Historical Dataset Import

This guide walks you through importing the Lahman historical baseball database into StratSphere.

## Overview

The MLB historical database contains 150+ years of Major League Baseball statistics. StratSphere imports this data into the PostgreSQL database to create an **MLB Historical Dataset**, which is separate from each league's Strat-o-matic data.

**Tables imported:**

- `MlbPeople` - Player biographical information
- `MlbTeams` - Historical team statistics and information
- `MlbBatting` - Annual batting statistics
- `MlbPitching` - Annual pitching statistics
- `MlbFielding` - Annual fielding statistics
- `MlbAllstars` - All-Star game appearances
- `MlbHallOfFames` - Hall of Fame voting records

## Prerequisites

1. PostgreSQL database running with StratSphere schema (see `QUICKSTART_DATABASE.md`)
2. API running (`dotnet run` from `src/StratSphere.Api`)
3. Admin user authenticated with a valid JWT token
4. MLB CSV files downloaded and extracted (provided in `lahmanDB/lahman_1871-2024u_csv/`)

## Import Process

### 1. Start the Application

```bash
cd src/StratSphere.Api
dotnet run
```

The app will automatically apply the `LahmanTables` migration to create the necessary tables.

### 2. Get an Admin JWT Token

First, register or login with an admin account:

```bash
# Login (replace credentials as needed)
curl -X POST http://localhost:5151/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrUsername": "admin@example.com",
    "password": "AdminPassword123!"
  }'
```

Response:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-12-06T19:00:00Z",
  "user": { ... }
}
```

### 3. Import Lahman Data

Call the import endpoint with the path to your MLB CSV directory:

```bash
curl -X POST http://localhost:5151/api/mlb-import/import \
  -H "Authorization: Bearer <YOUR_JWT_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "csvDirectoryPath": "/Users/jimcorrell/Development/neho/StratSphere/lahmanDB/lahman_1871-2024u_csv"
  }'
```

Response (success example):

```json
{
  "success": true,
  "duration": "00:00:45.250",
  "tablesProcessed": 7,
  "rowsImported": {
    "People.csv": 20051,
    "Teams.csv": 2945,
    "Batting.csv": 11534762,
    "Pitching.csv": 45209,
    "Fielding.csv": 1342516,
    "AllstarFull.csv": 4627,
    "HallOfFame.csv": 8903
  },
  "errors": []
}
```

## Querying Lahman Data

Once imported, you can query the data directly from PostgreSQL:

```sql
-- Find all Hall of Famers
SELECT p.NameFirst, p.NameLast, hof.VotePct, hof.YearElected
FROM public."MlbPeople" p
JOIN public."MlbHallOfFames" hof ON p."PlayerId" = hof."PlayerId"
WHERE hof."YearElected" IS NOT NULL
ORDER BY hof."YearElected" DESC;

-- Career batting stats for a player
SELECT b.Year, b.TeamId, b.Hits, b.HR, b.RBIs, b.BA
FROM public."MlbBatting" b
WHERE b."PlayerId" = 'ruthba01'
ORDER BY b.Year ASC;

-- Teams' historical performance
SELECT Year, Name, Wins, Losses, (Wins::float / (Wins + Losses)) as WinPct
FROM public."MlbTeams"
WHERE League = 'AL' AND Year >= 1990
ORDER BY Year DESC, Wins DESC;
```

## Import Workflow in StratSphere

Future features will expose this data through:

1. **Player Lookup** - Search MLB historical database when scouting historical players
2. **Comparison Reports** - Compare league stats to historical averages
3. **Career Retrospectives** - View player career arcs and Hall of Fame voting

The Lahman data remains in a separate schema from league-specific Strat-o-matic data, ensuring:

- Clean separation of concerns
- Fast lookups of historical stats
- No conflicts with league management data

## Troubleshooting

**Import fails with "Directory not found"**

- Verify the path is correct and accessible
- Use absolute paths (not relative paths)

**Import fails with authentication error**

- Ensure your JWT token is valid and not expired
- Verify you're logged in as an Admin user

**Slow import on first run**

- Importing 20+ million rows takes 30-60 seconds
- This is expected; subsequent queries will be fast due to PostgreSQL indexing

**Missing or duplicate data**

- Check that you're importing the 2024 update (`lahman_1871-2024u_csv`)
- The import uses `AddRange`, so re-running will add duplicates
- To reimport, drop and recreate the tables or use `TRUNCATE` on each table

## Database Schema

View the auto-generated migration file to see the exact schema:

- File: `src/StratSphere.Infrastructure/Migrations/[timestamp]_LahmanTables.cs`
- Tables created with proper indexing for common queries

## Next Steps

1. ✅ Import Lahman data (this guide)
2. Create API endpoints for querying Lahman data
3. Build UI for player lookups and historical comparisons
4. Integrate with draft and roster management

See `DATABASE.md` for more details on the overall database architecture.
