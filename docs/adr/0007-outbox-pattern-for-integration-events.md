# 0007 — Outbox pattern for integration events

- **Status:** Accepted
- **Date:** 2026-03-15

## Context

Closing a dues period needs two things to happen *together*: the row in
`DuesPeriods` flips to `IsClosed = true`, and every resident with an unpaid
item gets an email. If those run in two separate commits, the system can
mail residents about a closure that never happened (commit B succeeded
after commit A rolled back) — or the closure can land without the mails
(crash between commits).

Two camps:

1. **Send the email inside the same transaction.** Simple, but a slow or
   flaky SMTP server now stalls (or rolls back) the closure, and a 2-phase
   commit between Postgres and the mail server doesn't exist.
2. **Outbox pattern.** The handler writes the side-effect intent to an
   `OutboxMessages` row in the *same* transaction as the state change; a
   background worker delivers it after commit, with retries.

The pattern is the standard answer to "I need exactly-once-ish delivery
under at-least-once mechanics," and the code reviewer expects it.

## Decision

A two-layer event vocabulary:

- `IDomainEvent` — **in-transaction**. Used when one aggregate's change
  must produce another aggregate's change atomically (e.g.
  `ResidentAssignedToApartment` → mark the apartment occupied). Dispatched
  by `EfUnitOfWork.SaveChangesAsync` in a bounded loop until no aggregate
  has pending events.
- `IIntegrationEvent` — **after-commit, via the outbox**.
  Emitted by raising on the aggregate; the EF interceptor serialises the
  event into an `OutboxMessages` row inside the same `SaveChangesAsync`
  transaction. An `OutboxBackgroundService` polls the table, hands rows
  to an `OutboxProcessor`, which deserialises and dispatches them through
  MediatR. Successful dispatch marks the row processed; failures back off
  for a retry.

`DuesPeriodClosed`, `UtilityBillPeriodClosed`, etc. are integration
events. `ResidentAssignedToApartment` is a domain event.

The resident welcome email is **the explicit exception**: it is sent
*inline in-transaction* because the system is the only holder of the
generated password — if SMTP fails the whole RegisterResident command
must roll back, otherwise the resident exists but can't log in.

## Alternatives considered

- **No outbox; send mail in handlers** — re-introduces the dual-write
  failure mode above. Considered and rejected on principle.
- **Hand the delivery to a queue (RabbitMQ / Service Bus)** — the right
  long-term move, but the outbox row is the *source of truth* either way;
  a queue is just a delivery mechanism. The outbox stays; "delivery is
  RabbitMQ-fronted" is a swap-out, not a redesign. RabbitMQ is in the
  roadmap, not in compose today.
- **Idempotency keys at the receiver instead of an outbox** — works
  when the receiver is yours; doesn't work for SMTP. The outbox is the
  one-fits-many answer.

## Consequences

- **Positive:** `Db state + side effect` are now atomic; a crash between
  them is recoverable (the row stays unprocessed, the background worker
  picks it up). The pattern is identical for "send an email" today and
  "publish to RabbitMQ" tomorrow — only the dispatcher changes.
- **Negative:** every integration event is a tiny ceremony — define the
  event, write a handler, register the handler. Forgetting the handler
  means the row sits in the outbox forever (silent failure).
- **Guardrail:** an architecture test asserts that every concrete
  `IIntegrationEvent` has a registered MediatR notification handler;
  the build fails on a hanging event. The Outbox table itself is the
  recovery mechanism — `select * from "OutboxMessages" where
  "ProcessedAtUtc" is null` is the operator's diagnostic.
