# StratSphere — Project Architecture

## Overview
A multi-tenant web platform for hosting Strat-O-Matic baseball simulation leagues.
Built on **ASP.NET Core 10 MVC** + **PostgreSQL 16** + **HTML/CSS/Vanilla JS**.
Player data sourced from the **Lahman Baseball Database (v2025)**.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend Framework | ASP.NET Core 10 (MVC, net10.0) |
| Auth | ASP.NET Core Identity 9 |
| ORM | Entity Framework Core 9 (Npgsql 9) |
| Database | PostgreSQL 16 |
| Frontend | Razor Views + Vanilla JS |
| Styling | Custom CSS (dark theme, CSS variables) |
| Player Data | Lahman Baseball Database v2025 (CSV import) |

---

## Solution Structure

```
StratSphere.sln
├── src/
│   ├── StratSphere.Web/              # ASP.NET Core MVC — entry point
│   ├── StratSphere.Core/             # Domain entities, interfaces, services
│   └── StratSphere.Data/             # EF Core DbContext, repositories, migrations
├── tools/
│   └── StratSphere.DataImport/       # CLI: import Lahman CSVs -> PostgreSQL
└── tests/
    └── StratSphere.Tests/
```

---

## Stats Model — The Core Concept

This is the most important design principle in the system.

For any given player card (e.g., 1986 Mike Schmidt), there are **two separate stat contexts**:

```
┌─────────────────────────────────────────────────────────────────┐
│  Player Card: Mike Schmidt, 1986                                 │
│                                                                  │
│  [1] Historical Stats (Lahman)          READ-ONLY               │
│      37 HR, .290 BA, 119 RBI                                    │
│      Source of truth for the card — never changes               │
│                                                                  │
│  [2] Simulated Stats (per league season)  LEAGUE-SCOPED         │
│      League A / 2025 Season: 12 HR, .271 BA, 44 RBI            │
│      League B / 2025 Season: 29 HR, .308 BA, 88 RBI            │
│      League C / 2024 Season: 7 HR, .244 BA, 31 RBI             │
│      (Not all leagues use this card. Not all seasons are active) │
└─────────────────────────────────────────────────────────────────┘
```

**Historical stats** live in the `lahman` schema, imported from CSVs.
**Simulated stats** live in `public.sim_batting_stats` / `public.sim_pitching_stats`,
scoped to `(card_id, season_id)` — one row per card per league season.

---

## Database Architecture — Three Layers

```
PostgreSQL DB: stratsphere
│
├── lahman schema         ← Read-only. Lahman CSV import. Never touched by app.
│   ├── people            ← Player identity (playerID, name, bats/throws)
│   ├── batting           ← Real historical batting stats (1871–2025)
│   ├── pitching          ← Real historical pitching stats
│   └── fielding          ← Real historical fielding stats
│
└── public schema         ← App data. EF Core migrations manage this.
    │
    ├── [Auth]
    │   └── asp_net_users / asp_net_roles / etc.
    │
    ├── [League / Tenant]
    │   ├── leagues
    │   ├── league_members
    │   ├── seasons
    │   └── teams
    │
    ├── [Cards & Rosters]
    │   ├── player_cards     ← (lahman_player_id, card_year) — the card definition
    │   └── roster_slots     ← which card is on which team in which season
    │
    ├── [Simulated Stats]    ← Accumulated as sim games are played per league
    │   ├── sim_batting_stats    (card_id, season_id) → counting stats
    │   └── sim_pitching_stats   (card_id, season_id) → counting stats
    │
    └── [Schedule / Results]
        ├── games
        └── standings
```

---

## Full Schema

### Lahman Schema (Read-Only)

```sql
CREATE TABLE lahman.people (
  player_id    VARCHAR(9) PRIMARY KEY,   -- e.g. 'schmimi01'
  birth_year   INT,
  birth_month  INT,
  birth_day    INT,
  name_first   VARCHAR(255),
  name_last    VARCHAR(255),
  name_given   VARCHAR(255),
  bats         VARCHAR(1),
  throws       VARCHAR(1),
  debut        DATE,
  final_game   DATE,
  bbref_id     VARCHAR(255),
  retro_id     VARCHAR(255)
);

CREATE TABLE lahman.batting (
  player_id  VARCHAR(9),
  year_id    INT,
  stint      INT,
  team_id    VARCHAR(3),
  lg_id      VARCHAR(2),
  g INT, ab INT, r INT, h INT,
  doubles INT, triples INT, hr INT, rbi INT,
  sb INT, cs INT, bb INT, so INT,
  ibb INT, hbp INT, sh INT, sf INT, g_idp INT,
  PRIMARY KEY (player_id, year_id, stint)
);

CREATE TABLE lahman.pitching (
  player_id  VARCHAR(9),
  year_id    INT,
  stint      INT,
  team_id    VARCHAR(3),
  lg_id      VARCHAR(2),
  w INT, l INT, g INT, gs INT,
  cg INT, sho INT, sv INT,
  ip_outs INT,   -- outs recorded (IP * 3)
  h INT, er INT, hr INT, bb INT, so INT,
  era DECIMAL(5,2),
  PRIMARY KEY (player_id, year_id, stint)
);

CREATE TABLE lahman.fielding (
  player_id  VARCHAR(9),
  year_id    INT,
  stint      INT,
  team_id    VARCHAR(3),
  lg_id      VARCHAR(2),
  pos        VARCHAR(5),
  g INT, gs INT, inn_outs INT,
  po INT, a INT, e INT, dp INT,
  PRIMARY KEY (player_id, year_id, stint, pos)
);
```

