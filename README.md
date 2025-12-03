# StratSphere

A multi-tenant SaaS platform for Strat-o-matic baseball league management. Host your league's standings, stats, rosters, and run live drafts with real-time updates.

## Features

- **Multi-League Support** - Each league operates in complete isolation with its own teams, rosters, and drafts
- **Live Drafts** - Real-time synchronous drafts with pick timers and SignalR broadcasting
- **Async Drafts** - Flexible drafting for leagues across time zones
- **Prospect Scouting** - 20-80 scale scouting reports for minor league and amateur players
- **Roster Management** - Full 40-man rosters with contract tracking
- **Standings & Stats** - Season tracking with divisional standings

## Tech Stack

- **.NET 8** - ASP.NET Core Web API
- **Entity Framework Core 8** - SQL Server with multi-tenant query filters
- **SignalR** - Real-time draft updates
- **JWT Authentication** - Secure API access with role-based authorization
- **Vanilla JS + Bootstrap 5** - Lightweight frontend UI

## Project Structure

```text
StratSphere/
├── src/
│   ├── StratSphere.Api/           # Web API + SignalR hubs
│   │   ├── Controllers/           # REST endpoints (including AuthController)
│   │   ├── Hubs/                  # SignalR draft hub
│   │   ├── Middleware/            # Tenant resolution
│   │   └── wwwroot/               # Frontend static files
│   │       ├── css/               # Custom styles
│   │       ├── js/                # App, API client, auth modules
│   │       └── index.html         # Main SPA entry point
│   ├── StratSphere.Core/          # Domain entities & business logic
│   │   ├── Entities/              # League, Team, Player, Draft, etc.
│   │   └── Enums/                 # Status codes, positions, etc.
│   ├── StratSphere.Infrastructure/# Data access & external services
│   │   ├── Data/                  # DbContext & configurations
│   │   └── Services/              # Auth, password hashing, tenant provider
│   └── StratSphere.Shared/        # DTOs & constants
│       └── DTOs/                  # Request/response models
└── tests/
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or full instance)
- IDE (Visual Studio 2022, Rider, or VS Code)

### Setup

1. **Clone and restore packages**

   ```bash
   git clone https://github.com/JimCorrell/StratSphere.git
   cd stratsphere
   dotnet restore
   ```

2. **Configure the database connection**

   Update `src/StratSphere.Api/appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=StratSphere;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Create and apply migrations**

   ```bash
   cd src/StratSphere.Infrastructure
   dotnet ef migrations add InitialCreate --startup-project ../StratSphere.Api
   dotnet ef database update --startup-project ../StratSphere.Api
   ```

4. **Run the API**

   ```bash
   cd src/StratSphere.Api
   dotnet run
   ```

5. **Access the application**

   - **Frontend**: Navigate to `httpcd ..
   ://localhost:5151`
   - **Swagger UI**: Navigate to `http://localhost:5151/swagger` to explore the API

## Key Features Usage

### Authentication

#### Register a new user

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "username": "slugger42",
  "password": "securepassword",
  "displayName": "Mike Schmidt"
}
```

#### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrUsername": "slugger42",
  "password": "securepassword"
}
```

Response includes a JWT token:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2024-03-16T19:00:00Z",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "username": "slugger42",
    "displayName": "Mike Schmidt"
  }
}
```

**Get current user's leagues** (respects admin vs regular user)

```http
GET /api/auth/me/leagues
Authorization: Bearer {token}
```

Admins see all leagues; regular users see only leagues they're a member of.

### Creating a League

```http
POST /api/leagues?creatorUserId={userId}
Content-Type: application/json

{
  "name": "East Coast Strat League",
  "description": "Competitive 16-team league since 1985",
  "maxTeams": 16,
  "rosterSize": 40,
  "activeRosterSize": 25,
  "useDH": true
}
```

### Adding Teams to a League

```http
POST /api/leagues/{leagueId}/teams?ownerId={userId}
Content-Type: application/json

{
  "name": "Brooklyn Robins",
  "abbreviation": "BKN",
  "city": "Brooklyn",
  "division": "East",
  "conference": "National"
}
```

### Setting Up a Draft

1. **Create the draft**

   ```http
   POST /api/leagues/{leagueId}/drafts
   Content-Type: application/json

   {
     "name": "2024 Rookie Draft",
     "mode": "Synchronous",
     "totalRounds": 5,
     "scheduledStartTime": "2024-03-15T19:00:00Z",
     "pickTimeLimitSeconds": 120,
     "snakeDraft": true,
     "allowTrading": true
   }
   ```

2. **Set the draft order**

   ```http
   POST /api/leagues/{leagueId}/drafts/{draftId}/order
   Content-Type: application/json

   {
     "order": [
       { "teamId": "team-guid-1", "position": 1 },
       { "teamId": "team-guid-2", "position": 2 }
     ]
   }
   ```

3. **Start the draft**

   ```http
   POST /api/leagues/{leagueId}/drafts/{draftId}/start
   ```

### Connecting to Live Draft (SignalR)

```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5151/hubs/draft")
  .withAutomaticReconnect()
  .build();

// Join a draft room
await connection.start();
await connection.invoke("JoinDraft", draftId);

// Listen for picks
connection.on("PickMade", (event) => {
  console.log(`${event.pick.playerName} selected by ${event.pick.teamName}`);
  console.log(
    `Next up: ${event.nextTeamId}, deadline: ${event.nextPickDeadline}`
  );
});

