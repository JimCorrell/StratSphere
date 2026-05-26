// app.jsx — root StratSphere prototype

const TWEAK_DEFAULTS = /*EDITMODE-BEGIN*/{
  "theme": "paper",
  "accent": "#8f2727",
  "density": "comfortable",
  "commish": false
}/*EDITMODE-END*/;

// Curated accent swatches (vintage-leaning team palettes)
const ACCENT_SWATCHES = ["#8f2727", "#1f3a5c", "#2d5b34", "#c89a1f", "#6e1c1c", "#3b7a6b", "#7a4a1e", "#1b1815"];

function StratSphereApp() {
  const [t, setTweak] = useTweaks(TWEAK_DEFAULTS);
  const [signedIn, setSignedIn] = React.useState(false);
  const [leagueId, setLeagueId] = React.useState("tbl");
  const [page, setPage] = React.useState("home");
  const [teamId, setTeamId] = React.useState("brk");

  const league = LEAGUES.find(L => L.id === leagueId) || LEAGUES[0];

  // Apply theme + density to root
  React.useEffect(() => {
    const themeAttr = t.theme === "dark" ? "dark" : t.theme === "scoreboard" ? "scoreboard" : "paper";
    document.documentElement.setAttribute("data-theme", themeAttr);
    document.documentElement.setAttribute("data-density", t.density);
    document.documentElement.style.setProperty("--accent", t.accent);
    // pick legible ink color for the accent
    const ink = pickInk(t.accent);
    document.documentElement.style.setProperty("--accent-ink", ink);
  }, [t.theme, t.density, t.accent]);

  function navigate(p, id) {
    if (p === "team" && id) setTeamId(id);
    setPage(p);
    window.scrollTo({ top: 0, behavior: "instant" });
  }

  if (!signedIn) {
    return (
      <>
        <LoginPage
          leagues={LEAGUES}
          setLeague={setLeagueId}
          onEnter={() => setSignedIn(true)}
        />
        <TweaksUI t={t} setTweak={setTweak} />
      </>
    );
  }

  return (
    <div className="app">
      <TopStrip
        league={league}
        setLeague={setLeagueId}
        leagues={LEAGUES}
        commish={t.commish}
        setCommish={(v) => setTweak("commish", v)}
        onLogout={() => setSignedIn(false)}
      />
      {t.commish && <CommishBar league={league} />}
      <Masthead league={league} />
      <PrimaryNav page={page} setPage={(p) => navigate(p)} />
      <div className="page">
        {page === "home" && <HomePage league={league} navigate={navigate} />}
        {page === "team" && <TeamPage teamId={teamId} navigate={navigate} />}
        {page === "player" && <PlayerPage navigate={navigate} />}
        {page === "standings" && <StandingsPage navigate={navigate} />}
        {(page === "stats" || page === "wire" || page === "history") && (
          <div style={{ padding: 80, textAlign: "center", fontFamily: "var(--f-display)", fontSize: 28, color: "var(--ink-3)", textTransform: "uppercase", letterSpacing: ".06em" }}>
            {page} · Front-office page coming soon
          </div>
        )}
      </div>
      <Footer league={league} />
      <TweaksUI t={t} setTweak={setTweak} />
    </div>
  );
}

function TweaksUI({ t, setTweak }) {
  return (
    <TweaksPanel title="Tweaks">
      <TweakSection label="Theme" />
      <TweakRadio
        label="Mode"
        value={t.theme}
        options={[
          { value: "paper", label: "Paper" },
          { value: "dark", label: "Night" },
          { value: "scoreboard", label: "Scoreboard" },
        ]}
        onChange={(v) => setTweak("theme", v)}
      />
      <TweakColor
        label="Accent / team color"
        value={t.accent}
        options={ACCENT_SWATCHES}
        onChange={(v) => setTweak("accent", v)}
      />
      <TweakSection label="Layout" />
      <TweakRadio
        label="Density"
        value={t.density}
        options={["compact", "comfortable"]}
        onChange={(v) => setTweak("density", v)}
      />
      <TweakSection label="Role" />
      <TweakToggle
        label="Commissioner mode"
        value={t.commish}
        onChange={(v) => setTweak("commish", v)}
      />
      <div style={{ fontSize: 10.5, color: "rgba(41,38,27,.6)", padding: "0 2px", letterSpacing: ".06em", lineHeight: 1.4 }}>
        Enables the league-office bar at the top with schedule, transactions, and "advance day" controls.
      </div>
    </TweaksPanel>
  );
}

// helper: choose ink (white/black) for legibility against a bg
function pickInk(hex) {
  if (!hex || hex[0] !== "#") return "#fff";
  const h = hex.replace("#", "");
  const r = parseInt(h.slice(0, 2), 16);
  const g = parseInt(h.slice(2, 4), 16);
  const b = parseInt(h.slice(4, 6), 16);
  const luma = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
  return luma > 0.62 ? "#1b1815" : "#f6ecd2";
}

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(<StratSphereApp />);