---

### Public Schema — League / Tenant Tables

```sql
CREATE TABLE public.leagues (
  id               UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name             VARCHAR(100) NOT NULL,
  slug             VARCHAR(60) UNIQUE NOT NULL,
  commissioner_id  UUID REFERENCES asp_net_users(id),
  status           VARCHAR(20) DEFAULT 'setup',
  created_at       TIMESTAMPTZ DEFAULT now()
);

CREATE TABLE public.league_members (
  league_id  UUID REFERENCES leagues(id),
  user_id    UUID REFERENCES asp_net_users(id),
  role       VARCHAR(20) NOT NULL,   -- 'commissioner' | 'manager'
  joined_at  TIMESTAMPTZ DEFAULT now(),
  PRIMARY KEY (league_id, user_id)
);

CREATE TABLE public.seasons (
  id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  league_id  UUID REFERENCES leagues(id),
  name       VARCHAR(50),
  card_year  INT NOT NULL,           -- Strat card set year (e.g. 1986)
  status     VARCHAR(20) DEFAULT 'setup',
  start_date DATE,
  end_date   DATE
);

CREATE TABLE public.teams (
  id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  league_id     UUID REFERENCES leagues(id),
  user_id       UUID REFERENCES asp_net_users(id),
  name          VARCHAR(100),
  city          VARCHAR(100),
  abbreviation  CHAR(3),
  logo_url      VARCHAR(255)
);
```

---

### Public Schema — Cards & Rosters

```sql
-- The card definition: one unique player + season year combination.
-- Shared across all leagues — if two leagues both use 1986 Schmidt,
-- they reference the same player_cards row.
CREATE TABLE public.player_cards (
  id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  lahman_player_id  VARCHAR(9) NOT NULL,   -- soft ref to lahman.people.player_id
  card_year         INT NOT NULL,
  position          VARCHAR(5) NOT NULL,
  UNIQUE (lahman_player_id, card_year, position)
);

-- Roster assignment: which card is on which team in which season.
-- A card can appear on one team per league season (enforced in app logic).
-- Not all player_cards will appear in every season/league.
CREATE TABLE public.roster_slots (
  id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  team_id     UUID REFERENCES teams(id),
  season_id   UUID REFERENCES seasons(id),
  card_id     UUID REFERENCES player_cards(id),
  slot_type   VARCHAR(20) DEFAULT 'active',  -- 'active' | 'bench' | 'injured'
  UNIQUE (season_id, card_id)               -- a card can only be on one team per season
);
```

---

### Public Schema — Simulated Stats

```sql
-- Accumulated batting stats for a card within a specific league season.
-- Isolated per (card_id, season_id) — entirely independent across leagues.
-- Row is created when a card is first added to a roster; stats start at 0.
CREATE TABLE public.sim_batting_stats (
  id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  card_id    UUID NOT NULL REFERENCES player_cards(id),
  season_id  UUID NOT NULL REFERENCES seasons(id),
  team_id    UUID NOT NULL REFERENCES teams(id),   -- current team (can change via trade)
  g          INT DEFAULT 0,
  ab         INT DEFAULT 0,
  r          INT DEFAULT 0,
  h          INT DEFAULT 0,
  doubles    INT DEFAULT 0,
  triples    INT DEFAULT 0,
  hr         INT DEFAULT 0,
  rbi        INT DEFAULT 0,
  bb         INT DEFAULT 0,
  so         INT DEFAULT 0,
  sb         INT DEFAULT 0,
  cs         INT DEFAULT 0,
  hbp        INT DEFAULT 0,
  sf         INT DEFAULT 0,
  UNIQUE (card_id, season_id)
);

-- Accumulated pitching stats for a card within a specific league season.
CREATE TABLE public.sim_pitching_stats (
  id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  card_id    UUID NOT NULL REFERENCES player_cards(id),
  season_id  UUID NOT NULL REFERENCES seasons(id),
  team_id    UUID NOT NULL REFERENCES teams(id),
  w          INT DEFAULT 0,
  l          INT DEFAULT 0,
  g          INT DEFAULT 0,
  gs         INT DEFAULT 0,
  sv         INT DEFAULT 0,
  ip_outs    INT DEFAULT 0,   -- outs recorded (IP * 3); divide by 3 for display
  h          INT DEFAULT 0,
  er         INT DEFAULT 0,
  hr         INT DEFAULT 0,
  bb         INT DEFAULT 0,
  so         INT DEFAULT 0,
  UNIQUE (card_id, season_id)
);
```

