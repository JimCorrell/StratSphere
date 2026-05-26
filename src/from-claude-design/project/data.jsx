// data.jsx — fake leagues/teams/players/games for StratSphere
// All teams are historical defunct franchise names (public-domain Americana).

const LEAGUES = [
  { id: "tbl",  name: "Twilight Baseball League",   abbr: "TBL", year: "1958 Replay", mark: "T", established: "EST. 2019", teamCount: 12, gamesToday: 8, currentDay: "Day 102 of 162" },
  { id: "pcr",  name: "Pacific Coast Replay",       abbr: "PCR", year: "1952 Replay", mark: "P", established: "EST. 2021", teamCount: 8,  gamesToday: 4 },
  { id: "hfl",  name: "Heartland Federation",       abbr: "HFL", year: "Live Sim '26", mark: "H", established: "EST. 2017", teamCount: 16, gamesToday: 6 },
  { id: "dust", name: "Dust Bowl Replay Society",   abbr: "DBR", year: "1934 Replay", mark: "D", established: "EST. 2024", teamCount: 10, gamesToday: 0 },
  { id: "iron", name: "Ironworks Sim League",       abbr: "ISL", year: "Career Yr 7", mark: "I", established: "EST. 2020", teamCount: 14, gamesToday: 7 },
];

// Twilight Baseball League — 12 teams
// Crest color drives ::accent for the team chip
const TEAMS = [
  { id: "brk", city: "Brooklyn",      nick: "Royals",     abbr: "BRK", mono: "BR", div: "East", color: "#1f3a5c", ink: "#f6ecd2", w: 64, l: 38, pct: ".627", gb: "—",  streak: "W4", l10: "7-3", home: "34-17", away: "30-21", rs: 487, ra: 386, rd: "+101" },
  { id: "stp", city: "Saint Paul",    nick: "Apostles",   abbr: "STP", mono: "SP", div: "Central", color: "#2d5b34", ink: "#f6ecd2", w: 60, l: 42, pct: ".588", gb: "—",  streak: "L1", l10: "6-4", home: "33-18", away: "27-24", rs: 472, ra: 421, rd: "+51"  },
  { id: "det", city: "Detroit",       nick: "Wolverines", abbr: "DET", mono: "DT", div: "Central", color: "#142840", ink: "#dcdcdc", w: 57, l: 45, pct: ".559", gb: "3",  streak: "W2", l10: "6-4", home: "31-20", away: "26-25", rs: 466, ra: 422, rd: "+44"  },
  { id: "phl", city: "Philadelphia",  nick: "Quakers",    abbr: "PHL", mono: "PQ", div: "East", color: "#6e1c1c", ink: "#f6ecd2", w: 56, l: 46, pct: ".549", gb: "8",  streak: "W1", l10: "5-5", home: "28-23", away: "28-23", rs: 451, ra: 433, rd: "+18"  },
  { id: "cle", city: "Cleveland",     nick: "Spiders",    abbr: "CLE", mono: "CV", div: "Central", color: "#1b1815", ink: "#c89a1f", w: 54, l: 48, pct: ".529", gb: "6",  streak: "L2", l10: "4-6", home: "27-24", away: "27-24", rs: 459, ra: 458, rd: "+1"   },
  { id: "buf", city: "Buffalo",       nick: "Bisons",     abbr: "BUF", mono: "BB", div: "East", color: "#c89a1f", ink: "#1b1815", w: 52, l: 50, pct: ".510", gb: "12", streak: "W1", l10: "5-5", home: "29-22", away: "23-28", rs: 433, ra: 440, rd: "-7"   },
  { id: "psf", city: "San Francisco", nick: "Seals",      abbr: "SFS", mono: "SF", div: "West", color: "#3b7a6b", ink: "#f6ecd2", w: 51, l: 51, pct: ".500", gb: "—",  streak: "W3", l10: "7-3", home: "30-21", away: "21-30", rs: 444, ra: 451, rd: "-7"   },
  { id: "ind", city: "Indianapolis",  nick: "Federals",   abbr: "IND", mono: "IF", div: "Central", color: "#9b2a2a", ink: "#f6ecd2", w: 48, l: 54, pct: ".471", gb: "12", streak: "L3", l10: "3-7", home: "26-25", away: "22-29", rs: 416, ra: 451, rd: "-35"  },
  { id: "tpk", city: "Topeka",        nick: "Drovers",    abbr: "TPK", mono: "TD", div: "West", color: "#7a4a1e", ink: "#f6ecd2", w: 47, l: 55, pct: ".461", gb: "4",  streak: "W1", l10: "5-5", home: "24-27", away: "23-28", rs: 422, ra: 458, rd: "-36"  },
  { id: "scr", city: "Sacramento",    nick: "Solons",     abbr: "SAC", mono: "SS", div: "West", color: "#8f2727", ink: "#f6ecd2", w: 46, l: 56, pct: ".451", gb: "5",  streak: "L1", l10: "4-6", home: "25-26", away: "21-30", rs: 419, ra: 462, rd: "-43"  },
  { id: "nor", city: "New Orleans",   nick: "Crescents",  abbr: "NOR", mono: "NC", div: "West", color: "#2a5b6e", ink: "#c89a1f", w: 44, l: 58, pct: ".431", gb: "7",  streak: "W2", l10: "6-4", home: "23-28", away: "21-30", rs: 411, ra: 469, rd: "-58"  },
  { id: "mtl", city: "Montreal",      nick: "Royaux",     abbr: "MTL", mono: "MR", div: "East", color: "#2c2c2c", ink: "#c89a1f", w: 39, l: 63, pct: ".382", gb: "25", streak: "L5", l10: "2-8", home: "21-30", away: "18-33", rs: 391, ra: 491, rd: "-100" },
];

