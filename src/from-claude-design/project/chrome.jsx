// chrome.jsx — top strip (multi-tenant), masthead, primary nav, footer

function TeamChip({ team, size = "" }) {
  if (!team) return null;
  return (
    <span
      className={"team-chip " + size}
      style={{ background: team.color, color: team.ink, borderColor: team.color }}
      title={team.city + " " + team.nick}
    >
      {team.mono}
    </span>
  );
}

function TenantSwitcher({ league, setLeague, leagues }) {
  const [open, setOpen] = React.useState(false);
  const ref = React.useRef(null);
  React.useEffect(() => {
    function onDoc(e) { if (ref.current && !ref.current.contains(e.target)) setOpen(false); }
    document.addEventListener("mousedown", onDoc);
    return () => document.removeEventListener("mousedown", onDoc);
  }, []);
  return (
    <div className="tenant-switcher no-sel" ref={ref} onClick={() => setOpen(o => !o)}>
      <span className="tenant-mark">{league.mark}</span>
      <span>
        <div className="tenant-name">{league.name}</div>
        <div className="tenant-meta">{league.year} · {league.teamCount} clubs</div>
      </span>
      <span className="tenant-caret">▼</span>
      {open && (
        <div className="tenant-menu" onClick={e => e.stopPropagation()}>
          <div className="tenant-menu-hdr">Your Leagues</div>
          {leagues.map(L => (
            <div key={L.id} className="tenant-row" onClick={() => { setLeague(L.id); setOpen(false); }}>
              <span className="tenant-mark" style={{ width: 30, height: 30, flex: "0 0 30px", fontSize: 16 }}>{L.mark}</span>
              <div className="tenant-row-body">
                <b>{L.name}</b>
                <div>{L.abbr} · {L.year} · {L.teamCount} clubs</div>
              </div>
              {L.id === league.id && <span className="check">✓</span>}
            </div>
          ))}
          <div className="tenant-menu-foot">
            <button>+ New League</button>
            <button>Browse Public</button>
            <button style={{ marginLeft: "auto" }}>Settings</button>
          </div>
        </div>
      )}
    </div>
  );
}

function TopStrip({ league, setLeague, leagues, commish, setCommish, onLogout }) {
  return (
    <div className="top-strip no-sel">
      <TenantSwitcher league={league} setLeague={setLeague} leagues={leagues} />
      <div className="top-strip-spacer" />
      <div className="top-strip-meta">
        <span><span className="live-dot" />3 GAMES LIVE</span>
        <span>{league.currentDay || "Day 102 of 162"}</span>
        <span>SIM ENGINE: <b>v4.2</b></span>
        <span style={{ cursor: "default" }} onClick={() => setCommish(!commish)} title="Toggle commissioner mode">
          {commish ? "◉" : "○"} COMMISH
        </span>
        <span>NEXT TICK <b>22:00 ET</b></span>
        <span style={{ cursor: "default" }} onClick={onLogout}>↪ SIGN OUT</span>
      </div>
    </div>
  );
}

function CommishBar({ league }) {
  return (
    <div className="commish-bar no-sel">
      <span>⚙</span>
      <b>Commissioner Console</b>
      <span>· {league.name} · Sim paused for owner approvals · 4 pending transactions</span>
      <div className="commish-actions">
        <button>Schedule</button>
        <button>Transactions ⓸</button>
        <button>Park Factors</button>
        <button>Advance Day</button>
      </div>
    </div>
  );
}

function Masthead({ league }) {
  return (
    <div className="masthead no-sel">
      <div className="masthead-logo">
        <div className="masthead-crest">{league.mark}</div>
        <div className="masthead-text">
          <h1>{league.name}</h1>
          <div className="sub">{league.year} · {league.established || "EST. 2019"}</div>
        </div>
      </div>
      <div className="masthead-right">
        <div className="masthead-stat"><div className="lbl">Today</div><div className="val">{league.gamesToday || 8}</div></div>
        <div className="masthead-stat"><div className="lbl">Day</div><div className="val">102</div></div>
        <div className="masthead-stat"><div className="lbl">Sim Rate</div><div className="val">1×/24h</div></div>
        <div className="masthead-stat"><div className="lbl">Standings</div><div className="val">.500+</div></div>
      </div>
    </div>
  );
}

function PrimaryNav({ page, setPage }) {
  const items = [
    { id: "home",      label: "Scoreboard" },
    { id: "standings", label: "Standings" },
    { id: "team",      label: "Teams" },
    { id: "player",    label: "Players" },
    { id: "stats",     label: "Stats", soft: true },
    { id: "wire",      label: "Trade Wire", soft: true },
    { id: "history",   label: "Almanac", soft: true },
  ];
  return (
    <div className="primary-nav no-sel">
      {items.map(it => (
        <div
          key={it.id}
          className={"nv " + (page === it.id ? "active" : "")}
          onClick={() => !it.soft && setPage(it.id)}
          style={it.soft ? { opacity: .55 } : null}
        >
          {page === it.id && <span className="dot">●</span>}{it.label}
        </div>
      ))}
      <div className="primary-nav-right">
        <div className="search-pill">
          <span>🔍</span> Search players, teams, news <kbd>⌘K</kbd>
        </div>
      </div>
    </div>
  );
}

function Footer({ league }) {
  return (
    <div className="footer no-sel">
      <span className="brand-tag">★ StratSphere</span>
      <span>{league.name} · {league.year}</span>
      <span className="spacer"></span>
      <span>Sim Engine v4.2.1</span>
      <span>Park Factors · Glossary · Changelog</span>
      <span>© 2026 StratSphere Co. · A multi-tenant sim platform</span>
    </div>
  );
}

Object.assign(window, { TeamChip, TenantSwitcher, TopStrip, CommishBar, Masthead, PrimaryNav, Footer });
