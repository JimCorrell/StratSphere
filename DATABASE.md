# Database Architecture

## Overview

StratSphere uses a **single PostgreSQL database** for all data—league management, player data, and historical statistics. This unified approach provides:

- **Simplicity** - One database, one connection string, one migration system
- **Scalability** - PostgreSQL handles everything from development to production
- **Multi-tenancy** - All data is scoped by LeagueId
- **ACID compliance** - Data integrity across all operations

## Database Design

### Connection String

```json
"DefaultConnection": "Host=localhost;Database=stratsphere;Username=postgres;Password=postgres;Port=5432"
```

### Core Tables

**League Management**

- Users
- Leagues
- LeagueMembers
- Teams
- Seasons
- StandingsEntries
- GameResults
- Transactions

**Drafts**

- Drafts
- DraftOrders
- DraftPicks

**Rosters & Scouting**

- RosterEntries
- PlayerStats
- Players
- ScoutingReports

**MLB Historical Data (Lahman CSV Source)**

- MlbPeople
- MlbTeam
- MlbBatting
- MlbPitching
- MlbFielding
- MlbAllstar
- MlbHallOfFame

## Running Migrations

### Initial Setup

```bash
cd src/StratSphere.Infrastructure

# Create and apply migrations
dotnet ef database update --startup-project ../StratSphere.Api
```

### Creating New Migrations

```bash
cd src/StratSphere.Infrastructure

# Create migration
dotnet ef migrations add YourMigrationName --startup-project ../StratSphere.Api

# Apply migration
dotnet ef database update --startup-project ../StratSphere.Api
```

## PostgreSQL Setup

### Local Development

**Option 1: Using Docker**

```bash
docker run --name stratsphere-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=stratsphere \
  -p 5432:5432 \
  -d postgres:latest
```

**Option 2: Using Homebrew (macOS)**

```bash
brew install postgresql
brew services start postgresql
createdb stratsphere
```

**Option 3: Using PostgreSQL Installer**

- Download from https://www.postgresql.org/download/
- Follow installation instructions for your OS
- Create database: `createdb stratsphere`

### Production Deployment

Update `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-db.example.com;Database=stratsphere;Username=prod_user;Password=secure_password;Port=5432;SSL Mode=Require"
  }
}
```

## MLB Historical Data

The database includes comprehensive tables for MLB historical player and team data, imported from the Lahman database CSV files via the MlbImportService.

### MlbPeople

Player biographical data and career information:

```csharp
PlayerId (string) - Primary key (retrosheet ID)
NameFirst, NameLast (string)
BirthDate, DeathDate (DateTime, nullable)
BirthCity, BirthCountry (string)
BirthState (string, nullable)
Bats, Throws (char)
Height, Weight (int, nullable)
Debut, FinalYear (int, nullable)
Retrosheet (string, nullable)
BbRefId (string, nullable)
NameGiven (string, nullable)
Weight (int, nullable)
ImportedAt (DateTime)
```

### MlbTeam

Historical team records and statistics:

```csharp
TeamId (string) - Primary key
Year (int)
LgId (string) - League ID
TeamCode (string) - Team abbreviation
NameFull (string) - Full team name
NameShort (string, nullable) - Short name
NameNote (string, nullable)
Franchid (string, nullable) - Franchise ID
Div (string, nullable) - Division
Rank (int, nullable) - Division rank
Games (int, nullable)
GamesHome (int, nullable)
W (int, nullable) - Wins
L (int, nullable) - Losses
DivWin (string, nullable)
WcWin (string, nullable)
LgWin (string, nullable)
WorldSeries (string, nullable)
// ... additional fields for runs, hits, ERA, etc.
ImportedAt (DateTime)
```

### MlbBatting

Annual batting statistics by player:

```csharp
BattingId (string) - Primary key
PlayerId (string) - FK to MlbPeople
Year (int)
Stint (int)
TeamId (string) - Team abbreviation
LgId (string) - League ID
G (int, nullable) - Games
AB (int, nullable) - At-bats
R (int, nullable) - Runs
H (int, nullable) - Hits
B2 (int, nullable) - Doubles
B3 (int, nullable) - Triples
HR (int, nullable) - Home runs
RBI (int, nullable) - RBIs
SB (int, nullable) - Stolen bases
CS (int, nullable) - Caught stealing
BB (int, nullable) - Walks
SO (int, nullable) - Strikeouts
IBB (int, nullable) - Intentional walks
HBP (int, nullable) - Hit by pitch
SH (int, nullable) - Sacrifice hits
SF (int, nullable) - Sacrifice flies
GIDP (int, nullable) - Grounded into double play
ImportedAt (DateTime)
```