// Listen for timer updates
connection.on("TimerUpdate", (event) => {
  console.log(`${event.secondsRemaining} seconds remaining`);
});

connection.on("DraftCompleted", (event) => {
  console.log("Draft complete!");
});
```

### Making a Pick

```http
POST /api/leagues/{leagueId}/drafts/{draftId}/pick?teamId={teamId}
Content-Type: application/json

{
  "playerId": "player-guid"
}
```

### Searching for Players

```http
GET /api/players/search?searchTerm=smith&level=TripleA&position=Shortstop&availableOnly=true&leagueId={leagueId}
```

### Creating a Scouting Report

```http
POST /api/leagues/{leagueId}/scouting?scoutUserId={userId}
Content-Type: application/json

{
  "playerId": "prospect-guid",
  "hitTool": 55,
  "powerTool": 60,
  "speedTool": 50,
  "fieldingTool": 45,
  "armTool": 50,
  "overallGrade": 55,
  "potentialGrade": 60,
  "riskLevel": "Medium",
  "eta": "2025",
  "strengths": "Plus raw power, improving plate discipline",
  "weaknesses": "Swing-and-miss concerns, below average defense",
  "comparable": "Rhys Hoskins"
}
```

## Multi-Tenancy

StratSphere uses a shared database with `LeagueId` discriminators. Tenant context is resolved automatically via:

1. **Route parameter**: `/api/leagues/{leagueId}/teams`
2. **Header**: `X-League-Id: {guid}`
3. **Query string**: `?leagueId={guid}`

EF Core global query filters ensure all queries are automatically scoped to the current league.

## API Endpoints

| Resource | Endpoint                                        | Description                         |
| -------- | ----------------------------------------------- | ----------------------------------- |
| Auth     | `POST /api/auth/register`                       | Register new user                   |
| Auth     | `POST /api/auth/login`                          | Login, returns JWT                  |
| Auth     | `GET /api/auth/me`                              | Get current user info               |
| Auth     | `GET /api/auth/me/leagues`                      | Get user's leagues (admin sees all) |
| Leagues  | `GET /api/leagues`                              | List all leagues                    |
| Leagues  | `POST /api/leagues`                             | Create a league                     |
| Leagues  | `GET /api/leagues/{id}`                         | Get league details                  |
| Teams    | `GET /api/leagues/{id}/teams`                   | List teams in league                |
| Teams    | `POST /api/leagues/{id}/teams`                  | Create a team                       |
| Roster   | `GET /api/leagues/{id}/teams/{teamId}/roster`   | Get team roster                     |
| Drafts   | `GET /api/leagues/{id}/drafts`                  | List drafts                         |
| Drafts   | `POST /api/leagues/{id}/drafts`                 | Create a draft                      |
| Drafts   | `POST /api/leagues/{id}/drafts/{draftId}/start` | Start a draft                       |
| Drafts   | `POST /api/leagues/{id}/drafts/{draftId}/pick`  | Make a pick                         |
| Players  | `GET /api/players/search`                       | Search player database              |
| Scouting | `POST /api/leagues/{id}/scouting`               | Create scouting report              |

---

## Roadmap

### Phase 1: Authentication & Users ✅ Complete

- [x] JWT authentication
- [x] User registration and login endpoints
- [x] Role-based authorization (Admin flag)
- [x] Password hashing (PBKDF2)
- [ ] Refresh tokens
- [ ] Password reset flow
- [ ] Commissioner/Co-Commissioner league roles enforcement

### Phase 2: Player Data Integration

- [ ] MLB Stats API integration service
- [ ] Scheduled job for daily roster/stats updates
- [ ] Player photo URLs
- [ ] Historical stats import (career numbers)
- [ ] Minor league affiliate mapping

### Phase 3: Draft Enhancements

- [ ] Draft timer background service (auto-pick on timeout)
- [ ] Pre-draft player queue per team
- [ ] Draft pick trading
- [ ] Draft results export (CSV, PDF)
- [ ] Draft chat/comments via SignalR

### Phase 4: Season Management

- [ ] Schedule generation
- [ ] Game result entry
- [ ] Automatic standings calculation
- [ ] Playoff bracket generation
- [ ] Season history archive

### Phase 5: Transactions & Waivers

- [ ] Trade proposal system
- [ ] Trade review period
- [ ] Waiver wire with priority order
- [ ] Free agent bidding (FAAB)
- [ ] Transaction log with full audit trail

### Phase 6: Frontend Application ✅ In Progress

- [x] Vanilla JS + Bootstrap 5 UI
- [x] Login/Register modals
- [x] League list with admin visibility
- [x] League detail view with tabs
- [x] Mobile-responsive design
- [ ] Real-time draft board UI
- [ ] Player search with filters
- [ ] Team roster management UI

### Phase 7: Advanced Features

- [ ] Strat-o-matic card data integration
- [ ] Salary cap management
- [ ] Keeper/dynasty league support
- [ ] League constitution builder
- [ ] Commissioner tools (force trades, adjust rosters)
- [ ] League activity feed

### Phase 8: Infrastructure & Scale

- [ ] Redis caching for player data
- [ ] Background job processing (Hangfire)
- [ ] Multi-region deployment
- [ ] API rate limiting
- [ ] Comprehensive logging (Serilog + Seq)
- [ ] Health checks and monitoring

---

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

_Built for the love of the game_ ⚾