function teamById(id) { return TEAMS.find(t => t.id === id) || TEAMS[0]; }

// Player headshots: we'll draw stylized monogram cards
const PLAYERS = [
  { id: "p1",  team: "brk", num: 9,  name: "Whitey Sutter",   pos: "CF", bats: "L", thrs: "L", avg: ".341", hr: 22, rbi: 71, ops: ".984", war: 5.6, age: 27 },
  { id: "p2",  team: "stp", num: 14, name: "Cal Henderson",   pos: "1B", bats: "R", thrs: "R", avg: ".322", hr: 31, rbi: 96, ops: "1.001", war: 5.4, age: 30 },
  { id: "p3",  team: "det", num: 5,  name: "Marco DiPietro",  pos: "3B", bats: "R", thrs: "R", avg: ".308", hr: 27, rbi: 88, ops: ".942", war: 4.9, age: 26 },
  { id: "p4",  team: "phl", num: 22, name: "Lucky Brennan",   pos: "RF", bats: "L", thrs: "R", avg: ".316", hr: 24, rbi: 80, ops: ".948", war: 5.1, age: 28 },
  { id: "p5",  team: "cle", num: 7,  name: "Eddie Mosley",    pos: "SS", bats: "B", thrs: "R", avg: ".302", hr: 14, rbi: 62, ops: ".857", war: 4.6, age: 24 },
  { id: "p6",  team: "buf", num: 33, name: "Reggie Caldwell", pos: "LF", bats: "R", thrs: "L", avg: ".296", hr: 28, rbi: 91, ops: ".921", war: 4.4, age: 31 },
  { id: "p7",  team: "ind", num: 12, name: "Otis Brown",      pos: "2B", bats: "L", thrs: "R", avg: ".311", hr: 11, rbi: 54, ops: ".861", war: 4.0, age: 25 },
  { id: "p8",  team: "psf", num: 4,  name: "Hank Forrester",  pos: "C",  bats: "R", thrs: "R", avg: ".289", hr: 19, rbi: 73, ops: ".839", war: 3.8, age: 29 },
];

