// page-standings.jsx — Full standings page

function StandingsPage({ navigate }) {
  const [view, setView] = React.useState("Division");
  const views = ["Division", "League", "Wild Card", "Expanded"];
  const [sortKey, setSortKey] = React.useState("w");
  const [sortDir, setSortDir] = React.useState("desc");

  function sortFn(a, b) {
    let av = a[sortKey], bv = b[sortKey];
    if (typeof av === "string") { av = parseFloat(av); bv = parseFloat(bv); }
    const r = av - bv;
    return sortDir === "desc" ? -r : r;
  }
  function clickSort(k) {
    if (sortKey === k) setSortDir(sortDir === "desc" ? "asc" : "desc");
    else { setSortKey(k); setSortDir("desc"); }
  }
  function H({ k, lbl, l }) {
    const on = sortKey === k;
    return <th onClick={() => clickSort(k)} className={(l ? "l " : "") + (on ? "sort-active" : "")} style={{ cursor: "default" }}>{lbl}</th>;
  }

  function DivTable({ d }) {
    const teams = TEAMS.filter(t => t.div === d).sort(sortFn);
    return (
      <div className="card lift">
        <div className="card-hdr">
          <h3>{d} Division</h3>
          <span className="stamp">Click headers to sort</span>
        </div>
        <table className="standings-table">
          <thead>
            <tr>
              <th className="l">Club</th>
              <H k="w" lbl="W" /><H k="l" lbl="L" /><H k="pct" lbl="PCT" />
              <th>GB</th>
              <H k="rs" lbl="RS" /><H k="ra" lbl="RA" /><H k="rd" lbl="DIFF" />
              <th>HOME</th><th>AWAY</th><th>L10</th><th>STRK</th>
            </tr>
          </thead>
          <tbody>
            {teams.map((t, i) => (
              <tr key={t.id} className={i === 0 ? "div-leader" : ""} onClick={() => navigate("team", t.id)}>
                <td>
                  <span className="team-cell">
                    <TeamChip team={t} />
                    <span className="team-mono">{t.city} {t.nick}</span>
                  </span>
                </td>
                <td className="num win">{t.w}</td>
                <td className="num">{t.l}</td>
                <td className="num">{t.pct}</td>
                <td className="num">{t.gb}</td>
                <td className="num">{t.rs}</td>
                <td className="num">{t.ra}</td>
                <td className="num" style={{ color: t.rd.startsWith("+") ? "var(--infield)" : "var(--oxblood)", fontWeight: 700 }}>{t.rd}</td>
                <td className="num">{t.home}</td>
                <td className="num">{t.away}</td>
                <td className="num">{t.l10}</td>
                <td className={"num streak " + t.streak[0]}>{t.streak}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  }

  function LeagueTable() {
    const all = TEAMS.slice().sort(sortFn);
    return (
      <div className="card lift">
        <div className="card-hdr">
          <h3>Overall League Standings</h3>
          <span className="stamp">12 Clubs · Through Day 102</span>
        </div>
        <table className="standings-table">
          <thead>
            <tr>
              <th className="l">#</th>
              <th className="l">Club</th>
              <th>DIV</th>
              <H k="w" lbl="W" /><H k="l" lbl="L" /><H k="pct" lbl="PCT" />
              <H k="rs" lbl="RS" /><H k="ra" lbl="RA" /><H k="rd" lbl="DIFF" />
              <th>L10</th><th>STRK</th>
            </tr>
          </thead>
          <tbody>
            {all.map((t, i) => (
              <tr key={t.id} className={i === 0 ? "div-leader" : ""} onClick={() => navigate("team", t.id)}>
                <td className="num0 l">{i + 1}</td>
                <td>
                  <span className="team-cell">
                    <TeamChip team={t} />
                    <span className="team-mono">{t.city} {t.nick}</span>
                  </span>
                </td>
                <td className="num"><span className="pill outline" style={{ fontSize: 9 }}>{t.div.slice(0, 4).toUpperCase()}</span></td>
                <td className="num win">{t.w}</td>
                <td className="num">{t.l}</td>
                <td className="num">{t.pct}</td>
                <td className="num">{t.rs}</td>
                <td className="num">{t.ra}</td>
                <td className="num" style={{ color: t.rd.startsWith("+") ? "var(--infield)" : "var(--oxblood)", fontWeight: 700 }}>{t.rd}</td>
                <td className="num">{t.l10}</td>
                <td className={"num streak " + t.streak[0]}>{t.streak}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  }

  return (
    <div data-screen-label="04 Standings">
      <div className="sec-hdr">
        <h2>Standings</h2>
        <span className="sub">Twilight Baseball League · 1958 Replay · Day 102 of 162</span>
        <span className="right">Updated 2 min ago</span>
      </div>

      <div className="inner-tabs" style={{ marginBottom: 18 }}>
        {views.map(v => (
          <button key={v} className={view === v ? "on" : ""} onClick={() => setView(v)}>{v}</button>
        ))}
        <div style={{ marginLeft: "auto", display: "flex", gap: 6, alignItems: "center", padding: "5px 0" }}>
          <span className="pill outline">SEASON</span>
          <span className="pill outline">LAST 30 DAYS</span>
          <span className="pill outline">vs .500+</span>
          <span className="pill outline">vs &lt; .500</span>
        </div>
      </div>

      {view === "Division" && (
        <div style={{ display: "grid", gap: 22 }}>
          <DivTable d="East" />
          <DivTable d="Central" />
          <DivTable d="West" />
        </div>
      )}
      {view === "League" && <LeagueTable />}
      {view === "Wild Card" && (
        <div className="col-2">
          <div className="card lift">
            <div className="card-hdr"><h3>Wild Card Standings · Tournament Bound</h3><span className="stamp">4 Berths</span></div>
            <table className="standings-table">
              <thead><tr><th className="l">Club</th><th>W</th><th>L</th><th>PCT</th><th>GB</th><th>STRK</th></tr></thead>
              <tbody>
                {TEAMS.slice().sort((a, b) => b.w - a.w).slice(0, 8).map((t, i) => (
                  <tr key={t.id} className={i < 4 ? "div-leader" : ""} onClick={() => navigate("team", t.id)}>
                    <td><span className="team-cell"><TeamChip team={t} /><span className="team-mono">{t.city} {t.nick}</span></span></td>
                    <td className="num win">{t.w}</td><td className="num">{t.l}</td><td className="num">{t.pct}</td><td className="num">{i === 0 ? "—" : (TEAMS[0].w - t.w) + ".5"}</td>
                    <td className={"num streak " + t.streak[0]}>{t.streak}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="card lift">
            <div className="card-hdr"><h3>Magic / Tragic Numbers</h3><span className="stamp">60 Games to Go</span></div>
            <div style={{ padding: "12px 16px" }}>
              {TEAMS.slice().sort((a, b) => b.w - a.w).slice(0, 8).map((t, i) => (
                <div key={t.id} style={{ display: "grid", gridTemplateColumns: "auto 1fr auto auto", gap: 14, alignItems: "center", padding: "8px 0", borderBottom: i < 7 ? "1px dashed var(--rule)" : 0 }}>
                  <TeamChip team={t} />
                  <div style={{ fontFamily: "var(--f-display)", fontSize: 14, fontWeight: 700, letterSpacing: ".02em", textTransform: "uppercase" }}>{t.city} {t.nick}</div>
                  <span className="pill outline">CLINCH <b style={{ color: "var(--infield)", marginLeft: 4 }}>{60 - i * 5}</b></span>
                  <span className="pill outline">ELIM <b style={{ color: "var(--oxblood)", marginLeft: 4 }}>{40 + i * 4}</b></span>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}
      {view === "Expanded" && <LeagueTable />}

      <div style={{ marginTop: 22 }} className="col-3">
        <div className="card lift">
          <div className="card-hdr"><h3>Run Differential</h3><span className="stamp">League-wide</span></div>
          <div style={{ padding: "12px 16px" }}>
            {TEAMS.slice().sort((a, b) => parseInt(b.rd) - parseInt(a.rd)).map((t, i) => {
              const v = parseInt(t.rd);
              const max = 110;
              const pct = Math.min(Math.abs(v) / max, 1) * 100;
              const pos = v >= 0;
              return (
                <div key={t.id} style={{ display: "grid", gridTemplateColumns: "auto 70px 1fr 1fr 44px", gap: 8, alignItems: "center", padding: "5px 0" }}>
                  <TeamChip team={t} />
                  <div style={{ fontFamily: "var(--f-display)", fontSize: 11, letterSpacing: ".03em", textTransform: "uppercase", fontWeight: 700 }}>{t.abbr}</div>
                  <div style={{ height: 8, background: pos ? "transparent" : "var(--paper-3)", position: "relative", border: pos ? 0 : "1px solid var(--rule-strong)" }}>
                    {!pos && <div style={{ position: "absolute", right: 0, top: 0, bottom: 0, width: pct + "%", background: "var(--oxblood)" }} />}
                  </div>
                  <div style={{ height: 8, background: pos ? "var(--paper-3)" : "transparent", position: "relative", border: pos ? "1px solid var(--rule-strong)" : 0 }}>
                    {pos && <div style={{ position: "absolute", left: 0, top: 0, bottom: 0, width: pct + "%", background: "var(--infield)" }} />}
                  </div>
                  <div style={{ fontFamily: "var(--f-mono)", fontSize: 11, fontWeight: 700, textAlign: "right", color: pos ? "var(--infield)" : "var(--oxblood)" }}>{t.rd}</div>
                </div>
              );
            })}
          </div>
        </div>
        <div className="card lift">
          <div className="card-hdr"><h3>Vs. Division</h3><span className="stamp">Head-to-head</span></div>
          <div style={{ padding: 16, overflowX: "auto" }}>
            <table className="roster" style={{ fontSize: 11 }}>
              <thead>
                <tr>
                  <th className="l"></th>
                  {TEAMS.slice(0, 6).map(t => <th key={t.id}>{t.abbr}</th>)}
                </tr>
              </thead>
              <tbody>
                {TEAMS.slice(0, 6).map((t, i) => (
                  <tr key={t.id}>
                    <td className="l"><span className="player-cell"><TeamChip team={t} /><span style={{ fontFamily: "var(--f-display)", fontSize: 11, letterSpacing: ".03em", textTransform: "uppercase" }}>{t.abbr}</span></span></td>
                    {TEAMS.slice(0, 6).map((o, j) => {
                      if (i === j) return <td key={j} style={{ background: "var(--ink)", color: "var(--paper)", fontFamily: "var(--f-mono)" }}>—</td>;
                      const w = Math.floor(Math.random() * 5) + 2;
                      const l = Math.floor(Math.random() * 5) + 1;
                      const lead = w > l;
                      return <td key={j} style={{ fontFamily: "var(--f-mono)", fontWeight: lead ? 700 : 500, color: lead ? "var(--infield)" : "var(--ink-3)" }}>{w}-{l}</td>;
                    })}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
        <div className="card lift">
          <div className="card-hdr"><h3>Sim Projections</h3><span className="stamp">10K Monte Carlo</span></div>
          <div style={{ padding: "12px 16px" }}>
            {TEAMS.slice().sort((a, b) => b.w - a.w).slice(0, 8).map((t, i) => {
              const playoff = 99 - i * 12;
              const ws = Math.max(2, 28 - i * 4);
              return (
                <div key={t.id} style={{ display: "grid", gridTemplateColumns: "auto 1fr auto auto", gap: 10, alignItems: "center", padding: "8px 0", borderBottom: i < 7 ? "1px dashed var(--rule)" : 0 }}>
                  <TeamChip team={t} />
                  <div style={{ fontFamily: "var(--f-display)", fontSize: 13, fontWeight: 700, letterSpacing: ".02em", textTransform: "uppercase" }}>{t.abbr}</div>
                  <div style={{ fontFamily: "var(--f-sans)", fontSize: 10, color: "var(--ink-3)", letterSpacing: ".1em", textTransform: "uppercase", fontWeight: 700 }}>PO <b style={{ color: "var(--ink)", fontFamily: "var(--f-mono)" }}>{playoff}%</b></div>
                  <div style={{ fontFamily: "var(--f-sans)", fontSize: 10, color: "var(--ink-3)", letterSpacing: ".1em", textTransform: "uppercase", fontWeight: 700 }}>WS <b style={{ color: "var(--accent)", fontFamily: "var(--f-mono)" }}>{ws}%</b></div>
                </div>
              );
            })}
          </div>
        </div>
      </div>
    </div>
  );
}

Object.assign(window, { StandingsPage });
