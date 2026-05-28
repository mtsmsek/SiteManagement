# 0002 — Rich domain model (anti-anemic)

- **Status:** Accepted
- **Date:** 2026-03-01

## Context

Once we've committed to DDD (ADR 0001), there's still a daily choice: do
aggregates *own* their behaviour, or do they expose state for some `*Service`
to mutate? Anemic models are the gravitational pull of every .NET codebase —
public setters, `_repo.Update(entity)` after the controller poked it, business
rules quietly migrating into helpers no one can find. The PDF for the project
won't care which we choose; the reader of the code will.

## Decision

**Behaviour lives on the aggregate root.** Setters are `private` or
`protected`; state changes go through methods named in the ubiquitous
language (`apartment.MarkAsOccupied(resident)`, `duesPeriod.Close()`,
`conversation.MarkRead(...)`); invariants are enforced at the
factory/method boundary and never trusted from the caller. Value objects
are immutable, equality-by-value. Domain services exist **only** when the
logic spans more than one aggregate; single-aggregate logic belongs to
that aggregate.

## Alternatives considered

- **Anemic model + transaction script in Application** — fast initially,
  rots into untestable `*Service` god classes. Killed on principle.
- **Anemic model + heavy use of domain events for everything** — moves the
  rot one layer up; reviewers still can't find where the invariant lives.

## Consequences

- **Positive:** aggregates are unit-testable in pure C# (no mocks). The
  consistency boundary is *visible* — a reviewer looking at `Site.AddBlock`
  sees the duplicate-name check right there. TDD on the domain feels
  natural; that's why the Domain.Tests suite is the largest one.
- **Negative:** more ctor + factory boilerplate; EF Core needs the
  private parameterless ctor convention (a "materialisation ctor" comment
  appears in every aggregate). DTOs for projections are duplicated
  effort.
- **Guardrail:** Domain test coverage stays above the roadmap floor (now
  ~95% on Domain). New aggregates without invariant tests fail the
  Domain.Tests guardrail by being obviously missing.
