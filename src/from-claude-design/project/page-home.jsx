// page-home.jsx — League Home / scoreboard hub

function ScoreTile({ g, onOpen }) {
  const away = teamById(g.away), home = teamById(g.home);
  const live = g.status === "LIVE";
  const isFinal = g.status === "FINAL";
  const aWin = isFinal && g.ar > g.hr;
  const hWin = isFinal && g.hr > g.ar;
  return (
    <div className={"score-tile " + (live ? "live" : isFinal ? "final" : "")} onClick={onOpen}>
      <div className="score-status">
        {live ? <><span className="live-dot" /><span className="ip">{g.inning}</span><span className="right">{g.outs} OUT · {g.baseState}</span></> :
         isFinal ? <><span>FINAL</span><span className="right">{g.inning !== "FINAL" ? g.inning : ""}</span></> :
         <><span>{g.inning}</span><span className="right">TBL Network</span></>}
      </div>
      <div className="score-rows">
        <div className={"score-row " + (isFinal ? (aWin ? "win" : "lose") : "")}>
          <span className="seed">A</span>
          <span className="team">
            <TeamChip team={away} />
            <span className="city">{away.city}</span><span className="nick">{away.nick}</span>
            <span className="rec">{away.w}-{away.l}</span>
          </span>
          <span className="runs">{g.status === "LIVE" || isFinal ? g.ar : "—"}</span>
        </div>
        <div className={"score-row " + (isFinal ? (hWin ? "win" : "lose") : "")}>
          <span className="seed">H</span>
          <span className="team">
            <TeamChip team={home} />
            <span className="city">{home.city}</span><span className="nick">{home.nick}</span>
            <span className="rec">{home.w}-{home.l}</span>
          </span>
          <span className="runs">{g.status === "LIVE" || isFinal ? g.hr : "—"}</span>
        </div>
      </div>
      {(live || isFinal) ? (
        <div className="pitcher-chip">
          {live ? (
            <>
              <span>WIN PROB <b>{home.abbr} {g.winProb}%</b></span>
              {g.walkoff && <span style={{ color: "var(--oxblood)", fontWeight: 700 }}>· WALK-OFF CHANCE</span>}
            </>
          ) : (
            <>
              <span>WP <b>{g.wp || "Holloway"}</b></span>
              <span>LP {g.lp || "Brennan"}</span>
            </>
          )}
        </div>
      ) : (
        <div className="pitcher-chip">
          <span>SP <b>{g.pitchA}</b></span>
          <span style={{ color: "var(--ink-4)" }}>vs</span>
          <span>SP <b>{g.pitchH}</b></span>
        </div>
      )}
    </div>
  );
}

function DateStrip() {
  const days = [
    { dow: "MON", num: 16, mo: "JUN" },
    { dow: "TUE", num: 17, mo: "JUN", on: true },
    { dow: "WED", num: 18, mo: "JUN" },
    { dow: "THU", num: 19, mo: "JUN" },
    { dow: "FRI", num: 20, mo: "JUN" },
    { dow: "SAT", num: 21, mo: "JUN" },
    { dow: "SUN", num: 22, mo: "JUN" },
  ];
  return (
    <div className="date-strip">
      {days.map((d, i) => (
        <div key={i} className={"d " + (d.on ? "on" : "")}>
          <div className="dow">{d.dow}</div>
          <div className="num">{d.num}</div>
          <div className="mo">{d.mo}</div>
        </div>
      ))}
      <div className="d" style={{ minWidth: "auto", padding: "6px 12px", display: "flex", alignItems: "center" }}>
        <span style={{ fontFamily: "var(--f-sans)", fontSize: 11, letterSpacing: ".12em" }}>JUMP TO ▾</span>
      </div>
    </div>
  );
}