// Pitchers
const PITCHERS = [
  { id: "pp1", team: "brk", num: 31, name: "Vince Holloway",  pos: "SP", w: 16, l: 4,  era: "2.42", whip: "0.98", k: 198, ip: "182.0" },
  { id: "pp2", team: "stp", num: 28, name: "Lou Whitcomb",    pos: "SP", w: 14, l: 6,  era: "2.71", whip: "1.04", k: 178, ip: "176.1" },
  { id: "pp3", team: "det", num: 19, name: "Roy Kasprzak",    pos: "SP", w: 13, l: 7,  era: "2.88", whip: "1.11", k: 162, ip: "178.0" },
  { id: "pp4", team: "phl", num: 41, name: "Buster Lamont",   pos: "SP", w: 12, l: 5,  era: "3.01", whip: "1.14", k: 154, ip: "170.2" },
  { id: "pp5", team: "cle", num: 24, name: "Hal Brennan",     pos: "SP", w: 12, l: 8,  era: "3.18", whip: "1.18", k: 151, ip: "173.0" },
];

// Brooklyn Royals roster (sample)
const BRK_ROSTER = [
  { num: 9,  name: "Whitey Sutter",    pos: "CF", b: "L", t: "L", age: 27, g: 102, ab: 401, h: 137, hr: 22, rbi: 71, bb: 58, k: 73, sb: 18, avg: ".341", obp: ".421", slg: ".563", ops: ".984", war: 5.6 },
  { num: 31, name: "Vince Holloway",   pos: "SP", b: "R", t: "R", age: 29, g: 24,  w: 16, l: 4, era: "2.42", whip: "0.98", k: 198, ip: "182.0", war: 5.4 },
  { num: 7,  name: "Mickey Olivares",  pos: "SS", b: "R", t: "R", age: 25, g: 99,  ab: 388, h: 116, hr: 12, rbi: 51, bb: 41, k: 81, sb: 14, avg: ".299", obp: ".374", slg: ".459", ops: ".833", war: 4.2 },
  { num: 12, name: "Tommy Reardon",    pos: "1B", b: "L", t: "L", age: 31, g: 101, ab: 391, h: 119, hr: 28, rbi: 88, bb: 49, k: 72, sb: 1,  avg: ".304", obp: ".388", slg: ".556", ops: ".944", war: 3.8 },
  { num: 25, name: "Dom Pilarski",     pos: "C",  b: "R", t: "R", age: 28, g: 87,  ab: 311, h: 86,  hr: 14, rbi: 49, bb: 31, k: 66, sb: 0,  avg: ".277", obp: ".351", slg: ".466", ops: ".817", war: 3.1 },
  { num: 18, name: "Sal Marquez",      pos: "2B", b: "R", t: "R", age: 26, g: 96,  ab: 358, h: 99,  hr: 9,  rbi: 47, bb: 34, k: 58, sb: 12, avg: ".277", obp: ".342", slg: ".411", ops: ".753", war: 3.0 },
  { num: 22, name: "Lou Yoshida",      pos: "3B", b: "L", t: "R", age: 24, g: 98,  ab: 372, h: 110, hr: 11, rbi: 58, bb: 28, k: 64, sb: 6,  avg: ".296", obp: ".346", slg: ".457", ops: ".803", war: 2.9 },
  { num: 14, name: "Pete Daugherty",   pos: "LF", b: "L", t: "L", age: 30, g: 95,  ab: 341, h: 92,  hr: 16, rbi: 55, bb: 38, k: 70, sb: 4,  avg: ".270", obp: ".352", slg: ".481", ops: ".833", war: 2.8 },
  { num: 6,  name: "Buddy Calame",     pos: "RF", b: "R", t: "R", age: 27, g: 92,  ab: 332, h: 91,  hr: 13, rbi: 47, bb: 26, k: 78, sb: 9,  avg: ".274", obp: ".334", slg: ".440", ops: ".774", war: 2.5 },
  { num: 28, name: "Russ Petrocelli",  pos: "SP", b: "R", t: "R", age: 26, g: 22,  w: 11, l: 5, era: "3.04", whip: "1.16", k: 142, ip: "159.1", war: 3.4 },
  { num: 36, name: "Cliff Banuelos",   pos: "SP", b: "L", t: "L", age: 28, g: 23,  w: 10, l: 7, era: "3.22", whip: "1.21", k: 138, ip: "164.2", war: 3.1 },
  { num: 44, name: "Ace Coughlin",     pos: "RP", b: "R", t: "R", age: 31, g: 58,  w: 4,  l: 2, era: "1.88", whip: "0.94", k: 78,  ip: "62.0",  war: 2.2 },
];

