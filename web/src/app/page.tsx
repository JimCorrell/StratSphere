import Link from "next/link";

const demoAccounts = [
  { label: "Admin", email: "admin@stratsphere.local", password: "Admin123!" },
  { label: "USBL", email: "usbl@stratsphere.local", password: "Usbl123!" },
];

export default function Home() {
  return (
    <>
      <section className="hero">
        <div className="card">
          <div className="tag">Early access</div>
          <h1 className="headline">
            Run your Strat-o-Matic league like a front office.
          </h1>
          <p className="lead">
            Authenticate with the API, see only the leagues you belong to, and keep commissioners in the driver&apos;s seat. This UI is wired to the new auth endpoints.
          </p>
          <div className="pill-row">
            {demoAccounts.map((acct) => (
              <span className="pill" key={acct.email}>
                <strong>{acct.label}</strong>
                <span className="mono">{acct.email}</span>
                <span className="mono">{acct.password}</span>
              </span>
            ))}
          </div>
          <div className="action-row">
            <Link className="primary-button" href="/login">
              Open console
            </Link>
            <Link className="muted-link" href="/account">
              View account
            </Link>
          </div>
        </div>
        <div className="grid">
          <div className="card">
            <h3>League-aware access</h3>
            <p>
              Admins see everything; other managers only see the leagues they belong to via <code className="mono">/auth/me</code> and <code className="mono">/auth/me/leagues</code>.
            </p>
          </div>
          <div className="card">
            <h3>JWT ready</h3>
            <p>
              Tokens are persisted client-side and attached to every request. Expired or invalid sessions automatically sign out.
            </p>
          </div>
          <div className="card">
            <h3>Next.js app router</h3>
            <p>
              Built with React Server Components where possible and client islands for auth-aware UI. Styled with a custom theme—no boilerplate kits.
            </p>
          </div>
        </div>
      </section>
    </>
  );
}
