"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useEffect, useMemo, useState } from "react";
import { useAuth } from "@/components/auth-provider";

const demoAccounts = [
  { label: "Admin", email: "admin@stratsphere.local", password: "Admin123!" },
  { label: "USBL", email: "usbl@stratsphere.local", password: "Usbl123!" },
];

export default function LoginPage() {
  const { user, login, loading, initializing } = useAuth();
  const router = useRouter();
  const [emailOrUsername, setEmailOrUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const disableSubmit = useMemo(() => !emailOrUsername || !password || submitting, [
    emailOrUsername,
    password,
    submitting,
  ]);

  useEffect(() => {
    if (!initializing && user) {
      router.push("/account");
    }
  }, [user, initializing, router]);

  async function handleSubmit(evt: FormEvent<HTMLFormElement>) {
    evt.preventDefault();
    setSubmitting(true);
    setError(null);

    try {
      await login(emailOrUsername, password);
      router.push("/account");
    } catch (err) {
      const message = err instanceof Error ? err.message : "Unable to sign in";
      setError(message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="grid">
      <div className="card form-card">
        <div className="tag">Secure login</div>
        <h1 className="headline" style={{ fontSize: "28px" }}>
          Enter the StratSphere console.
        </h1>
        <p className="lead">
          Use the seeded credentials or your own account created via the API. Passwords never leave the browser except to the StratSphere API.
        </p>
        <form className="form-card" onSubmit={handleSubmit}>
          <div className="form-row">
            <label htmlFor="emailOrUsername">Email or username</label>
            <input
              id="emailOrUsername"
              className="input"
              autoComplete="username"
              placeholder="admin@stratsphere.local"
              value={emailOrUsername}
              onChange={(e) => setEmailOrUsername(e.target.value)}
            />
          </div>
          <div className="form-row">
            <label htmlFor="password">Password</label>
            <input
              id="password"
              type="password"
              className="input"
              autoComplete="current-password"
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>
          {error && <div className="error">{error}</div>}
          <button className="primary-button" type="submit" disabled={disableSubmit || loading}>
            {submitting || loading ? "Signing in…" : "Sign in"}
          </button>
          <p className="helper">
            Tokens are stored locally and attached as <code className="mono">Authorization: Bearer</code> on every request.
          </p>
        </form>
      </div>
      <div className="card">
        <h3>Demo accounts</h3>
        <p className="lead" style={{ fontSize: "14px", marginBottom: "10px" }}>
          These are seeded in the database. Admin can see every league; USBL user is scoped to the USBL league.
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
        <p className="helper">
          Need an account? Use the API&apos;s <code className="mono">/auth/register</code> endpoint to create one, then sign in here.
        </p>
        <div className="action-row">
          <Link className="ghost-button" href="/">
            Back to overview
          </Link>
        </div>
      </div>
    </div>
  );
}