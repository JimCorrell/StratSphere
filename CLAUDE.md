# StratSphere — Architecture Decision Log

| Date | Decision | Rationale |
|---|---|---|
| — | Rate stats (BA, OBP, SLG, ERA, WHIP) never persisted | Derived stats go stale; compute at query/display time instead |
| — | Dual-schema: `public` (EF-managed) + `lahman` (read-only import) | Clean boundary between app data and historical source |
| — | Repository pattern; no DbContext in controllers | Thin controllers; testable without real DB |
| 2026-03 | `LeagueStatus` as C# enum, stored as string via EF value converter | Type safety in code; readable in DB; case-insensitive reader handles legacy rows |
| 2026-03 | Roster soft-delete via `AcquiredAt`/`DroppedAt` + partial unique index | Re-acquisition within a season; history preserved; one active slot per card enforced at DB level |
| 2026-03 | `PlayerCard.GetOrCreateAsync` catches `DbUpdateException` + re-fetches | Optimistic concurrency for duplicate card insert; avoids serializable transaction |
| 2026-03 | `MigrateAsync()` on startup | Single-instance deploy; idempotent; no separate migration step in deployment |
| 2026-03 | Tests use EF InMemory; no real DB in CI | Business logic covered; real Postgres integration deferred |

---

## Non-obvious gotchas

**EF migrations — watch for spurious column renames.** If the model snapshot is stale, EF will generate rename operations for columns that are already correct in the DB. Always review the full migration diff before applying.

**EF enum-to-string:** Use `HasDefaultValueSql("'Value'")` not `HasDefaultValue("Value")` when the property type and column type differ. `HasDefaultValue` throws at model build time.

**Routing — GUID in 4th URL segment.** The generic slug route parses anything in position 4 as an action name. Any route where a GUID lands there needs an explicit route with `:guid` constraint registered before the slug route in `Program.cs`.
