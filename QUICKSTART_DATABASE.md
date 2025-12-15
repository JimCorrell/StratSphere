# Quick Start: PostgreSQL Database

## TL;DR

Your app now uses a single **PostgreSQL database** for everything—no more multi-database complexity.

```bash
# 1. Start PostgreSQL (Docker)
docker run --name stratsphere-db -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=stratsphere -p 5432:5432 -d postgres:latest

# 2. Run the app
cd src/StratSphere.Api
dotnet run

# 3. App automatically initializes the database and seeds sample data
```

## Testing

```bash
# Search players
curl http://localhost:5151/api/player-database/search?searchTerm=ruth

# Hall of Famers
curl http://localhost:5151/api/player-database/hall-of-fame

# Player details
curl http://localhost:5151/api/player-database/1
```

## Configuration

Update `src/StratSphere.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=stratsphere;Username=postgres;Password=postgres;Port=5432"
  }
}
```

For production:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-db.com;Database=stratsphere;Username=app_user;Password=SECURE;Port=5432;SSL Mode=Require"
  }
}
```

## Setting Up PostgreSQL

### Docker (Recommended)

```bash
docker run --name stratsphere-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=stratsphere \
  -p 5432:5432 \
  -d postgres:latest
```

### macOS with Homebrew

```bash
brew install postgresql
brew services start postgresql
createdb stratsphere
```

## Database Details

- **Single Database**: All league and player data in one PostgreSQL instance
- **Automatic Setup**: App creates tables and seeds data on first run
- **Multi-tenant**: All data scoped by LeagueId
- **Production Ready**: Scales from development to enterprise

## Troubleshooting

```bash
# Check PostgreSQL is running
psql -h localhost -U postgres -d stratsphere

# Reset database (dev only)
dropdb stratsphere && createdb stratsphere

# See database schema
\dt

# Exit psql
\q
```

## Documentation

- `DATABASE.md` - Comprehensive guide
- `DATABASE_IMPLEMENTATION.md` - Architecture details
- `README.md` - General setup