---

### Public Schema — Schedule & Standings

```sql
CREATE TABLE public.games (
  id             UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  season_id      UUID REFERENCES seasons(id),
  home_team_id   UUID REFERENCES teams(id),
  away_team_id   UUID REFERENCES teams(id),
  home_score     INT,
  away_score     INT,
  game_date      DATE,
  status         VARCHAR(20) DEFAULT 'scheduled'  -- 'scheduled'|'completed'|'postponed'
);

CREATE TABLE public.standings (
  id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  team_id       UUID REFERENCES teams(id),
  season_id     UUID REFERENCES seasons(id),
  wins          INT DEFAULT 0,
  losses        INT DEFAULT 0,
  ties          INT DEFAULT 0,
  runs_scored   INT DEFAULT 0,
  runs_allowed  INT DEFAULT 0,
  streak        VARCHAR(10),
  last_updated  TIMESTAMPTZ DEFAULT now(),
  UNIQUE (team_id, season_id)
);
```

---

## Core Entities (StratSphere.Core)

```
Entities/
├── ApplicationUser.cs
├── League.cs
├── LeagueMember.cs
├── Season.cs
├── Team.cs
├── PlayerCard.cs                  # (LahmanPlayerId, CardYear, Position)
├── RosterSlot.cs                  # Card -> Team, Season
├── SimBattingStats.cs             # Sim stats: card + season scoped
├── SimPitchingStats.cs
├── Game.cs
├── Standings.cs
└── Lahman/
    ├── LahmanPerson.cs            # Read-only EF entity
    ├── LahmanBatting.cs
    ├── LahmanPitching.cs
    └── LahmanFielding.cs

Services/
├── LeagueService.cs
├── StandingsService.cs            # Recalculates from games
├── RosterService.cs               # Validates card uniqueness per season
├── SimStatsService.cs             # Updates sim stats when game results entered
└── PlayerCardService.cs           # Resolves card -> historical + sim stats
```

---

## Stats Service Design

`PlayerCardService` is the single place that assembles the full picture for a card:

```csharp
public class PlayerCardStats
{
    // Identity
    public string PlayerName { get; set; }
    public int CardYear { get; set; }
    public string Position { get; set; }

    // Historical stats (from lahman schema — always present)
    public LahmanBatting HistoricalBatting { get; set; }
    public LahmanPitching HistoricalPitching { get; set; }

    // Derived historical rate stats (calculated, not stored)
    public decimal? HistoricalBA  => ...   // H / AB
    public decimal? HistoricalOBP => ...   // (H + BB + HBP) / (AB + BB + HBP + SF)
    public decimal? HistoricalSLG => ...
    public decimal? HistoricalERA => ...   // (ER / IP_Outs * 27)

    // Simulated stats for THIS league season (null if card not in this league)
    public SimBattingStats? SimBatting { get; set; }
    public SimPitchingStats? SimPitching { get; set; }

    // Derived sim rate stats (calculated, not stored)
    public decimal? SimBA  => ...
    public decimal? SimOBP => ...
    public decimal? SimERA => ...
}
```

`SimStatsService.RecordGameResult(gameId)` is called after a commissioner enters a score.
For V1 this updates team-level standings only. In V2 (when per-game box scores are added),
it will also update individual sim counting stats.

---

## Player Profile Page

The player detail page shows both stat contexts side-by-side:

```
┌─────────────────────────────────────────────────────────┐
│  Mike Schmidt  •  3B  •  1986 Card                      │
│                                                          │
│  HISTORICAL (1986 season, Philadelphia Phillies)         │
│  G    AB    H    HR    RBI    BA     OBP    SLG          │
│  160  552   160   37   119  .290   .390   .547           │
│                                                          │
│  YOUR LEAGUE (Retro 1986 League • 2025 Season)           │
│  G    AB    H    HR    RBI    BA     OBP    SLG          │
│   42  151    41   12    44  .272   .374   .510           │
└─────────────────────────────────────────────────────────┘
```

