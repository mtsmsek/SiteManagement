# 0003 — Modular monolith with one external Payment microservice

- **Status:** Accepted
- **Date:** 2026-03-01

## Context

Two forces pull in opposite directions:

1. The project's PDF brief mandates **a separate microservice for payments
   backed by MongoDB** — explicitly a polyglot-persistence teaching point.
2. The rest of the system (Property, Residency, Tenancy, Billing, Messaging,
   Identity) is a single product owner, a single deployment target, a single
   relational store. Splitting *that* into microservices buys nothing but
   distributed-transaction headaches.

The reader of the code (a hiring committee) needs to see that the author
understands what a "modular monolith" actually is — not "everything piled
into one place" but "one deployment, many independent modules inside it" —
and that they didn't reach for microservices just because microservices are
fashionable.

## Decision

Run the main API as a **modular monolith**: one process, one Postgres
database, one deployment artefact, but inside it six bounded contexts
(Property, Residency, Tenancy, Billing, Messaging, Identity) live in their
own folders + namespaces under `Domain/<Context>` and `Application/<Context>`
and **do not reference each other's types** — they coordinate through
domain events when they need to.

Run **PaymentService as a separate microservice** in its own solution under
`payment-service/`, with its own MongoDB, its own deployment target
(`payment-api:8090`), and its own test suites. Communication is
synchronous HTTP through a port: `IPaymentGateway` in Application,
`PaymentGatewayAdapter` + `IPaymentServiceApi` Refit client in
Infrastructure (the wire details belong outside Application — see ADR
0010).

### Why "modular monolith" and not "monolith"

The modular adjective is load-bearing: a contributor adding a feature in,
say, Tenancy can't `using SiteManagement.Domain.Billing` to peek at a
dues item directly. If a context split into its own microservice tomorrow,
the refactor would be a packaging change, not a rewrite. That is what
gives the modular monolith its option value over a flat monolith — *and*
what justifies *not* splitting every context out today.

## Alternatives considered

- **Everything in one solution, including payment** — kills the polyglot
  persistence requirement (PDF) and the consumer-side-contract test that
  the two-service split makes possible.
- **One microservice per bounded context** — overkill for one author,
  premature for one machine; would have us writing service meshes
  instead of features. The boundaries are right; the deployment unit
  doesn't have to mirror them.
- **Flat monolith with no module boundaries** — the bounded contexts
  would drift into each other's tables within a quarter; would forfeit
  the future-split option.

## Consequences

- **Positive:** the monolith side keeps transactional integrity (one DB →
  one `IDbContextTransaction` per command via the `TransactionBehavior`).
  Cross-context coordination is *explicit* — a tenancy assignment marks a
  property apartment occupied via a domain event, not a method call. The
  Payment microservice is an isolated showcase of "two services + HTTP +
  retry policy + consumer-side contract test."
- **Negative:** two solutions to build, two test suites to keep green
  (`SiteManagement.slnx` + `payment-service/PaymentService.slnx`). Plain
  `dotnet test` with no args fails on the multi-solution layout —
  every command in CLAUDE.md threads the `.slnx` explicitly. A genuinely
  cross-context bug requires reading two contexts at once; the price of
  the boundary.
- **Guardrail:** PaymentService has its own NetArchTest suite asserting
  that its Domain has no Mongo references and Application has no ASP.NET
  references (kept symmetrical with the main API's layering tests).
  Cross-context import inside the main API is caught by `ArchitectureTests`.