function NewsRail({ onTeam }) {
  return (
    <div className="card lift">
      <div className="card-hdr">
        <h3>The Wire</h3>
        <span className="stamp">TBL · TUE JUN 17</span>
      </div>
      <div style={{ padding: "0 16px" }}>
        {NEWS.map((n, i) => (
          <div key={i} className="news-item" onClick={() => i === 0 && onTeam && onTeam("brk")}>
            <div className={"news-thumb " + (n.thumbAccent ? "accent" : n.ink ? "ink" : "")}>{n.mono}</div>
            <div>
              <div className="news-eyebrow">{n.eye} {n.live && <span style={{ color: "var(--oxblood)" }}>· LIVE RECAP</span>}</div>
              <div className="news-headline">{n.head}</div>
              <div className="news-meta">{n.meta}</div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

function LeadersCard({ onPlayer }) {
  const [tab, setTab] = React.useState("AVG");
  const cats = ["AVG", "HR", "RBI", "ERA", "K"];
  const rows = LEADERS[tab] || [];
  return (
    <div className="card lift">
      <div className="card-hdr">
        <h3>Statistical Leaders</h3>
        <span className="stamp">Season · Through Day 102</span>
      </div>
      <div className="leaders-tabs">
        {cats.map(c => (
          <button key={c} className={tab === c ? "on" : ""} onClick={() => setTab(c)}>{c}</button>
        ))}
      </div>
      <div className="leaders" style={{ padding: "4px 14px 8px" }}>
        {rows.map((r, i) => (
          <div key={i} className={"leader-row r" + (i + 1)} onClick={() => i === 0 && onPlayer && onPlayer()}>
            <span className="rk">{i + 1}</span>
            <span className="nm">{r.name}<small>{r.team}</small></span>
            <span className="v">{r.v}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

function StandingsStrip({ onTeam }) {
  // group by division
  const divs = ["East", "Central", "West"];
  return (
    <div className="col-3">
      {divs.map(d => {
        const teams = TEAMS.filter(t => t.div === d).sort((a, b) => b.w - a.w);
        return (
          <div className="card lift" key={d}>
            <div className="card-hdr">
              <h3>{d} Division</h3>
              <span className="stamp">Through Day 102</span>
            </div>
            <table className="standings-table">
              <thead>
                <tr>
                  <th className="l">Club</th>
                  <th>W</th><th>L</th><th>PCT</th><th>GB</th><th>L10</th><th>STRK</th>
                </tr>
              </thead>
              <tbody>
                {teams.map((t, i) => (
                  <tr key={t.id} className={i === 0 ? "div-leader" : ""} onClick={() => onTeam && onTeam(t.id)}>
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
                    <td className="num">{t.l10}</td>
                    <td className={"num streak " + t.streak[0]}>{t.streak}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        );
      })}
    </div>
  );
}

function HotStreaks() {
  const items = [
    { t: "brk", lbl: "Brooklyn", streak: "W4", note: "Outscoring opponents 27–9 over four" },
    { t: "psf", lbl: "Pacific Coast", streak: "W3", note: "Bullpen has a 1.42 ERA in the run" },
    { t: "mtl", lbl: "Montreal",  streak: "L5", note: "Royaux drop 13 of last 17 overall" },
  ];
  return (
    <div className="card lift">
      <div className="card-hdr">
        <h3>Streaks &amp; Trends</h3>
        <span className="stamp">Auto-generated</span>
      </div>
      <div style={{ padding: "8px 16px 14px" }}>
        {items.map((it, i) => {
          const tm = teamById(it.t);
          return (
            <div key={i} style={{ display: "grid", gridTemplateColumns: "auto 1fr auto", gap: 12, alignItems: "center", padding: "10px 0", borderBottom: i < items.length - 1 ? "1px dashed var(--rule)" : "0" }}>
              <TeamChip team={tm} size="lg" />
              <div>
                <div style={{ fontFamily: "var(--f-display)", textTransform: "uppercase", fontWeight: 700, fontSize: 15, letterSpacing: ".03em" }}>{tm.city} {tm.nick}</div>
                <div style={{ fontFamily: "var(--f-sans)", fontSize: 11.5, color: "var(--ink-3)", marginTop: 2 }}>{it.note}</div>
              </div>
              <span className={"pill " + (it.streak[0] === "W" ? "accent" : "ink")}>{it.streak}</span>
            </div>
          );
        })}
      </div>
    </div>
  );
}

function PennantRace() {
  const east = TEAMS.filter(t => t.div === "East").sort((a, b) => b.w - a.w);
  const max = east[0].w + east[0].l;
  return (
    <div className="card lift">
      <div className="card-hdr">
        <h3>Pennant Race — East</h3>
        <span className="stamp">PYTHAGOREAN W%</span>
      </div>
      <div style={{ padding: "12px 16px 16px" }}>
        {east.map((t, i) => {
          const pct = parseFloat(t.pct);
          const width = pct * 100;
          const ahead = i === 0;
          return (
            <div key={t.id} style={{ display: "grid", gridTemplateColumns: "auto 110px 1fr auto", gap: 10, alignItems: "center", padding: "8px 0" }}>
              <TeamChip team={t} />
              <div style={{ fontFamily: "var(--f-display)", fontSize: 13, letterSpacing: ".03em", textTransform: "uppercase", fontWeight: 700, color: ahead ? "var(--ink)" : "var(--ink-3)" }}>{t.city.toUpperCase()} {t.nick}</div>
              <div style={{ height: 10, background: "var(--rule)", position: "relative", border: "1px solid var(--rule-strong)" }}>
                <div style={{ position: "absolute", left: 0, top: 0, bottom: 0, width: width + "%", background: ahead ? "var(--accent)" : "var(--ink-3)" }} />
                <div style={{ position: "absolute", left: "50%", top: -2, bottom: -2, width: 1, background: "var(--rule-strong)" }} />
              </div>
              <div style={{ fontFamily: "var(--f-mono)", fontSize: 12, fontWeight: 700 }}>{t.pct}</div>
            </div>
          );
        })}
        <div style={{ fontFamily: "var(--f-sans)", fontSize: 10.5, color: "var(--ink-3)", marginTop: 6, letterSpacing: ".12em", textTransform: "uppercase" }}>
          Magic # to clinch <b style={{ color: "var(--ink)" }}>43</b> · Tragic # <b style={{ color: "var(--ink)" }}>27</b>
        </div>
      </div>
    </div>
  );
}

function TickerHero({ league }) {
  return (
    <div className="card lift-accent" style={{ background: "var(--paper-2)" }}>
      <div style={{ display: "grid", gridTemplateColumns: "auto 1fr auto", gap: 22, alignItems: "center", padding: "18px 22px" }}>
        <div style={{ display: "flex", flexDirection: "column", gap: 4 }}>
          <span className="eyebrow">{league.year} · TUESDAY JUN 17</span>
          <span style={{ fontFamily: "var(--f-display)", fontWeight: 900, fontSize: 28, textTransform: "uppercase", lineHeight: 1, letterSpacing: "-0.005em" }}>
            Today’s Card
          </span>
        </div>
        <div style={{ display: "flex", gap: 22, fontFamily: "var(--f-sans)", flexWrap: "nowrap", whiteSpace: "nowrap" }}>
          <div><div style={{ fontSize: 10, letterSpacing: ".14em", textTransform: "uppercase", color: "var(--ink-3)", fontWeight: 700 }}>Slate</div><div style={{ fontFamily: "var(--f-display)", fontSize: 22, fontWeight: 900 }}>8 Games</div></div>
          <div><div style={{ fontSize: 10, letterSpacing: ".14em", textTransform: "uppercase", color: "var(--ink-3)", fontWeight: 700 }}>Live</div><div style={{ fontFamily: "var(--f-display)", fontSize: 22, fontWeight: 900, color: "var(--oxblood)" }}>3</div></div>
          <div><div style={{ fontSize: 10, letterSpacing: ".14em", textTransform: "uppercase", color: "var(--ink-3)", fontWeight: 700 }}>Final</div><div style={{ fontFamily: "var(--f-display)", fontSize: 22, fontWeight: 900 }}>2</div></div>
          <div><div style={{ fontSize: 10, letterSpacing: ".14em", textTransform: "uppercase", color: "var(--ink-3)", fontWeight: 700 }}>Upcoming</div><div style={{ fontFamily: "var(--f-display)", fontSize: 22, fontWeight: 900 }}>3</div></div>
          <div><div style={{ fontSize: 10, letterSpacing: ".14em", textTransform: "uppercase", color: "var(--ink-3)", fontWeight: 700 }}>HR Today</div><div style={{ fontFamily: "var(--f-display)", fontSize: 22, fontWeight: 900 }}>14</div></div>
        </div>
        <div style={{ display: "flex", gap: 8 }}>
          <button className="btn sm">Box Scores</button>
          <button className="btn sm primary">Play-by-Play</button>
        </div>
      </div>
    </div>
  );
}

function HomePage({ league, navigate }) {
  return (
    <div data-screen-label="01 League Home">
      <TickerHero league={league} />
      <div style={{ marginTop: 22 }}>
        <div className="sec-hdr">
          <h2>Scoreboard</h2>
          <span className="sub">Today's slate · 12 clubs · Updates every tick</span>
          <span className="right">All times ET · <a>View Yesterday →</a></span>
        </div>
        <DateStrip />
        <div className="scores-grid">
          {TODAYS_GAMES.map(g => <ScoreTile key={g.id} g={g} onOpen={() => {}} />)}
        </div>
      </div>

      <div style={{ marginTop: 28 }}>
        <div className="sec-hdr">
          <h2>Standings</h2>
          <span className="sub">East · Central · West</span>
          <span className="right"><a onClick={() => navigate("standings")}>Full Standings →</a></span>
        </div>
        <StandingsStrip onTeam={(id) => navigate("team", id)} />
      </div>

      <div style={{ marginTop: 28 }}>
        <div className="col-2">
          <div>
            <div className="sec-hdr">
              <h2>The Wire</h2>
              <span className="sub">Recaps, transactions, league office</span>
              <span className="right"><a>All News →</a></span>
            </div>
            <NewsRail onTeam={(id) => navigate("team", id)} />
          </div>
          <div>
            <div className="sec-hdr">
              <h2>League Pulse</h2>
              <span className="sub">Auto · Sabermetrics</span>
            </div>
            <LeadersCard onPlayer={() => navigate("player")} />
            <div style={{ height: 16 }} />
            <HotStreaks />
            <div style={{ height: 16 }} />
            <PennantRace />
          </div>
        </div>
      </div>
    </div>
  );
}

Object.assign(window, { HomePage });
