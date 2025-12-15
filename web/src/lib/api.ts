export const API_BASE = process.env.NEXT_PUBLIC_API_BASE ?? "http://localhost:5151/api";

export type UserResponse = {
  id: string;
  email: string;
  username: string;
  displayName: string;
  createdAt: string;
  lastVisitedLeagueId?: string;
};

export type LeagueSummaryResponse = {
  id: string;
  name: string;
  slug: string;
  status: string | number;
  teamCount: number;
};

export type AuthResponse = {
  token: string;
  expiresAt: string;
  user: UserResponse;
};

type ApiRequestOptions = {
  method?: string;
  body?: unknown;
  token?: string | null;
  headers?: HeadersInit;
};

export class ApiError extends Error {
  status: number;

  constructor(message: string, status: number) {
    super(message);
    this.status = status;
  }
}

export async function apiRequest<T>(path: string, options: ApiRequestOptions = {}): Promise<T> {
  const { method = "GET", body, token, headers } = options;
  const response = await fetch(`${API_BASE}${path}`, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const message = await safeErrorMessage(response);
    throw new ApiError(message, response.status);
  }

  if (response.status === 204) {
    return {} as T;
  }

  return (await response.json()) as T;
}

async function safeErrorMessage(response: Response) {
  try {
    const data = await response.json();
    if (data?.message) return data.message as string;
    if (typeof data === "string") return data;
  } catch {
    // Ignore parse errors and fall through
  }

  return response.statusText || "Request failed";
}
