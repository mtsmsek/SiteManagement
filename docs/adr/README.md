# Architecture Decision Records

This directory holds the architecturally significant decisions made on the
project, in the [MADR](https://adr.github.io/madr/) format. Each record is a
small, self-contained Markdown file: **Context** (why we had to choose),
**Decision** (what we picked), **Alternatives** (what we rejected and why),
and **Consequences** (what we now live with — good and bad).

ADRs are immutable once accepted. To revise a choice, write a new ADR that
**supersedes** the old one and leave the original in place — the history of
*why* matters as much as the current state.

## Index

| # | Title | Status |
|---|---|---|
| [0001](0001-ddd-clean-architecture.md) | DDD with Clean Architecture layering | Accepted |
| [0002](0002-rich-domain-model.md) | Rich domain model (anti-anemic) | Accepted |
| [0003](0003-modular-monolith-and-payment-microservice.md) | Modular monolith + one external Payment microservice | Accepted |
| [0004](0004-exception-based-error-handling.md) | Exception-based error handling (no `Result<T>`) | Accepted |
| [0005](0005-cqrs-lite-with-mediatr.md) | CQRS-lite via MediatR (no event sourcing, no read DB) | Accepted |
| [0006](0006-authorization-pipeline-and-request-markers.md) | Authorization pipeline + single-marker request types | Accepted |
| [0007](0007-outbox-pattern-for-integration-events.md) | Outbox pattern for integration events | Accepted |
| [0008](0008-soft-delete-aggregate-root-only.md) | Soft delete only on aggregate roots (Site) | Accepted |
| [0009](0009-token-scoped-resident-endpoints.md) | Token-scoped resident endpoints (`/api/me/*`) — structural IDOR fix | Accepted |
| [0010](0010-refit-polly-for-payment-integration.md) | Refit + Polly (HttpResilience) for the payment integration | Accepted |

## Template

Start a new ADR by copying [`0000-template.md`](0000-template.md).