// Brooklyn's recent results + upcoming
const BRK_SCHEDULE = [
  { date: "JUN 12", opp: "phl", ha: "@", res: "W 6-2", wp: "Holloway",   sv: null },
  { date: "JUN 13", opp: "phl", ha: "@", res: "W 4-3", wp: "Petrocelli", sv: "Coughlin" },
  { date: "JUN 14", opp: "phl", ha: "@", res: "L 2-5", wp: null,         sv: null },
  { date: "JUN 15", opp: "buf", ha: "vs", res: "W 7-1", wp: "Banuelos",   sv: null },
  { date: "JUN 16", opp: "buf", ha: "vs", res: "W 5-4", wp: "Coughlin",   sv: null },
  { date: "JUN 17", opp: "ind", ha: "vs", res: "FINAL · W 9-2", wp: "Holloway", sv: null },
];

// Today's games — drives the scoreboard
const TODAYS_GAMES = [
  { id: "g1", away: "stp", home: "brk", status: "LIVE",   inning: "TOP 7TH",  ar: 3, hr: 5, baseState: "1st & 3rd", outs: 1, winProb: 78 },
  { id: "g2", away: "cle", home: "det", status: "LIVE",   inning: "BOT 4TH",  ar: 2, hr: 1, baseState: "Bases empty", outs: 2, winProb: 42 },
  { id: "g3", away: "phl", home: "buf", status: "LIVE",   inning: "BOT 9TH",  ar: 4, hr: 4, baseState: "1st",          outs: 2, winProb: 61, walkoff: true },
  { id: "g4", away: "mtl", home: "ind", status: "FINAL",  inning: "F/10",      ar: 6, hr: 7 },
  { id: "g5", away: "tpk", home: "psf", status: "FINAL",  inning: "FINAL",     ar: 2, hr: 8 },
  { id: "g6", away: "nor", home: "scr", status: "7:05 PM", inning: "FIRST PITCH 7:05 PM", pitchA: "Renteria", pitchH: "Knepper" },
  { id: "g7", away: "brk", home: "cle", status: "7:35 PM", inning: "FIRST PITCH 7:35 PM", pitchA: "Holloway", pitchH: "Hal Brennan" },
  { id: "g8", away: "stp", home: "phl", status: "8:05 PM", inning: "FIRST PITCH 8:05 PM", pitchA: "Whitcomb", pitchH: "Lamont"   },
];

// News rail
const NEWS = [
  { eye: "Pennant Race", head: "Royals Tighten Grip on the East with Three-Game Sweep in Philadelphia", meta: "by Howard Linnell · 2h ago · TBL Wire", thumbAccent: true, mono: "BR" },
  { eye: "Trade Wire",   head: "Wolverines Acquire Veteran Catcher From Solons For Two Prospects",      meta: "by Della McGraw · 4h ago", mono: "DT" },
  { eye: "Sim Recap",    head: "Bisons Walk-Off Phillies on Cabrera Single in the Ninth",                meta: "Auto-recap · 32 min ago", mono: "BB", live: true },
  { eye: "Commissioner", head: "Twilight League Releases Updated Park Factors for the 1958 Replay",      meta: "League Office · 1d ago", mono: "T", ink: true },
  { eye: "Sabermetric",  head: "Why Whitey Sutter's xwOBA Says The MVP Race is Already Over",            meta: "by Jules Wexler · 1d ago", mono: "★" },
  { eye: "Injury Notes", head: "Pilarski to the 7-Day for Hamstring Strain; Halliburton Recalled",       meta: "Brooklyn beat · 1d ago", mono: "BR" },
];

