"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/components/auth-provider";
import { apiRequest, LeagueSummaryResponse } from "@/lib/api";

export default function DashboardPage() {
  const { user, token, initializing } = useAuth();
  const router = useRouter();
  const [selectedLeague, setSelectedLeague] = useState<LeagueSummaryResponse | null>(null);
  const [leagues, setLeagues] = useState<LeagueSummaryResponse[]>([]);
  const [loadingLeague, setLoadingLeague] = useState(true);

  useEffect(() => {
    if (!initializing && !user) {
      router.replace("/login");
    }
  }, [user, initializing, router]);

  // Load user's leagues
  useEffect(() => {
    if (!token) return;

    let cancelled = false;

    apiRequest<LeagueSummaryResponse[]>("/auth/me/leagues", { token })
      .then((data) => {
        if (!cancelled) {
          setLeagues(data);

          // Try to load last visited league
          if (user?.lastVisitedLeagueId) {
            const found = data.find((l) => l.id === user.lastVisitedLeagueId);
            if (found) {
              setSelectedLeague(found);
            } else if (data.length > 0) {
              setSelectedLeague(data[0]);
            }
          } else if (data.length > 0) {
            setSelectedLeague(data[0]);
          }

          setLoadingLeague(false);
        }
      })
      .catch(() => {
        if (!cancelled) {
          setLoadingLeague(false);
        }
      });

    return () => {
      cancelled = true;
    };
  }, [token, user?.lastVisitedLeagueId]);

  if (!user && initializing) {
    return (
      <div className="card">
        <div className="skeleton" style={{ width: "40%", marginBottom: "10px" }} />
        <div className="skeleton" style={{ width: "60%", marginBottom: "10px" }} />
      </div>
    );
  }

  if (!user) return null;

  return (
    <>
      {loadingLeague ? (
        <div className="card">
          <div className="skeleton" style={{ width: "80%" }} />
        </div>
      ) : selectedLeague ? (
        <div className="dashboard-container">
          <div className="dashboard-main">
            <div className="hero-graph card">
              <div className="hero-header">
                <h2>{selectedLeague.name}</h2>
                <p className="lead">{selectedLeague.teamCount} teams</p>
              </div>
              <div className="graph-placeholder">
                <p className="helper">Standings graph will display here</p>
              </div>
            </div>
          </div>

          <div className="dashboard-sidebar">
            <div className="sidebar-section card">
              <h3>League News</h3>
              <div className="section-content">
                <p className="helper">No news yet. Check back soon!</p>
              </div>
            </div>

            <div className="sidebar-section card">
              <h3>Hot Stove Rumors</h3>
              <div className="section-content">
                <p className="helper">No rumors yet. Check back soon!</p>
              </div>
            </div>
          </div>
        </div>
      ) : (
        <div className="card">
          <h2>No leagues available</h2>
          <p className="lead">You are not a member of any leagues yet.</p>
        </div>
      )}
    </>
  );
}
