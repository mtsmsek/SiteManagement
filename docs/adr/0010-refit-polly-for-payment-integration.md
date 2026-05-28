# 0010 — Refit + Polly (HttpResilience) for the payment integration

- **Status:** Accepted
- **Date:** 2026-04-12

## Context

The main API calls PaymentService synchronously over HTTP for every
pay-by-card request (ADR 0003). That call has three properties any
integration with an external service must respect:

1. **Type-safe.** The wire DTOs (`ProcessPaymentApiRequest`,
   `ProcessPaymentApiResponse`) have to round-trip the same way both sides
   serialise them — a manual `HttpClient.PostAsync` + `JsonSerializer.Deserialize`
   is the place per-field bugs hide.
2. **Resilient.** The downstream service can be momentarily slow (cold
   start, GC pause); a single transient failure shouldn't fail a charge
   if a 200-ms retry would have succeeded. But it must also be **bounded**:
   waiting forever on a charge is worse than declining.
3. **Idempotent.** A retry that lands while the first call also lands must
   not double-charge. PaymentService enforces this with a unique index on
   the idempotency key; the gateway adapter must pass one.

## Decision

The integration is built from three layers:

- **`IPaymentGateway`** (Application). The port the handler talks to:
  `ChargeAsync(idempotencyKey, amount, card)`. Knows nothing about HTTP.
- **`PaymentGatewayAdapter`** (Infrastructure). Anti-corruption layer.
  Translates the Application's `CardDetails` value into PaymentService's
  wire DTO and back. Wraps decline-responses in
  `PaymentRejectedException` so the handler doesn't reach into the
  transport.
- **`IPaymentServiceApi`** (Infrastructure, Refit interface). Refit
  generates the HTTP client from the interface declaration, so the wire
  contract is one source of truth that the compiler enforces.

Resilience is layered on with **`AddStandardResilienceHandler`** from
`Microsoft.Extensions.Http.Resilience` (the official Polly successor):
- retry with exponential backoff on transient HTTP failures (5xx,
  network errors, 408),
- circuit breaker after a sustained failure rate,
- per-attempt and total-call timeouts.
- The retry passes the same idempotency key every time; PaymentService's
  unique index turns the second physical attempt into a 200 + the original
  result.

Declines (insufficient funds, card rejected) come back as **HTTP 200 with
a Failed status** — they are valid business outcomes, not transport errors.
The adapter inspects the status field and throws `PaymentRejectedException`
(handled by `GlobalExceptionMiddleware` as 402). Polly only retries
*transport* failures, not business outcomes.

## Alternatives considered

- **Raw `HttpClient` + `JsonSerializer`** — works, but every wire DTO
  drift is a runtime bug to find. Refit shifts that to compile time.
- **gRPC** — more efficient, but adds .proto build steps and a binary
  format the operator can't curl. Vitrin value lower; cost higher.
- **A queue (RabbitMQ) instead of HTTP** — would push the pay flow into
  async-with-callback shape; resident UI loses the immediate Paid /
  Declined feedback. The Outbox (ADR 0007) is right for "send mail
  on closure"; HTTP is right for "charge this card now."

## Consequences

- **Positive:** the gateway is a single port the rest of the system talks
  to; PaymentService can be swapped (real bank, sandbox, in-memory stub)
  by changing the adapter only. The two-layer E2E test split falls out
  for free — PaymentService is tested over real Mongo + real HTTP; the
  main API stubs the gateway with WireMock, so it's a consumer-side
  contract test, not a binding on PaymentService source code.
- **Negative:** two interface declarations (the port + the Refit client)
  for one integration is more ceremony than a single `HttpClient` call.
  The Polly standard handler is opinionated; non-defaults are configured
  through the resilience pipeline DSL, which is a small DSL to learn.
- **Guardrail:** the `PayByCardFlowTests` E2E suite asserts the happy
  path (200 → Paid) **and** the decline path (402 → Unpaid) **and** the
  idempotency-key replay (charging the same item twice doesn't
  double-bill, because the same key returns the original result). All
  three are necessary properties of "we integrated with a payment
  service correctly."