// Leaders
const LEADERS = {
  AVG: [
    { name: "Whitey Sutter", team: "BRK", v: ".341" },
    { name: "Cal Henderson", team: "STP", v: ".322" },
    { name: "Lucky Brennan", team: "PHL", v: ".316" },
    { name: "Otis Brown",    team: "IND", v: ".311" },
    { name: "Marco DiPietro",team: "DET", v: ".308" },
  ],
  HR: [
    { name: "Cal Henderson", team: "STP", v: "31" },
    { name: "Reggie Caldwell", team: "BUF", v: "28" },
    { name: "Tommy Reardon", team: "BRK", v: "28" },
    { name: "Marco DiPietro", team: "DET", v: "27" },
    { name: "Lucky Brennan", team: "PHL", v: "24" },
  ],
  RBI: [
    { name: "Cal Henderson", team: "STP", v: "96" },
    { name: "Reggie Caldwell", team: "BUF", v: "91" },
    { name: "Marco DiPietro", team: "DET", v: "88" },
    { name: "Tommy Reardon", team: "BRK", v: "88" },
    { name: "Lucky Brennan", team: "PHL", v: "80" },
  ],
  ERA: [
    { name: "Vince Holloway", team: "BRK", v: "2.42" },
    { name: "Lou Whitcomb",  team: "STP", v: "2.71" },
    { name: "Roy Kasprzak",  team: "DET", v: "2.88" },
    { name: "Buster Lamont", team: "PHL", v: "3.01" },
    { name: "Hal Brennan",   team: "CLE", v: "3.18" },
  ],
  K: [
    { name: "Vince Holloway", team: "BRK", v: "198" },
    { name: "Lou Whitcomb",  team: "STP", v: "178" },
    { name: "Roy Kasprzak",  team: "DET", v: "162" },
    { name: "Buster Lamont", team: "PHL", v: "154" },
    { name: "Hal Brennan",   team: "CLE", v: "151" },
  ],
};

// Game log for a player
const SUTTER_GAMELOG = [
  { date: "JUN 17", opp: "vs IND", res: "W 9-2",  ab: 4, h: 3, r: 2, hr: 1, rbi: 3, bb: 1, k: 0, avg: ".341" },
  { date: "JUN 16", opp: "vs BUF", res: "W 5-4",  ab: 5, h: 2, r: 1, hr: 0, rbi: 1, bb: 0, k: 1, avg: ".339" },
  { date: "JUN 15", opp: "vs BUF", res: "W 7-1",  ab: 4, h: 2, r: 1, hr: 1, rbi: 2, bb: 1, k: 0, avg: ".339" },
  { date: "JUN 14", opp: "@ PHL",  res: "L 2-5",  ab: 4, h: 1, r: 0, hr: 0, rbi: 0, bb: 0, k: 2, avg: ".338" },
  { date: "JUN 13", opp: "@ PHL",  res: "W 4-3",  ab: 4, h: 2, r: 2, hr: 1, rbi: 2, bb: 0, k: 1, avg: ".340" },
  { date: "JUN 12", opp: "@ PHL",  res: "W 6-2",  ab: 3, h: 1, r: 1, hr: 0, rbi: 0, bb: 2, k: 0, avg: ".338" },
  { date: "JUN 11", opp: "vs MTL", res: "W 8-3",  ab: 5, h: 3, r: 2, hr: 1, rbi: 3, bb: 0, k: 1, avg: ".339" },
  { date: "JUN 10", opp: "vs MTL", res: "L 3-6",  ab: 4, h: 0, r: 0, hr: 0, rbi: 0, bb: 1, k: 2, avg: ".335" },
];

// Career line
const SUTTER_CAREER = [
  { yr: "2024", tm: "BRK", g: 138, ab: 542, h: 161, hr: 18, rbi: 71, avg: ".297", ops: ".872", war: 4.4 },
  { yr: "2025", tm: "BRK", g: 152, ab: 588, h: 188, hr: 24, rbi: 86, avg: ".320", ops: ".921", war: 5.1 },
  { yr: "2026", tm: "BRK", g: 102, ab: 401, h: 137, hr: 22, rbi: 71, avg: ".341", ops: ".984", war: 5.6 },
];

Object.assign(window, {
  LEAGUES, TEAMS, PLAYERS, PITCHERS, BRK_ROSTER, BRK_SCHEDULE, TODAYS_GAMES, NEWS, LEADERS,
  SUTTER_GAMELOG, SUTTER_CAREER, teamById,
});
