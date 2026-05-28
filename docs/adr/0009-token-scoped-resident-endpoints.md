# 0009 — Token-scoped resident endpoints (`/api/me/*`)

- **Status:** Accepted
- **Date:** 2026-05-18

## Context

The resident portal lets a sakin read their own bills, pay an unpaid
item, and message the admin. The naive endpoint shape is the same one the
admin side already uses: `GET /api/residents/{residentId}/bills`,
`POST /api/dues/{periodId}/items/{itemId}/pay-by-card`, … with
`[Authorize(Roles="Resident")]` on the controller and an
`if (residentId != currentUser.ResidentId) return Forbid();` inside the
handler.

That shape is the textbook IDOR setup. The resident now knows the URL
template; the only thing standing between them and another resident's
bills is the handler `if`, which is easy to forget when a new endpoint
goes in.

A senior reader of the codebase looks for *structural* fixes to IDOR,
not "a guard check on every handler."

## Decision

**Resident endpoints live under `/api/me/*` and never carry the resident
id in the route or the body.** The handler resolves the caller from the
token (`ICurrentUser.ResidentId`, which is the `resident_id` claim).
There is no client-supplied id to validate, so the canonical IDOR
shape ("substitute another id and see what comes back") is *inexpressible*.

For commands that target a specific item (pay-by-card, mark-conversation-read),
the request is an `IOwnedBillItemRequest` / `IOwnedConversationRequest`
(see ADR 0006). The ownership pipeline behavior verifies the item
belongs to the caller's `ResidentId` — the *server* decides the
relationship, never the client.

The admin side keeps the route-keyed shape
(`/api/residents/{residentId}/...`) because admins legitimately address
other residents. The two URL spaces don't overlap.

## Alternatives considered

- **Shared route shape + per-handler ownership check** — the textbook
  IDOR setup; rejected on the "structural over guard" principle.
- **Resource-based authorization handler in ASP.NET** — works, but the
  IDOR vulnerability surface is the *route* itself, not the access check.
  Take the id out of the route and there is nothing to check.
- **GraphQL-style "me" root field** — same shape, more machinery. The
  REST URL space already gave us `/api/me`.

## Consequences

- **Positive:** the resident-side route surface is small (`/api/me/bills`,
  `/api/me/conversations`, `/api/me/dues/.../pay-by-card`, …) and
  reads as a personal pronoun, not as an entity-relationship diagram.
  A new resident-facing endpoint costs one route and zero ownership-
  check code in the handler.
- **Negative:** there are now two parallel command types per pay flow —
  `PayDuesItem` (admin, route-keyed) and `PayMyDuesItem` (resident,
  token-scoped). They share a `IBillItemPaymentService` so the actual
  charge logic isn't duplicated, but the dispatch shape is two.
- **Guardrail:** E2E tests (`ResidentPortalFlowTests`, `MessagingFlowTests`)
  fire the negative case: resident A authenticates and asks for a
  resource that belongs to resident B; the response is 403. Both
  directions tested. The structural property (route carries no id)
  is enforced by code review — a `/api/me/{residentId}/...` route would
  be the kind of obvious smell the pattern is designed to surface.