### MlbPitching

Annual pitching statistics by player:

```csharp
PitchingId (string) - Primary key
PlayerId (string) - FK to MlbPeople
Year (int)
Stint (int)
TeamId (string) - Team abbreviation
LgId (string) - League ID
W (int, nullable) - Wins
L (int, nullable) - Losses
G (int, nullable) - Games
GS (int, nullable) - Games started
CG (int, nullable) - Complete games
SHO (int, nullable) - Shutouts
SV (int, nullable) - Saves
IPOuts (int, nullable) - Innings pitched (outs)
H (int, nullable) - Hits allowed
R (int, nullable) - Runs allowed
ER (int, nullable) - Earned runs
HR (int, nullable) - Home runs allowed
BB (int, nullable) - Walks
SO (int, nullable) - Strikeouts
BAOpp (decimal, nullable) - Batting average against
ERA (decimal, nullable) - Earned run average
IBB (int, nullable) - Intentional walks
WP (int, nullable) - Wild pitches
HBP (int, nullable) - Hit batters
BK (int, nullable) - Balks
BFP (int, nullable) - Batters faced
GF (int, nullable) - Games finished
R64 (int, nullable) - Runs allowed (64)
ImportedAt (DateTime)
```

### MlbFielding

Annual fielding statistics by player and position:

```csharp
FieldingId (string) - Primary key
PlayerId (string) - FK to MlbPeople
Year (int)
Stint (int)
Pos (string) - Position (e.g., "SS", "C", "OF")
TeamId (string) - Team abbreviation
LgId (string) - League ID
G (int, nullable) - Games
GS (int, nullable) - Games started
InnOuts (int, nullable) - Innings played
PO (int, nullable) - Put outs
A (int, nullable) - Assists
E (int, nullable) - Errors
DP (int, nullable) - Double plays
PB (int, nullable) - Passed balls
WP (int, nullable) - Wild pitches
SB (int, nullable) - Stolen bases allowed
CS (int, nullable) - Caught stealing
ZR (decimal, nullable) - Zone rating
ImportedAt (DateTime)
```

### MlbAllstar

All-Star game appearances:

```csharp
AllstarId (string) - Primary key
PlayerId (string) - FK to MlbPeople
Year (int)
GameNum (int, nullable) - Game number if multiple in a year
GameId (string, nullable)
TeamId (string, nullable) - Team representation
LgId (string, nullable) - League
GP (int, nullable) - Games played
StartingPos (string, nullable) - Starting position
AB (int, nullable)
R (int, nullable)
H (int, nullable)
B2 (int, nullable)
B3 (int, nullable)
HR (int, nullable)
RBI (int, nullable)
BB (int, nullable)
K (int, nullable)
IBB (int, nullable)
HBP (int, nullable)
SH (int, nullable)
SF (int, nullable)
GDP (int, nullable)
ImportedAt (DateTime)
```

### MlbHallOfFame

Hall of Fame voting records:

```csharp
HofId (string) - Primary key
PlayerId (string) - FK to MlbPeople
YearElected (int, nullable) - Year inducted (null if not elected)
BuiltInHof (int, nullable) - Built-in HOF indicator
FullPlayerName (string, nullable)
Votes (int, nullable) - Votes received
NeededVotes (int, nullable)
YearOnBallot (int)
YearLastOnBallot (int, nullable)
PlayersVotedFor (int, nullable)
ImportedAt (DateTime)
```

## Indexes

For optimal query performance, the following indexes are created on MLB tables:

**MlbPeople**

- PlayerId (unique, primary key)
- NameLast, NameFirst
- Debut

**MlbBatting**

- PlayerId, Year (composite)
- Year
- TeamId

**MlbPitching**

- PlayerId, Year (composite)
- Year
- TeamId

**MlbFielding**

