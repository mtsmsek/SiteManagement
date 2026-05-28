# 0001 — DDD with Clean Architecture layering

- **Status:** Accepted
- **Date:** 2026-03-01

## Context

The project is a portfolio showcase as much as a working system: hiring
committees will read the code with "is the author senior?" in mind, and the
domain (apartment-site administration with billing, messaging, payment, audit)
is rich enough to lean on. The brief from the school's PDF describes a single
DB-and-UI CRUD app; that's not enough vitrin. We have time for one round of
real layering before features pile up.

A second forcing function: the system must support two distinct UI surfaces
(admin + resident) plus a planned payment microservice. A flat layering would
let infrastructure concerns leak into shared models that both UIs read.

## Decision

Adopt **Domain-Driven Design vocabulary on top of Clean Architecture
layering**: a Domain project that depends on nothing, an Application project
that depends only on Domain, an Infrastructure project that wires the outside
world to those abstractions, and an Api project that hosts HTTP. Dependencies
point inward only. Each bounded context (Property, Residency, Tenancy,
Billing, Messaging, Identity) is a folder inside Domain/Application,
not its own project — that's the "modular" of the [0003 modular monolith](0003-modular-monolith-and-payment-microservice.md).

## Alternatives considered

- **Hexagonal / ports-and-adapters as the top-level metaphor** — almost the
  same thing in different vocabulary, but loses the explicit Application
  layer that the CQRS-lite story (ADR 0005) needs.
- **Vertical slice (feature folders, no shared layering)** — gives speed but
  forgoes the architectural payoff: this is a vitrin project; the layering
  *is* the message.
- **3-tier (presentation / business / data)** — what the school's PDF
  implicitly suggests. Anaemic in practice (every business rule ends up in
  some `*Service`). Killed by ADR 0002.

## Consequences

- **Positive:** the Application layer is the testable shape (handlers
  resolved from an `IServiceProvider`, repositories mockable with NSubstitute).
  Architecture tests (`NetArchTest`) can enforce the dependency direction in
  CI so a contributor can't accidentally `using SiteManagement.Infrastructure`
  inside Domain.
- **Negative:** more projects to navigate, more ceremony per feature (handler
  + validator + DTO + endpoint).
- **Guardrail:** `ArchitectureTests` asserts layer dependencies; a Domain or
  Application reference that points outward fails the build.
