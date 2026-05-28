# 0004 — Exception-based error handling (no `Result<T>`)

- **Status:** Accepted
- **Date:** 2026-03-01

## Context

Once the layering (ADR 0001) and rich domain model (ADR 0002) are in place,
every command can fail for at least three different reasons: domain
invariants (duplicate apartment number), missing prerequisites (resident
not found), and validation (empty subject). The shape of those failures
has to be uniform end-to-end: handlers can't proliferate ad-hoc tuples,
controllers can't sprinkle `if (result.Error == "X") return BadRequest()`,
and the localisation story (tr-TR + en-US, with `MessageKey`-keyed
ProblemDetails) has to plug in once, not per-handler.

Two camps in .NET land:
1. **Result\<T\> / Either** — control flow stays linear, exceptions
   reserved for "the world is on fire" scenarios.
2. **Exceptions for control flow** — the language's first-class mechanism,
   but historically ugly because of stack-trace cost and the temptation
   to catch-and-swallow.

## Decision

**Exceptions, with a typed hierarchy that carries a localisation key.**
`DomainException` is the abstract base for domain failures (each subclass
encodes one rule: `DuplicateApartmentNumberException`,
`ApartmentAlreadyOccupiedException`, ...). `ApplicationException` is the
abstract base for application-layer failures (`EntityNotFoundException`,
`UnauthorizedActionException`, `BusinessRuleViolationException`,
`PaymentRejectedException`, `ValidationException`). Every concrete
exception carries a `MessageKey` (e.g. `"tenancy.apartmentAlreadyAssigned"`).

A MediatR `ExceptionTranslationBehavior` runs at the Application boundary:
it catches `DomainException`, looks up its `MessageKey` in the
`IStringLocalizer<ErrorMessages>`, and re-throws as the matching
`ApplicationException`. The API layer has one `GlobalExceptionMiddleware`
that turns any `ApplicationException` into a RFC 7807 ProblemDetails with
the right HTTP status and the localised message in the response body.
Handlers write **no `try`/`catch`**.

## Alternatives considered

- **`Result<T>`** — clean control flow, but in .NET it adds ceremony at
  every call site (`if (!r.IsSuccess) return r.Error;`), erases the
  exception-message stack for free debugging, and makes the
  localisation story per-error-type. The senior-mindset reader expects
  exception throwing in .NET; not a pattern fight worth picking.
- **Ad-hoc DTOs with an "error" field** — what you end up with when you
  give up on either pattern. Drifts into magic strings within a sprint.
- **`Either<Error, T>` from a functional library (LanguageExt, etc.)** —
  same shape as `Result<T>` but pulls in a dependency the rest of the
  codebase wouldn't otherwise need. Increases the cognitive surface for
  a reviewer who hasn't seen LanguageExt before.

## Consequences

- **Positive:** handlers stay *tiny* — they just throw the right
  exception subclass; the pipeline takes care of translation,
  localisation, and HTTP shape. The exception type is the contract a
  test can assert on (`act.Should().ThrowAsync<DuplicateApartmentNumberException>()`).
- **Negative:** the "exceptions for control flow are slow" objection is
  real but immaterial here — the slow paths *are* the slow paths
  (one HTTP request = one transaction; the stack-trace cost is
  microseconds against a multi-millisecond DB round trip). Stack-trace
  capture is a debugger's friend; we accept the cost as a feature.
- **Guardrail:** an architecture test asserts every `MessageKey`
  appears in *both* `Resources/ErrorMessages.tr-TR.resx` and
  `ErrorMessages.en-US.resx` — a missing translation surfaces at build
  time, not as a runtime "missing translation" warning the operator
  sees in the browser.
