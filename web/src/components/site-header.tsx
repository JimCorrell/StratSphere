"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useAuth } from "./auth-provider";

export function SiteHeader() {
  const { user, logout } = useAuth();
  const pathname = usePathname();
  const router = useRouter();

  const isAuthPage = pathname === "/login";

  return (
    <header className="site-header">
      <Link href="/" className="brand">
        StratSphere
      </Link>
      <div className="header-actions">
        {user ? (
          <div className="user-chip">
            <span className="user-name">{user.displayName || user.username}</span>
            <Link className="ghost-button" href="/dashboard">
              Dashboard
            </Link>
            <button
              className="ghost-button"
              onClick={() => {
                logout();
                router.push("/login");
              }}
            >
              Log out
            </button>
            <Link className="primary-button" href="/account">
              Account
            </Link>
          </div>
        ) : (
          <div className="user-chip">
            {!isAuthPage && (
              <Link className="ghost-button" href="/login">
                Log in
              </Link>
            )}
            <Link className="primary-button" href="/login">
              Get started
            </Link>
          </div>
        )}
      </div>
    </header>
  );
}
