"use client";

import React, { useCallback, useContext, useEffect, useMemo, useState } from "react";
import { apiRequest, ApiError, AuthResponse, UserResponse } from "@/lib/api";

const TOKEN_KEY = "stratsphere:token";

type AuthContextValue = {
  user: UserResponse | null;
  token: string | null;
  loading: boolean;
  initializing: boolean;
  error: string | null;
  login: (emailOrUsername: string, password: string) => Promise<void>;
  logout: () => void;
  refreshUser: () => Promise<void>;
};

const AuthContext = React.createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserResponse | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [initializing, setInitializing] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadUser = useCallback(
    async (tokenValue: string) => {
      setLoading(true);
      try {
        const me = await apiRequest<UserResponse>("/auth/me", { token: tokenValue });
        setUser(me);
        setError(null);
      } catch (err) {
        setUser(null);
        setToken(null);
        localStorage.removeItem(TOKEN_KEY);
        const message = err instanceof ApiError ? err.message : "Session expired";
        setError(message);
      } finally {
        setLoading(false);
        setInitializing(false);
      }
    },
    []
  );

  useEffect(() => {
    const stored = localStorage.getItem(TOKEN_KEY);
    if (stored) {
      setToken(stored);
      loadUser(stored);
    } else {
      setInitializing(false);
    }
  }, [loadUser]);

  const login = useCallback(async (emailOrUsername: string, password: string) => {
    setLoading(true);
    try {
      const response = await apiRequest<AuthResponse>("/auth/login", {
        method: "POST",
        body: { EmailOrUsername: emailOrUsername, Password: password },
      });

      setToken(response.token);
      setUser(response.user);
      localStorage.setItem(TOKEN_KEY, response.token);
      setError(null);
    } catch (err) {
      const message = err instanceof ApiError ? err.message : "Login failed";
      setError(message);
      throw err;
    } finally {
      setLoading(false);
      setInitializing(false);
    }
  }, []);

  const logout = useCallback(() => {
    setUser(null);
    setToken(null);
    localStorage.removeItem(TOKEN_KEY);
  }, []);

  const refreshUser = useCallback(async () => {
    if (!token) return;
    await loadUser(token);
  }, [loadUser, token]);

  const value = useMemo(
    () => ({ user, token, loading, initializing, error, login, logout, refreshUser }),
    [user, token, loading, initializing, error, login, logout, refreshUser]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth must be used inside AuthProvider");
  }
  return ctx;
}