- PlayerId, Year, Pos (composite)
- PlayerId, Year
- Pos

**MlbAllstar**

- PlayerId, Year (composite)
- Year

**MlbHallOfFame**

- PlayerId (unique)
- YearElected

## Player Repository API

The IPlayerRepository interface provides methods for querying MLB historical data:

```csharp
// Get all Hall of Fame players (joins with MlbHallOfFames where YearElected is not null)
Task<List<MlbPeople>> GetHallOfFameAsync();

// Get a specific player by ID
Task<MlbPeople?> GetPlayerByIdAsync(int id);

// Get player batting statistics for a given year range
Task<List<MlbBatting>> GetPlayerBattingStatsAsync(string playerId, int? startYear = null, int? endYear = null);

// Get player pitching statistics for a given year range
Task<List<MlbPitching>> GetPlayerPitchingStatsAsync(string playerId, int? startYear = null, int? endYear = null);

// Get player fielding statistics for a given year range
Task<List<MlbFielding>> GetPlayerFieldingStatsAsync(string playerId, int? startYear = null, int? endYear = null);

// Search for players by name (case-insensitive, searches NameFirst and NameLast)
Task<List<MlbPeople>> SearchPlayersAsync(string searchTerm, int limit = 50);

// Get players by position (queries MlbFieldings, joins with MlbPeople)
Task<List<MlbPeople>> GetPlayersByPositionAsync(string position, int limit = 50);

// Persist changes to the database
Task SaveChangesAsync();
```

### Usage Examples

```csharp
// Get Hall of Fame members
var hofPlayers = await _playerRepository.GetHallOfFameAsync();

// Get a specific player
var player = await _playerRepository.GetPlayerByIdAsync(1);

// Get player's batting stats for years 2010-2015
var battingStats = await _playerRepository.GetPlayerBattingStatsAsync("playerid123", 2010, 2015);

// Search for players
var results = await _playerRepository.SearchPlayersAsync("ruth", limit: 50);

// Get all shortstops
var shortstops = await _playerRepository.GetPlayersByPositionAsync("SS", limit: 100);
```

## API Endpoints

### Player Lookup

```http
GET /api/player-database/search?searchTerm=ruth
GET /api/player-database/by-position?position=RF
GET /api/player-database/hall-of-fame
GET /api/player-database/{id}
GET /api/player-database/{id}/stats
GET /api/player-database/code/{playerCode}
```

## Performance Considerations

### Connection Pooling

PostgreSQL connection pooling is configured by default in EntityFrameworkCore. For production with many connections, consider:

```bash
# Install pgbouncer
brew install pgbouncer

# Configure ~/.pgbouncer/pgbouncer.ini
# Add: stratsphere = host=localhost port=5432 dbname=stratsphere

# Start pgbouncer
pgbouncer -d ~/.pgbouncer/pgbouncer.ini
```

### Query Optimization

- Indexes are automatically created on commonly filtered columns
- Use `FromSqlInterpolated()` for complex queries
- Connection string includes connection timeout settings

### Backup

```bash
# Full database backup
pg_dump -U postgres stratsphere > stratsphere_backup.sql

# Restore from backup
psql -U postgres stratsphere < stratsphere_backup.sql
```

## Troubleshooting

### Connection Issues

```bash
# Test connection
psql -h localhost -U postgres -d stratsphere

# Check PostgreSQL service status
brew services list  # macOS
systemctl status postgresql  # Linux

# Container logs (Docker)
docker logs stratsphere-db
```

### Database Reset (Development Only)

```bash
# Drop and recreate database
dropdb stratsphere
createdb stratsphere

# Reapply migrations
dotnet ef database update --startup-project ../StratSphere.Api
```

### Migration Issues

```bash
# View pending migrations
dotnet ef migrations list

# Remove last migration
dotnet ef migrations remove

# Rebuild snapshot
dotnet ef migrations remove && dotnet ef migrations add Rebuild --startup-project ../StratSphere.Api
```

## Development vs Production

### Development

- Local PostgreSQL
- Full EF Core logging
- Hot reload enabled
- Seeding of sample data

### Production

- Remote PostgreSQL with SSL
- Minimal logging (warning level)
- Connection pooling optimized
- No data seeding
