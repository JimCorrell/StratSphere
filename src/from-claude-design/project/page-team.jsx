// page-team.jsx — Team page with inner tabs

function TeamBigCard({ team }) {
  return (
    <div className="bigcard" style={{ boxShadow: `6px 6px 0 ${team.color}` }}>
      <span
        className="team-chip huge"
        style={{ background: team.color, color: team.ink, borderColor: team.color }}
      >{team.mono}</span>
      <div>
        <span className="city">{team.city.toUpperCase()}</span>
        <h2>{team.nick}</h2>
        <div className="est">EST. 1903 · TWILIGHT BASEBALL LEAGUE · {team.div.toUpperCase()} DIVISION · MGR. SHEA O'CONNOR</div>
      </div>
      <div className="stats">
        <div><div className="lbl">Record</div><div className="val">{team.w}-{team.l}</div></div>
        <div><div className="lbl">PCT</div><div className="val">{team.pct}</div></div>
        <div><div className="lbl">Run Diff</div><div className="val">{team.rd}</div></div>
        <div><div className="lbl">Streak</div><div className="val" style={{ color: team.streak[0] === "W" ? "var(--infield)" : "var(--oxblood)" }}>{team.streak}</div></div>
      </div>
    </div>
  );
}

function RosterTab({ team }) {
  const [sort, setSort] = React.useState("war");
  const [filter, setFilter] = React.useState("ALL");
  const hitters = BRK_ROSTER.filter(r => r.pos !== "SP" && r.pos !== "RP");
  const pitchers = BRK_ROSTER.filter(r => r.pos === "SP" || r.pos === "RP");

  function FilterChips() {
    const items = ["ALL", "LINEUP", "BENCH", "ROTATION", "BULLPEN", "INJURED"];
    return (
      <div style={{ display: "flex", gap: 6, marginBottom: 10 }}>
        {items.map(it => (
          <span key={it} className={"pill " + (filter === it ? "ink" : "outline")} onClick={() => setFilter(it)}>
            {it}
          </span>
        ))}
      </div>
    );
  }

  return (
    <div>
      <FilterChips />
      <div className="col-2" style={{ gridTemplateColumns: "1.4fr 1fr" }}>
        <div className="card lift">
          <div className="card-hdr">
            <h3>Position Players</h3>
            <span className="stamp">{hitters.length} ACTIVE · Sort by WAR</span>
          </div>
          <table className="roster">
            <thead>
              <tr>
                <th className="l">#</th>
                <th className="l" style={{ paddingLeft: 0 }}>Player</th>
                <th className="l">Pos</th>
                <th>G</th><th>AB</th><th>H</th><th>HR</th><th>RBI</th>
                <th>AVG</th><th>OBP</th><th>SLG</th><th>OPS</th>
                <th className="sort-active">WAR</th>
              </tr>
            </thead>
            <tbody>
              {hitters.map(p => (
                <tr key={p.num}>
                  <td className="num0 l">{p.num}</td>
                  <td className="l">
                    <span className="player-cell">
                      <TeamChip team={team} />
                      <span style={{ fontFamily: "var(--f-display)", textTransform: "uppercase", fontWeight: 700, letterSpacing: ".02em" }}>{p.name}</span>
                    </span>
                  </td>
                  <td className="l"><span className="pos-pill">{p.pos}</span></td>
                  <td>{p.g}</td><td>{p.ab}</td><td>{p.h}</td><td>{p.hr}</td><td>{p.rbi}</td>
                  <td>{p.avg}</td><td>{p.obp}</td><td>{p.slg}</td>
                  <td style={{ fontWeight: 700 }}>{p.ops}</td>
                  <td style={{ fontWeight: 800, color: "var(--accent)" }}>{p.war}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="card lift">
          <div className="card-hdr">
            <h3>Pitchers</h3>
            <span className="stamp">{pitchers.length} ACTIVE</span>
          </div>
          <table className="roster">
            <thead>
              <tr>
                <th className="l">#</th>
                <th className="l" style={{ paddingLeft: 0 }}>Pitcher</th>
                <th className="l">Role</th>
                <th>W</th><th>L</th><th>ERA</th><th>WHIP</th><th>K</th><th>IP</th>
              </tr>
            </thead>
            <tbody>
              {pitchers.map(p => (
                <tr key={p.num}>
                  <td className="num0 l">{p.num}</td>
                  <td className="l">
                    <span className="player-cell">
                      <TeamChip team={team} />
                      <span style={{ fontFamily: "var(--f-display)", textTransform: "uppercase", fontWeight: 700, letterSpacing: ".02em" }}>{p.name}</span>
                    </span>
                  </td>
                  <td className="l"><span className="pos-pill" style={{ background: p.pos === "SP" ? "var(--pennant)" : "var(--infield)" }}>{p.pos}</span></td>
                  <td>{p.w}</td><td>{p.l}</td>
                  <td style={{ color: "var(--accent)", fontWeight: 700 }}>{p.era}</td>
                  <td>{p.whip}</td><td>{p.k}</td><td>{p.ip}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

function ScheduleTab({ team }) {
  return (
    <div className="col-2">
      <div className="card lift">
        <div className="card-hdr"><h3>Recent Results</h3><span className="stamp">Last 6</span></div>
        <table className="roster">
          <thead>
            <tr>
              <th className="l">Date</th>
              <th className="l">Opp</th>
              <th className="l">Result</th>
              <th className="l">WP</th>
              <th className="l">SV</th>
            </tr>
          </thead>
          <tbody>
            {BRK_SCHEDULE.slice().reverse().map((g, i) => {
              const opp = teamById(g.opp);
              const win = g.res.startsWith("W") || g.res.includes("· W");
              return (
                <tr key={i}>
                  <td className="l" style={{ fontFamily: "var(--f-mono)" }}>{g.date}</td>
                  <td className="l">
                    <span className="player-cell">
                      <span style={{ color: "var(--ink-3)", fontFamily: "var(--f-display)", textTransform: "uppercase", fontSize: 12, width: 24 }}>{g.ha}</span>
                      <TeamChip team={opp} />
                      <span style={{ fontFamily: "var(--f-display)", textTransform: "uppercase", fontWeight: 700, letterSpacing: ".02em" }}>{opp.nick}</span>
                    </span>
                  </td>
                  <td className="l" style={{ fontFamily: "var(--f-display)", fontWeight: 800, color: win ? "var(--infield)" : "var(--oxblood)", textTransform: "uppercase" }}>{g.res}</td>
                  <td className="l">{g.wp || "—"}</td>
                  <td className="l">{g.sv || "—"}</td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
      <div className="card lift">
        <div className="card-hdr"><h3>Upcoming Slate</h3><span className="stamp">Next 7</span></div>
        <div style={{ padding: "8px 14px" }}>
          {[
            { date: "JUN 18", opp: "cle", ha: "@", time: "7:35 PM", probable: "Holloway vs Hal Brennan" },
            { date: "JUN 19", opp: "cle", ha: "@", time: "7:35 PM", probable: "Petrocelli vs Tate" },
            { date: "JUN 20", opp: "cle", ha: "@", time: "1:05 PM", probable: "Banuelos vs Wojcik" },
            { date: "JUN 21", opp: "stp", ha: "vs", time: "7:05 PM", probable: "Holloway vs Whitcomb" },
            { date: "JUN 22", opp: "stp", ha: "vs", time: "1:35 PM", probable: "Petrocelli vs Acuña" },
            { date: "JUN 23", opp: "stp", ha: "vs", time: "7:05 PM", probable: "Banuelos vs Marsh" },
            { date: "JUN 24", opp: "det", ha: "@", time: "7:10 PM", probable: "Holloway vs Kasprzak" },
          ].map((g, i) => {
            const opp = teamById(g.opp);
            return (
              <div key={i} style={{ display: "grid", gridTemplateColumns: "60px auto 1fr auto", gap: 12, alignItems: "center", padding: "10px 0", borderBottom: i < 6 ? "1px dashed var(--rule)" : "0" }}>
                <div style={{ fontFamily: "var(--f-mono)", fontSize: 12, fontWeight: 700 }}>{g.date}</div>
                <span className="player-cell">
                  <span style={{ color: "var(--ink-3)", fontFamily: "var(--f-display)", textTransform: "uppercase", fontSize: 12, width: 24 }}>{g.ha}</span>
                  <TeamChip team={opp} />
                </span>
                <div>
                  <div style={{ fontFamily: "var(--f-display)", textTransform: "uppercase", fontWeight: 700, letterSpacing: ".02em" }}>{opp.city} {opp.nick}</div>
                  <div style={{ fontFamily: "var(--f-sans)", fontSize: 11, color: "var(--ink-3)" }}>Probable: {g.probable}</div>
                </div>
                <div style={{ fontFamily: "var(--f-mono)", fontSize: 12, color: "var(--ink-3)" }}>{g.time}</div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}

function TeamStatsTab({ team }) {
  const cells = [
    { lbl: "Runs / G",  val: "4.78", lead: true },
    { lbl: "AVG",       val: ".284", lead: true },
    { lbl: "OBP",       val: ".358", lead: true },
    { lbl: "SLG",       val: ".451" },
    { lbl: "OPS",       val: ".809", lead: true },
    { lbl: "HR",        val: "142" },
    { lbl: "SB",        val: "84" },
    { lbl: "Team ERA",  val: "3.21", lead: true },
    { lbl: "Team WHIP", val: "1.18" },
    { lbl: "K/9",       val: "9.4" },
    { lbl: "BB/9",      val: "2.9" },
    { lbl: "Fld%",      val: ".988" },
  ];
  return (
    <div>
      <div className="sec-hdr">
        <h2>Team Stats</h2>
        <span className="sub">League ranks shown in oxblood</span>
      </div>
      <div className="statgrid">
        {cells.map((c, i) => (
          <div key={i} className="cell">
            <div className="lbl">{c.lbl}</div>
            <div className={"val " + (c.lead ? "lead" : "")}>{c.val}</div>
            <div style={{ fontFamily: "var(--f-sans)", fontSize: 10, color: "var(--ink-3)", letterSpacing: ".12em", textTransform: "uppercase", marginTop: 4, fontWeight: 700 }}>
              {c.lead ? "1ST IN TBL" : "MID-PACK"}
            </div>
          </div>
        ))}
      </div>

      <div style={{ marginTop: 22 }} className="col-2">
        <div className="card lift">
          <div className="card-hdr"><h3>Splits</h3><span className="stamp">Hitting</span></div>
          <table className="roster">
            <thead>
              <tr><th className="l">Split</th><th>G</th><th>AVG</th><th>OBP</th><th>SLG</th><th>OPS</th><th>HR</th></tr>
            </thead>
            <tbody>
              {[
                ["vs RHP", 68, ".291", ".366", ".471", ".837", 94],
                ["vs LHP", 34, ".272", ".341", ".411", ".752", 48],
                ["Home",   51, ".298", ".374", ".478", ".852", 82],
                ["Away",   51, ".271", ".341", ".425", ".766", 60],
                ["Day",    32, ".278", ".348", ".436", ".784", 38],
                ["Night",  70, ".287", ".362", ".458", ".820", 104],
              ].map((r, i) => (
                <tr key={i}>
                  <td className="l" style={{ fontFamily: "var(--f-display)", fontWeight: 700, textTransform: "uppercase", letterSpacing: ".03em" }}>{r[0]}</td>
                  {r.slice(1).map((v, j) => <td key={j}>{v}</td>)}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="card lift">
          <div className="card-hdr"><h3>Park Factors</h3><span className="stamp">Crown Field · {team.city}</span></div>
          <div style={{ padding: 16 }}>
            {[
              ["Runs",   105], ["HR",     112], ["2B",     97],
              ["3B",     118], ["BB",     101], ["K",      98],
              ["BA Right", 104], ["BA Left", 99],
            ].map((r, i) => (
              <div key={i} style={{ display: "grid", gridTemplateColumns: "100px 1fr 48px", gap: 12, alignItems: "center", padding: "7px 0", borderBottom: i < 7 ? "1px dashed var(--rule)" : "0" }}>
                <div style={{ fontFamily: "var(--f-sans)", fontSize: 11.5, letterSpacing: ".06em", textTransform: "uppercase", fontWeight: 600 }}>{r[0]}</div>
                <div style={{ height: 8, background: "var(--rule)", border: "1px solid var(--rule-strong)", position: "relative" }}>
                  <div style={{ position: "absolute", left: "50%", top: -2, bottom: -2, width: 1, background: "var(--rule-strong)" }} />
                  <div style={{ position: "absolute", left: r[1] >= 100 ? "50%" : ((r[1] / 100) * 50) + "%", top: 0, bottom: 0, width: Math.abs(r[1] - 100) / 2 + "%", background: r[1] >= 100 ? "var(--oxblood)" : "var(--pennant)" }} />
                </div>
                <div style={{ fontFamily: "var(--f-mono)", fontSize: 12, fontWeight: 700, textAlign: "right" }}>{r[1]}</div>
              </div>
            ))}
            <div style={{ fontFamily: "var(--f-sans)", fontSize: 10.5, color: "var(--ink-3)", marginTop: 8, letterSpacing: ".12em", textTransform: "uppercase" }}>
              100 = LEAGUE NEUTRAL · Updated for {team.city} replay
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

function TeamPage({ teamId, navigate }) {
  const team = teamById(teamId);
  const [tab, setTab] = React.useState("Roster");
  const tabs = ["Roster", "Schedule", "Stats", "News", "Depth Chart", "Transactions"];
  return (
    <div data-screen-label={"02 Team — " + team.city + " " + team.nick}>
      <TeamBigCard team={team} />
      <div style={{ marginTop: 18 }} className="inner-tabs">
        {tabs.map(t => (
          <button key={t} className={tab === t ? "on" : ""} onClick={() => setTab(t)}>{t}</button>
        ))}
        <div style={{ marginLeft: "auto", display: "flex", gap: 8, alignItems: "center", padding: "5px 0" }}>
          <button className="btn sm">Follow</button>
          <button className="btn sm primary">Take Control</button>
        </div>
      </div>
      <div style={{ marginTop: 18 }}>
        {tab === "Roster" && <RosterTab team={team} />}
        {tab === "Schedule" && <ScheduleTab team={team} />}
        {tab === "Stats" && <TeamStatsTab team={team} />}
        {tab === "News" && <NewsRail />}
        {(tab === "Depth Chart" || tab === "Transactions") && (
          <div style={{ padding: 60, textAlign: "center", fontFamily: "var(--f-display)", color: "var(--ink-3)", textTransform: "uppercase", letterSpacing: ".1em" }}>
            {tab} · Coming soon to your console
          </div>
        )}
      </div>
    </div>
  );
}

Object.assign(window, { TeamPage });
