// Same-origin fetch client for api/webhooks (cookie auth works: the host is same-origin).
async function http<T>(method: string, url: string, body?: unknown): Promise<T> {
  const res = await fetch(url, {
    method,
    credentials: "same-origin",
    headers: body === undefined ? { Accept: "application/json" } : { Accept: "application/json", "Content-Type": "application/json" },
    body: body === undefined ? undefined : JSON.stringify(body),
  });
  const text = await res.text();
  if (!res.ok) {
    let message = text;
    try {
      const json = JSON.parse(text);
      message = typeof json === "string" ? json : (json.message ?? json.title ?? text);
    } catch {
      // not JSON: keep the text
    }
    throw new Error(message || `${res.status} ${res.statusText}`);
  }
  return (text ? JSON.parse(text) : undefined) as T;
}

export type AuthType = "None" | "Basic" | "BearerToken" | "CustomHeader";

export interface WebhookEvent {
  eventId?: string;
  displayName?: string;
}

export interface Webhook {
  id?: string;
  name?: string;
  url?: string;
  contentType?: string;
  isActive?: boolean;
  isAllEvents?: boolean;
  authType?: AuthType;
  basicUsername?: string;
  basicPassword?: string;
  bearerToken?: string;
  customHttpHeaderName?: string;
  customHttpHeaderValue?: string;
  successCount?: number;
  errorCount?: number;
  events?: WebhookEvent[];
  payloads?: { eventPropertyName?: string }[];
}

export interface FeedEntry {
  id?: string;
  webHookId?: string;
  eventId?: string;
  attemptCount?: number;
  error?: string;
  createdDate?: string;
  modifiedDate?: string;
}

export interface SearchResult<T> {
  totalCount?: number;
  results?: T[];
}

// record type 1 = error (WebhookFeedEntryType.Error)
export const ERROR_RECORD = 1;

// a webhook on several events (or all) is read-only in this UI, as in the AngularJS one
export const isMultiEvent = (w: Webhook) => (w.events?.length ?? 0) > 1 || !!w.isAllEvents;

const ids = (list: string[]) => list.map((x) => `ids=${encodeURIComponent(x)}`).join("&");

export const api = {
  search: (criteria: Record<string, unknown>) => http<SearchResult<Webhook>>("POST", "/api/webhooks/search", criteria),
  get: (id: string) => http<Webhook>("GET", `/api/webhooks/${encodeURIComponent(id)}`),
  save: (webhooks: Webhook[]) => http<Webhook[]>("POST", "/api/webhooks", webhooks),
  delete: (list: string[]) => http<void>("DELETE", `/api/webhooks?${ids(list)}`),
  events: () => http<{ id: string; displayName?: string }[]>("GET", "/api/webhooks/events"),
  properties: (eventId: string) =>
    http<{ discovered?: boolean; properties?: string[] }>("GET", `/api/webhooks/properties?objectType=${encodeURIComponent(eventId)}`),
  searchFeed: (criteria: Record<string, unknown>) => http<SearchResult<FeedEntry>>("POST", "/api/webhooks/feed/search", criteria),
  deleteFeed: (list: string[]) => http<void>("DELETE", `/api/webhooks/feed?${ids(list)}`),
};

// The registered events don't change while the platform runs: requested once (again after a failure)
let eventList: Promise<{ id: string; displayName?: string }[]> | undefined;
export const registeredEvents = () =>
  (eventList ??= api.events().catch((e) => {
    eventList = undefined;
    throw e;
  }));

// The platform's merged module localization: event names live under "webhooks.events.types.<event id>"
let texts: Promise<Record<string, unknown>> | undefined;
const loadTexts = () =>
  (texts ??= http<Record<string, unknown>>("GET", "/api/platform/localization?lang=en").catch(() => ({}) as Record<string, unknown>));

// Event id → title: the translated name, else the registered display name ("Blob Created Event")
let titles: Promise<Record<string, string>> | undefined;
export function eventNames() {
  return (titles ??= Promise.all([loadTexts(), registeredEvents().catch(() => [])]).then(([all, events]) => {
    const types = ((all.webhooks as Record<string, unknown>)?.events as Record<string, unknown>)?.types as Record<string, string> | undefined;
    return { ...Object.fromEntries(events.map((e) => [e.id, e.displayName ?? e.id])), ...types };
  }));
}
