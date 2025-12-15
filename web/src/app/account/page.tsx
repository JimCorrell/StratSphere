"use client";

import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/components/auth-provider";
import { ApiError, LeagueSummaryResponse, apiRequest } from "@/lib/api";

export default function AccountPage() {
  const { user, token, initializing } = useAuth();
  const router = useRouter();
  const [leagues, setLeagues] = useState<LeagueSummaryResponse[]>([]);
  const [fetching, setFetching] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createdDate = useMemo(() => {
    if (!user?.createdAt) return null;
    return new Date(user.createdAt).toLocaleDateString();
  }, [user]);

  useEffect(() => {
    if (!initializing && !user) {
      router.replace("/login");
    }
  }, [user, initializing, router]);

  useEffect(() => {
    if (!token) return;
    let cancelled = false;
    // This effect is responsible for pulling league data once a token exists.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setFetching(true);
    apiRequest<LeagueSummaryResponse[]>("/auth/me/leagues", { token })
      .then((data) => {
        if (!cancelled) {
          setLeagues(data);
          setError(null);
        }
      })
      .catch((err) => {
        if (!cancelled) {
          const message = err instanceof ApiError ? err.message : "Could not load leagues";
          setError(message);
        }
      })
      .finally(() => {
        if (!cancelled) setFetching(false);
      });

    return () => {
      cancelled = true;
    };
  }, [token]);

  if (!user && initializing) {
    return (
      <div className="card">
        <div className="skeleton" style={{ width: "40%", marginBottom: "10px" }} />
        <div className="skeleton" style={{ width: "60%", marginBottom: "10px" }} />
        <div className="skeleton" style={{ width: "50%" }} />
      </div>
    );
  }

  if (!user) return null;

  return (
    <div className="grid">
      <div className="card">
        <div className="tag">Authenticated</div>
        <h1 className="headline" style={{ fontSize: "30px" }}>
          Hey {user.displayName || user.username}.
        </h1>
        <p className="lead">
          This view is backed by <code className="mono">/auth/me</code> and your JWT. Admin accounts return every league; other users return only the leagues they belong to.
        </p>
        <div className="pill-row" style={{ marginTop: "14px" }}>
          <span className="pill">
            <strong>Email</strong>
            <span className="mono">{user.email}</span>
          </span>
          <span className="pill">
            <strong>Username</strong>
            <span className="mono">{user.username}</span>
          </span>
          {createdDate && (
            <span className="pill">
              <strong>Joined</strong>
              <span className="mono">{createdDate}</span>
            </span>
          )}
        </div>
      </div>

      <div className="card">
        <h3>Leagues you can see</h3>
        <p className="helper" style={{ marginBottom: "10px" }}>
          Data comes from <code className="mono">/auth/me/leagues</code>. USBL user returns a single league; Admin returns all of them.
        </p>
        {error && <div className="error" style={{ marginBottom: "10px" }}>{error}</div>}
        {fetching ? (
          <div className="grid">
            {Array.from({ length: 3 }).map((_, idx) => (
              <div className="league-card" key={idx}>
                <div className="skeleton" style={{ width: "60%" }} />
                <div className="skeleton" style={{ width: "40%" }} />
              </div>
            ))}
          </div>
        ) : leagues.length === 0 ? (
          <p className="lead">No leagues yet for this account.</p>
        ) : (
          <div className="leagues-grid">
            {leagues.map((league) => (
              <button
                key={league.id}
                className="league-card"
                style={{
                  cursor: "pointer",
                  background: "linear-gradient(140deg, rgba(15, 27, 45, 0.94), rgba(17, 37, 62, 0.9))",
                  border: "1px solid var(--border)",
                  borderRadius: "var(--radius)",
                  padding: "14px 16px",
                  display: "grid",
                  gap: "6px",
                  transition: "all 200ms ease",
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.borderColor = "var(--accent)";
                  e.currentTarget.style.boxShadow = "0 8px 30px rgba(0, 198, 255, 0.15)";
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.borderColor = "var(--border)";
                  e.currentTarget.style.boxShadow = "none";
                }}
                onClick={() => {
                  router.push("/dashboard");
                }}
              >
                <div className="tag">{league.status}</div>
                <h3>{league.name}</h3>
                <p className="helper">Slug: {league.slug}</p>
                <p className="helper">Teams: {league.teamCount}</p>
              </button>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
