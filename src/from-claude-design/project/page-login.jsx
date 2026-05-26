// page-login.jsx — Login + tenant switcher landing screen

function LoginPage({ onEnter, leagues, setLeague }) {
  const [step, setStep] = React.useState("signin"); // signin | leagues
  const [email, setEmail] = React.useState("commissioner@royals.tbl");
  const [pw, setPw] = React.useState("•••••••••");
  const [sel, setSel] = React.useState("tbl");

  return (
    <div className="login-shell" data-screen-label="00 Login & League Picker">
      <div className="login-art">
        <div className="brand"><span className="star">★</span> StratSphere</div>
        <h1>
          Run the<br/>
          <em>league</em><br/>
          of your<br/>
          dreams.
        </h1>
        <div className="tag">
          A multi-tenant home for stat-engine baseball — replays, dynasties, and dice-and-card leagues, all in one office. Built for commissioners, owners, and the chronically obsessed.
        </div>
        <div className="stats-strip">
          <div className="it"><div className="lbl">Active Leagues</div><div className="val">1,284</div></div>
          <div className="it"><div className="lbl">Sims / Day</div><div className="val">42,180</div></div>
          <div className="it"><div className="lbl">Commissioners</div><div className="val">3,602</div></div>
          <div className="it"><div className="lbl">Hot Stove</div><div className="val" style={{ color: "var(--mustard)" }}>OPEN</div></div>
        </div>
      </div>

      <div className="login-form">
        {step === "signin" && (
          <div>
            <div className="eyebrow">Member sign-in · ★</div>
            <h2>Step into the<br/>front office.</h2>
            <p className="lead">Sign in to your StratSphere account to manage your leagues, set lineups, and run today’s slate of games.</p>

            <label>Email</label>
            <input type="email" value={email} onChange={e => setEmail(e.target.value)} />
            <label>Password</label>
            <input type="password" value={pw} onChange={e => setPw(e.target.value)} />

            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginTop: 14, fontFamily: "var(--f-sans)", fontSize: 11 }}>
              <label style={{ display: "flex", alignItems: "center", gap: 6, margin: 0, letterSpacing: "0", textTransform: "none", fontWeight: 500, color: "var(--ink-2)" }}>
                <input type="checkbox" defaultChecked /> Keep me signed in
              </label>
              <a style={{ color: "var(--accent)", fontWeight: 700, letterSpacing: ".12em", textTransform: "uppercase", fontSize: 10 }}>Forgot password</a>
            </div>

            <div style={{ marginTop: 22, display: "flex", gap: 10 }}>
              <button className="btn primary" style={{ flex: 1, justifyContent: "center" }} onClick={() => setStep("leagues")}>Sign in →</button>
              <button className="btn ghost">Use SSO</button>
            </div>

            <hr className="dash-rule" style={{ margin: "26px 0" }} />

            <div style={{ display: "flex", gap: 10 }}>
              <button className="btn ghost" style={{ flex: 1, justifyContent: "center" }}>Create Account</button>
              <button className="btn ghost" style={{ flex: 1, justifyContent: "center" }}>Browse Public Leagues</button>
            </div>

            <div style={{ marginTop: 28, fontFamily: "var(--f-sans)", fontSize: 10.5, color: "var(--ink-3)", letterSpacing: ".08em" }}>
              By signing in you agree to the <a style={{ color: "var(--ink)", textDecoration: "underline" }}>StratSphere Terms</a> and acknowledge the use of cookies for live sim state.
            </div>
          </div>
        )}

        {step === "leagues" && (
          <div>
            <div className="eyebrow">Welcome back · Howard Linnell</div>
            <h2>Choose your<br/>league office.</h2>
            <p className="lead">You belong to {leagues.length} leagues. Pick one to enter the front office — you can switch any time from the top-left.</p>

            <div className="tenant-picker" style={{ marginTop: 18 }}>
              {leagues.map(L => (
                <div
                  key={L.id}
                  className={"tenant-pick " + (sel === L.id ? "sel" : "")}
                  onClick={() => setSel(L.id)}
                >
                  <span className="mark">{L.mark}</span>
                  <div className="body">
                    <b>{L.name}</b>
                    <div>{L.abbr} · {L.year}</div>
                    <div style={{ marginTop: 4 }}>{L.teamCount} clubs · {L.gamesToday || 0} games today</div>
                  </div>
                </div>
              ))}
              <div className="tenant-pick" style={{ borderStyle: "dashed", borderColor: "var(--rule-strong)" }}>
                <span className="mark" style={{ background: "var(--paper-2)", color: "var(--ink)" }}>+</span>
                <div className="body">
                  <b>Start a new league</b>
                  <div>Replay, dynasty, draft-and-keep</div>
                  <div style={{ marginTop: 4, color: "var(--accent)", letterSpacing: ".1em", textTransform: "uppercase", fontWeight: 700 }}>14-day free trial →</div>
                </div>
              </div>
              <div className="tenant-pick" style={{ borderStyle: "dashed", borderColor: "var(--rule-strong)" }}>
                <span className="mark" style={{ background: "var(--paper-2)", color: "var(--ink)" }}>↪</span>
                <div className="body">
                  <b>Join with code</b>
                  <div>Commissioner invitation</div>
                  <div style={{ marginTop: 4, fontFamily: "var(--f-mono)", color: "var(--ink-3)" }}>e.g. HEARTLAND-7K2D</div>
                </div>
              </div>
            </div>

            <div style={{ marginTop: 22, display: "flex", gap: 10 }}>
              <button className="btn ghost" onClick={() => setStep("signin")}>← Back</button>
              <button className="btn primary" style={{ flex: 1, justifyContent: "center" }} onClick={() => { setLeague(sel); onEnter(); }}>
                Enter {(leagues.find(L => L.id === sel) || {}).name || "League"} →
              </button>
            </div>

            <div style={{ marginTop: 24, fontFamily: "var(--f-sans)", fontSize: 10.5, color: "var(--ink-3)", letterSpacing: ".12em", textTransform: "uppercase", fontWeight: 700 }}>
              Tip: switch leagues any time from the top-left workspace pill.
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

Object.assign(window, { LoginPage });