---

## V1 Feature Scope

| Feature | Status |
|---|---|
| User registration & login | ✅ Done |
| Create / join leagues | ✅ Done |
| League dashboard | ✅ Done |
| Season management (commissioner creates, sets card year) | ✅ Done |
| Team management (commissioner creates, managers claim) | ✅ Done |
| Player search — Lahman AJAX typeahead | ✅ Done |
| Roster management — add/drop cards per season | ✅ Done |
| DataImport CLI (Lahman CSV → PostgreSQL) | ✅ Done |
| Score entry → standings update | 🔲 Next |
| Standings page (per season) | 🔲 Next |
| Player profile: historical stats | 🔲 Next |
| Player profile: sim stats (league-scoped) | 🔲 Next |
| Season status transitions (setup → active → complete) | 🔲 Next |
| Per-game box score entry + individual sim stat tracking | V2 |
| Draft system | V2 |
| Trade system | V2 |
| Career sim stats across seasons | V2 |

---

## Next Steps — V1 Completion

Items are ordered by dependency. Each builds on the previous.

### 1. Score Entry + Standings

**Goal:** Commissioner enters a game result; standings auto-update.

- `StandingsService.RecordGameResult(gameId)` exists — needs a UI to call it.
- `GameController` (or `SeasonController`): `POST RecordResult(Guid gameId, int homeScore, int awayScore)`
  - Updates `games` row (`home_score`, `away_score`, `status = completed`)
  - Calls `StandingsService` to recalculate W/L/RS/RA for both teams
- Season detail view: editable schedule table with score inputs
- Standings table on season/league detail page: Rank | Team | W | L | PCT | RS | RA | Streak

**Key consideration:** No schedule generator yet — for V1, the commissioner can create games manually or import a schedule. A simple `POST CreateGame(homeTeamId, awayTeamId, gameDate)` is sufficient.

---

### 2. Standings Page

**Goal:** Any league member can view the current standings for a season.

- `GET /league/{slug}/season/{id}` — season detail with standings table + recent scores
- Standings sorted by PCT descending, then RS-RA differential as tiebreaker
- `SeasonController.Detail(Guid id)` — load standings via `IStandingsRepository.GetBySeasonIdAsync`

---

### 3. Season Status Transitions

**Goal:** Prevent roster adds/score entry in wrong season states.

States: `setup` → `active` → `completed`

- Commissioner action: "Open Season" button on season detail (setup → active)
- "Close Season" button (active → completed)
- `RosterController.AddPlayer` should check `season.Status == "active"` (or allow setup too — decide)
- Score entry only allowed when `status == "active"`

---

### 4. Player Profile Page

**Goal:** Show a card's historical (Lahman) and simulated (league-scoped) stats side-by-side.

- Route: `GET /league/{slug}/player/{lahmanPlayerId}/{cardYear}`
- `PlayerCardService.GetCardStatsAsync(lahmanPlayerId, cardYear, seasonId)` — already exists
- View: two stat blocks
  - **Historical** — from `lahman.batting` / `lahman.pitching` for the card year
  - **This League** — from `sim_batting_stats` / `sim_pitching_stats` for the current season
- Link from roster table: player name → player profile

---

### 5. Sim Stat Initialization

**Goal:** When a player card is added to a roster, ensure a `sim_batting_stats` or `sim_pitching_stats` row exists at zero so the player profile page always has a sim stat row.

- `RosterService.AddCardToRosterAsync` should call `SimStatsService.EnsureRowExistsAsync(cardId, seasonId, teamId)` after creating the slot
- `SimStatsService` needs `EnsureRowExistsAsync` — INSERT ON CONFLICT DO NOTHING

---

## Recommended Build Order

1. ✅ Scaffold solution, DataImport CLI, EF Core, Auth
2. ✅ League + Team CRUD
3. ✅ Season management
4. ✅ Player search (Lahman AJAX typeahead)
5. ✅ Roster management (add/drop per season)
6. Season status transitions (setup → active)
7. Score entry + standings recalc
8. Standings page
9. Sim stat initialization on roster add
10. Player profile page (historical + sim stats)

---

## Lahman Import Instructions

```bash
# Download latest Lahman CSVs (v2025, 1871-2025) from:
# https://sabr.org/lahman-database/

cd tools/StratSphere.DataImport
dotnet run -- \
  --source /path/to/lahman-csvs \
  --connection "Host=localhost;Database=stratsphere;Username=...;Password=..."

# Truncates and reloads lahman schema only. Safe to re-run annually.
# Does NOT touch public schema app data.
```