// page-player.jsx — Player profile (Whitey Sutter, BRK CF)

function PlayerBigCard() {
  const team = teamById("brk");
  return (
    <div className="bigcard" style={{ boxShadow: `6px 6px 0 ${team.color}`, gridTemplateColumns: "150px 1fr auto" }}>
      {/* Stylized "headshot" — vintage card art */}
      <div style={{ width: 130, height: 160, border: "2px solid var(--ink)", background: team.color, color: team.ink, display: "grid", placeItems: "center", position: "relative", boxShadow: "inset 0 0 0 6px " + team.ink + ", inset 0 0 0 8px " + team.color }}>
        <span style={{ fontFamily: "var(--f-display)", fontWeight: 900, fontSize: 84, lineHeight: 1, letterSpacing: "-0.03em" }}>9</span>
        <span style={{ position: "absolute", bottom: 6, left: 8, fontFamily: "var(--f-sans)", fontSize: 9, letterSpacing: ".14em", textTransform: "uppercase", color: team.ink, opacity: .8 }}>BROOKLYN · CF</span>
      </div>
      <div>
        <div className="eyebrow" style={{ marginBottom: 6 }}>Brooklyn Royals · Center Field · #9</div>
        <h2 style={{ fontSize: 56, lineHeight: .88 }}>Whitey<br/>Sutter</h2>
        <div style={{ marginTop: 12, display: "flex", gap: 22, fontFamily: "var(--f-sans)", fontSize: 12, color: "var(--ink-3)", letterSpacing: ".08em", textTransform: "uppercase", fontWeight: 600 }}>
          <span>B/T: <b style={{ color: "var(--ink)" }}>L / L</b></span>
          <span>Age: <b style={{ color: "var(--ink)" }}>27</b></span>
          <span>HT/WT: <b style={{ color: "var(--ink)" }}>6'1" / 195</b></span>
          <span>Born: <b style={{ color: "var(--ink)" }}>Spokane, WA</b></span>
          <span>Acquired: <b style={{ color: "var(--ink)" }}>R1 #4 (2023)</b></span>
        </div>
      </div>
      <div className="stats">
        <div><div className="lbl">AVG</div><div className="val" style={{ color: "var(--accent)" }}>.341</div></div>
        <div><div className="lbl">HR</div><div className="val">22</div></div>
        <div><div className="lbl">RBI</div><div className="val">71</div></div>
        <div><div className="lbl">OPS</div><div className="val">.984</div></div>
        <div><div className="lbl">WAR</div><div className="val" style={{ color: "var(--accent)" }}>5.6</div></div>
      </div>
    </div>
  );
}

