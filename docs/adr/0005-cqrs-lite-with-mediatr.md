# 0005 — CQRS-lite via MediatR (no event sourcing, no separate read DB)

- **Status:** Accepted
- **Date:** 2026-03-01

## Context

Within the Application layer (ADR 0001), every request belongs to one of two
shapes: a **command** that mutates state through an aggregate root, or a
**query** that projects state for the UI. The cross-cutting concerns
(transaction boundary, FluentValidation, exception translation, current-user
gate, audit metadata, IDOR ownership) want to attach to one of those shapes
uniformly — repeating them per controller / per handler is the failure mode
of every codebase that doesn't make the distinction explicit.

"Full CQRS" with event sourcing and a denormalised read store is overkill
for an apartment-site app — the writes are low-throughput, the reads are
not far behind, and the consistency model (Postgres ACID) is already what
the UI expects.

## Decision

**CQRS-lite over MediatR**, both reading and writing the same Postgres
database, no event sourcing:

- **Commands** implement an `ICommand` / `ICommand<T>` marker, are sent
  through `ISender`, have a FluentValidation validator (enforced by a
  build-time architecture test), and pass through a fixed pipeline of
  open behaviors: **Validation → Authorization → ResourceOwnership →
  ExceptionTranslation → Transaction → handler**.
- **Queries** implement an `IQuery<T>` marker, are also routed through
  MediatR for the role-marker check, but go to per-context query services
  (`ISiteQueries`, `IBillingQueries`, …) that project **straight into
  DTOs with `AsNoTracking`** — never the domain entities. The query side
  cannot accidentally mutate.
- One database, two object models: aggregate roots on the write side,
  DTOs on the read side, both reading the same EF Core mappings.

## Alternatives considered

- **Full CQRS with event sourcing + a denormalised read store** — gives
  audit-as-a-side-effect and free read scaling, but doubles the moving
  parts and the operational story. Not in the PDF brief; not justified
  by the workload.
- **No request abstraction — controller-talks-to-repository** — what most
  ASP.NET starter samples do. Cross-cutting concerns end up duplicated
  per controller (try/catch, IUnitOfWork.SaveChanges, "is the caller an
  admin?"). The whole pipeline benefit goes away.
- **Roll-our-own dispatcher** — re-implements MediatR poorly. Not worth
  the cognitive cost.

## Consequences

- **Positive:** every command goes through the same five behaviors in the
  same order; a new command author can't forget to wrap a save in a
  transaction or to ship a validator (the architecture test catches the
  validator gap; the pipeline catches the missing transaction). Read
  paths stay obviously read-only — a code reviewer sees `AsNoTracking()`
  + DTO projection and knows there's no write coming.
- **Negative:** MediatR is one more package to track (and Jimmy Bogard's
  v12+ licence change is a known watch-item; the project pins MediatR
  on an open-source-friendly version and tracks the situation). A
  contributor used to MVC controllers needs ten minutes to find their
  way around the handler + behavior layout.
- **Guardrail:** the `CqrsConventionsTests` architecture suite asserts
  every command has a validator class in the same folder and every
  request implements one (and only one) authorisation marker
  (`IAdminRequest` / `IResidentRequest` / `IPublicRequest`); the
  build fails if a contributor introduces a request without one. See
  the upcoming ADR 0006 on the authorisation pipeline.
