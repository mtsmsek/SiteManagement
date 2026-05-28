# 0006 — Authorization pipeline with single-marker request types

- **Status:** Accepted
- **Date:** 2026-05-18

## Context

By the time the resident portal landed (W5), authorisation had three distinct
shapes the system had to enforce on every request: "is the caller an admin?",
"is the caller a resident?", and (separately) "does this *specific* dues
item / conversation belong to *this* resident?". The default ASP.NET options
are `[Authorize(Roles="Admin")]` per controller and an `if (item.ResidentId
!= currentUser.ResidentId)` check per handler.

Both leak. A new endpoint that someone forgets to decorate ships open. A
handler that the author forgot to put the ownership `if` in is an IDOR. The
build doesn't catch either. And once handlers are doing authz, the whole
"handler does *one* business job" payoff (ADR 0005) is gone.

## Decision

**Authorisation is a MediatR pipeline concern; handlers never reach for it.**

Every MediatR request implements **exactly one** role marker — one of
`IAdminRequest`, `IResidentRequest`, `IPublicRequest`. A single
`AuthorizationBehavior` reads the marker and checks `ICurrentUser`
accordingly:

- `IAdminRequest` → must have the Admin role.
- `IResidentRequest` → must have the Resident role *and* a non-null
  `ResidentId` claim.
- `IPublicRequest` → explicitly unauthenticated (login, password reset, etc.).

A request without any marker, or with more than one, **fails the build**:
`AuthorizationConventionsTests` (a NetArchTest case) inspects every
request type and refuses the assembly if the marker isn't exactly one.
Forgetting to declare authz is no longer possible.

Resource ownership is its own pair of pipeline behaviors. Any request that
touches a per-resident resource implements either `IOwnedBillItemRequest`
or `IOwnedConversationRequest` (which exposes the resource id); the
matching ownership behavior calls the read side and verifies the resource
belongs to `ICurrentUser.ResidentId`. Again: handlers don't do the check.

## Alternatives considered

- **`[Authorize(Roles=...)]` on controllers** — works for the role part but
  leaves ownership in handlers (the `if (item.ResidentId != ...)` pattern),
  which is exactly where IDORs hide. Also no compile-time guarantee that
  every endpoint is decorated.
- **One behavior per role (one for admin, one for resident)** — duplicates
  the pipeline wiring; harder to reason about request flow.
- **Resource-based `IAuthorizationHandler` from ASP.NET** — solves a
  similar problem but is policy-keyed (string identifiers in attributes);
  the marker-interface approach makes the intent visible in the request
  type itself, and the architecture test can iterate over markers in C#
  rather than parse attribute strings.

## Consequences

- **Positive:** every authz decision is in two files (the two ownership
  behaviors) plus one (`AuthorizationBehavior`). A new endpoint costs one
  marker declaration; forgetting it fails the build via
  `AuthorizationConventionsTests`. Handlers are tiny again.
  **IDOR is structurally impossible** on the resident side (the token-scoped
  endpoints — ADR 0009 — combine with the ownership behavior to make
  "another resident's id" inexpressible).
- **Negative:** an unfamiliar contributor has to learn *why* their command
  needs the marker on its request type. The ergonomics are better than
  attribute soup once they have, but the upfront pattern read is real.
- **Guardrail:** `AuthorizationConventionsTests` enforces the
  single-marker rule. Ownership coverage is enforced by E2E tests
  (`ResidentPortalFlowTests`, `MessagingFlowTests`) that fire negative
  cases — resident A asking for resident B's item — and assert 403.