function PlayerOverview() {
  return (
    <div className="col-2">
      <div>
        <div className="card lift">
          <div className="card-hdr"><h3>2026 Stat Line</h3><span className="stamp">Through Day 102</span></div>
          <div className="statgrid" style={{ border: 0 }}>
            {[
              ["G", "102"], ["AB", "401"], ["R", "78", true], ["H", "137", true],
              ["2B", "26"], ["3B", "7", true], ["HR", "22"], ["RBI", "71"],
              ["BB", "58"], ["K", "73"], ["SB", "18"], ["AVG", ".341", true],
              ["OBP", ".421", true], ["SLG", ".563"], ["OPS", ".984", true], ["wRC+", "168", true],
              ["WAR", "5.6", true], ["UZR", "+8.4"],
            ].map((c, i) => (
              <div key={i} className="cell" style={{ borderRight: (i + 1) % 6 === 0 ? "0" : "1px dashed var(--rule)" }}>
                <div className="lbl">{c[0]}</div>
                <div className={"val " + (c[2] ? "lead" : "")}>{c[1]}</div>
              </div>
            ))}
          </div>
        </div>

        <div style={{ height: 18 }} />

        <div className="card lift">
          <div className="card-hdr"><h3>Game Log · Last 8</h3><span className="stamp">June</span></div>
          <table className="roster">
            <thead>
              <tr>
                <th className="l">Date</th><th className="l">Opp</th><th className="l">Result</th>
                <th>AB</th><th>H</th><th>R</th><th>HR</th><th>RBI</th><th>BB</th><th>K</th><th>AVG</th>
              </tr>
            </thead>
            <tbody>
              {SUTTER_GAMELOG.map((g, i) => {
                const win = g.res.startsWith("W");
                return (
                  <tr key={i}>
                    <td className="l" style={{ fontFamily: "var(--f-mono)" }}>{g.date}</td>
                    <td className="l" style={{ fontFamily: "var(--f-display)", fontWeight: 700, letterSpacing: ".03em", textTransform: "uppercase" }}>{g.opp}</td>
                    <td className="l" style={{ fontFamily: "var(--f-display)", fontWeight: 800, textTransform: "uppercase", color: win ? "var(--infield)" : "var(--oxblood)" }}>{g.res}</td>
                    <td>{g.ab}</td><td style={{ fontWeight: 700 }}>{g.h}</td><td>{g.r}</td><td>{g.hr}</td><td>{g.rbi}</td><td>{g.bb}</td><td>{g.k}</td>
                    <td style={{ fontWeight: 700 }}>{g.avg}</td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>

      <div>
        <div className="card lift">
          <div className="card-hdr"><h3>Career</h3><span className="stamp">3 Seasons</span></div>
          <table className="roster">
            <thead>
              <tr><th className="l">Yr</th><th className="l">Tm</th><th>G</th><th>AB</th><th>H</th><th>HR</th><th>RBI</th><th>AVG</th><th>OPS</th><th>WAR</th></tr>
            </thead>
            <tbody>
              {SUTTER_CAREER.map((r, i) => (
                <tr key={i}>
                  <td className="l" style={{ fontFamily: "var(--f-mono)", fontWeight: 700 }}>{r.yr}</td>
                  <td className="l"><TeamChip team={teamById("brk")} /></td>
                  <td>{r.g}</td><td>{r.ab}</td><td>{r.h}</td><td>{r.hr}</td><td>{r.rbi}</td>
                  <td style={{ fontWeight: 700 }}>{r.avg}</td>
                  <td style={{ fontWeight: 700 }}>{r.ops}</td>
                  <td style={{ fontWeight: 800, color: "var(--accent)" }}>{r.war}</td>
                </tr>
              ))}
              <tr style={{ borderTop: "2px solid var(--ink)" }}>
                <td className="l" colSpan={2} style={{ fontFamily: "var(--f-display)", textTransform: "uppercase", fontWeight: 800 }}>CAREER</td>
                <td>392</td><td>1531</td><td>486</td><td>64</td><td>228</td>
                <td style={{ fontWeight: 700 }}>.317</td>
                <td style={{ fontWeight: 700 }}>.925</td>
                <td style={{ fontWeight: 800, color: "var(--accent)" }}>15.1</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div style={{ height: 18 }} />

        <div className="card lift">
          <div className="card-hdr"><h3>Hot Zone</h3><span className="stamp">vs RHP · 2026</span></div>
          <div style={{ padding: 16, display: "grid", gridTemplateColumns: "1fr auto", gap: 18, alignItems: "center" }}>
            {/* 9-zone strike zone heatmap */}
            <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 2, width: 180, height: 220, border: "2px solid var(--ink)", padding: 2 }}>
              {[
                [".289", ".341", ".268"],
                [".352", ".418", ".305"],
                [".272", ".334", ".259"],
              ].flat().map((v, i) => {
                const num = parseFloat(v);
                const hot = num > .35, warm = num > .30;
                return (
                  <div key={i} style={{
                    display: "grid", placeItems: "center",
                    background: hot ? "var(--oxblood)" : warm ? "var(--mustard)" : "var(--paper-3)",
                    color: hot ? "var(--paper)" : "var(--ink)",
                    fontFamily: "var(--f-mono)", fontSize: 12, fontWeight: 700,
                  }}>{v}</div>
                );
              })}
            </div>
            <div style={{ fontFamily: "var(--f-sans)", fontSize: 11.5, color: "var(--ink-3)", maxWidth: 220, lineHeight: 1.5 }}>
              Crushes inside heaters. Cold low-and-away — opponents are pitching there <b style={{ color: "var(--ink)" }}>34%</b> of the time, up from 21% in 2025.
            </div>
          </div>
        </div>

        <div style={{ height: 18 }} />

        <div className="card lift">
          <div className="card-hdr"><h3>Awards & Honors</h3><span className="stamp">TBL</span></div>
          <div style={{ padding: "10px 16px" }}>
            {[
              { yr: "2026", title: "All-Star · CF (Starter)", note: "Through midseason vote" },
              { yr: "2026", title: "Player of the Week × 3",  note: "Wks 6, 10, 12" },
              { yr: "2025", title: "Silver Bat · CF", note: "Led league in OBP" },
              { yr: "2025", title: "Royals Team MVP", note: "Voted by Brooklyn beat" },
              { yr: "2024", title: "Rookie of the Year · 2nd", note: "Behind Buster Lamont" },
            ].map((a, i, all) => (
              <div key={i} style={{ display: "grid", gridTemplateColumns: "48px 1fr", gap: 14, padding: "10px 0", borderBottom: i < all.length - 1 ? "1px dashed var(--rule)" : 0 }}>
                <div style={{ fontFamily: "var(--f-display)", fontSize: 22, fontWeight: 900, color: "var(--accent)" }}>{a.yr}</div>
                <div>
                  <div style={{ fontFamily: "var(--f-display)", fontWeight: 700, fontSize: 15, textTransform: "uppercase", letterSpacing: ".02em" }}>{a.title}</div>
                  <div style={{ fontFamily: "var(--f-sans)", fontSize: 11, color: "var(--ink-3)" }}>{a.note}</div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}

function PlayerPage({ navigate }) {
  const [tab, setTab] = React.useState("Overview");
  const tabs = ["Overview", "Splits", "Game Log", "Career", "Scouting", "News"];
  return (
    <div data-screen-label="03 Player — Whitey Sutter">
      <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 14, fontFamily: "var(--f-sans)", fontSize: 11, letterSpacing: ".14em", textTransform: "uppercase", fontWeight: 700, color: "var(--ink-3)" }}>
        <a onClick={() => navigate("home")} style={{ cursor: "default" }}>Scoreboard</a>
        <span>›</span>
        <a onClick={() => navigate("team", "brk")} style={{ cursor: "default" }}>Brooklyn Royals</a>
        <span>›</span>
        <span style={{ color: "var(--ink)" }}>Whitey Sutter</span>
      </div>
      <PlayerBigCard />
      <div style={{ marginTop: 18 }} className="inner-tabs">
        {tabs.map(t => (
          <button key={t} className={tab === t ? "on" : ""} onClick={() => setTab(t)}>{t}</button>
        ))}
        <div style={{ marginLeft: "auto", display: "flex", gap: 8, alignItems: "center", padding: "5px 0" }}>
          <button className="btn sm">Compare</button>
          <button className="btn sm">Watch</button>
          <button className="btn sm primary">Offer Trade</button>
        </div>
      </div>
      <div style={{ marginTop: 18 }}>
        <PlayerOverview />
      </div>
    </div>
  );
}

Object.assign(window, { PlayerPage });
