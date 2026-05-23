# CLAUDE.md — SiteManagement

> Auto-loaded by Claude Code. This is the source of truth for conventions +
> current status. **Memory (`~/.claude`) does NOT travel between machines — this
> file does.** Read it fully before working; it exists so a fresh session on any
> machine doesn't break the patterns already in place.

## Project
Apartment-site management system. **Portfolio/showcase** project — code quality,
DDD, and senior-level patterns matter as much as features. Author is a
Turkish-speaking senior-minded developer.

## Tech stack (verified)
- **Backend:** .NET 10, ASP.NET Core **controllers** (not Carter) + MediatR.
- **DB:** PostgreSQL 16 (EF Core, code-first, migrations). MongoDB 7 is planned
  for an isolated PaymentService in W4 (not used yet).
- **API docs:** Scalar at `/scalar/v1` + native OpenAPI at `/openapi/v1.json`.
- **Dev mail:** MailHog (UI `:8025`).
- **Frontend:** Angular 21 (standalone components, signals, signal-based stores,
  Angular Material, ngx-translate), Vitest.
- Redis / RabbitMQ / SignalR / Hangfire are roadmap, **not in compose yet**.

## Architecture & layering
Clean layered: `Domain` → `Application` → `Infrastructure` / `Api`.
- **CQRS-lite:** writes via repositories (`IXRepository`) + MediatR command
  handlers; reads via `IXQueries` projection services (DTOs, `AsNoTracking`,
  never return domain entities).
- Commands implement the `ICommand` / `ICommand<T>` marker; **every command needs
  a FluentValidation validator** (enforced by an architecture test). Requests are
  named `<Thing>Command` / `<Thing>Query` (enforced).
- `TransactionBehavior` wraps every command in a DB transaction — handlers do NOT
  open scopes themselves.
- **Errors: typed exception hierarchy with MessageKey, NO `Result<T>`.** Domain
  throws `DomainException`s; the pipeline localizes them (tr/en) to ProblemDetails.

## Hard conventions (MUST follow)
1. **TDD: tests come before implementation** in every change. Keep all suites green.
2. **Tests:** xUnit + FluentAssertions + NSubstitute. AAA comments (`// arrange`
   `// act` `// assert`). **No custom assert messages.** Shared test data in `Doubles/`.
3. **English XML `<summary>` on all public types/members**; professional inline comments.
4. **No magic literals** — repeated FluentValidation rules go in
   `CommonValidationRules`; limits in `*Limits` classes.
5. **Git: never add a `Co-Authored-By: Claude` trailer.** Commit per logical unit;
   work on `main` (author's workflow). Surface-validation (length/required) lives
   in validators; deep validity (TcNo checksum, etc.) lives on value objects.

## Implemented patterns — DON'T break these
- **Domain events vs Integration events (Outbox).** `IDomainEvent` → dispatched
  **in-transaction** (atomic cross-aggregate state, e.g. assignment → apartment
  occupied). `IIntegrationEvent` → persisted to the **OutboxMessage** table in the
  same transaction, delivered **after commit** by `OutboxBackgroundService` /
  `OutboxProcessor` (emails, future SignalR/RabbitMQ). `DuesPeriodClosed` /
  `UtilityBillPeriodClosed` are integration events. **Guardrail test:** every
  `IIntegrationEvent` must have a handler. The resident welcome email is sent
  **inline in-transaction on purpose** (system is the only holder of the password).
- **Soft delete = aggregate-root only.** Only `Site` is `ISoftDeletable` (carries
  a global EF query filter). `DELETE /api/sites/{id}` = soft archive
  (`Site.Archive`), `DELETE .../permanent` = hard `PurgeSiteCommand`,
  `POST .../restore` = `RestoreSiteCommand`. Inner entities (Block/Apartment) are
  reached via the root, so they need no own flag. **Guardrail test:** every
  `ISoftDeletable` model type must declare a query filter. Hard delete uses
  `FindIncludingArchivedAsync` (bypasses the filter). Soft-delete is an explicit
  domain operation — never auto-convert `Remove()` into a flag flip.
- **Audit.** `AggregateRoot<TId>` implements `IAuditableEntity`
  (Created/Modified At + By); `AuditSaveChangesInterceptor` stamps it from
  `ICurrentUser` (null for system/background writes) + `TimeProvider`. New roots
  are audited automatically.
- **Adding a child to an already-loaded aggregate:** call
  `IUnitOfWork.MarkAsAdded(child)` — EF otherwise tracks the new inner entity as
  `Modified` and emits an UPDATE against a non-existent row. (See AddBlock /
  AddApartment / DistributeDues handlers.)

## Dev workflow
```powershell
Copy-Item .env.example .env          # .env is gitignored — recreate per machine
docker compose up -d --build         # api :8080, postgres :5432, mongo :27017, mailhog :8025
cd web; npm install; npm start       # Angular dev server :4200
```
- Bootstrap admin (from .env): `admin@sitemanagement.local` / `Str0ng-P@ss-Dev`.
  No public registration; the first admin is seeded on startup.
- **Migrations auto-apply on startup** (`DatabaseInitializer.MigrateAndSeedAsync`).
  Add one: `dotnet ef migrations add <Name> -p src/SiteManagement.Infrastructure -s src/SiteManagement.Api`.
- **`npm run gen:api`** (in `web/`) regenerates Angular types from the running
  API's OpenAPI — the **API must be up on :8080** first. After backend endpoint
  changes: rebuild the api container, then gen:api.
- Frontend i18n: `web/public/i18n/{tr,en}.json` — add keys to **both**.

## Testing & gotchas
- Suites: `SiteManagement.Domain.Tests`, `.Application.Tests` (handler units),
  `.ArchitectureTests` (conventions/guardrails), `.E2E.Tests` (full HTTP flows).
- **E2E uses Testcontainers → Docker MUST be running** or 9+ E2E tests fail with
  "Docker is either not running or misconfigured" (NOT a code bug).
- Docker Desktop / the `npm start` dev server occasionally stop on this setup —
  if `:4200` or `:8080` is down, restart them; it's environmental.

## Current status (updated end of last session)
**Done:** W1–W3 core (Property, Residency, Tenancy, Billing dues+utility);
Day-6 Angular tenancy + billing admin pages; item payment recording;
modernized sites/billing UI + login language switcher; **transactional outbox**;
**soft delete + restore + hard purge** (Site); **audit metadata** (all roots);
**full Application command-handler unit tests**. ~283 tests green.

**Pending / next:**
- **W3: only Day 7 remains** (`WEEK-3-DETAIL.md` — Days 1–6 done). Day 7 =
  E2E failure-paths (closed-period distribute → 409, empty distribution,
  double active assignment → 409), a manual end-to-end pass, the **self-review**,
  and marking `WEEK-3-DETAIL.md` / `ROADMAP.md` W3 ✅. A `BillingFlowTests` E2E
  already exists; a dedicated `TenancyFlowTests` + the failure-path cases do not.
- **Day-7 self-review (Level-2):** add an `IQuery` marker + an architecture test
  asserting "every MediatR request is `ICommand` or `IQuery`" (only the Command
  side is fully marked today).
- **IDOR:** not yet relevant — all endpoints are `[Authorize(Roles = Admin)]`.
  Becomes important when a **resident-facing portal** is added (a resident must
  not read another resident's bills via `/residents/{id}/bills`). Design for it then.
- Then **W4** (MongoDB PaymentService) per `ROADMAP.md`.
- Optional UI polish (flow hints on the billing "Distribute/Close" actions).

## Collaboration style (author preferences)
- **Respond in Turkish.**
- For **foundational/architecture decisions, discuss in prose and decide together
  before building** — the author rejects "magic"/implicit behavior a future
  maintainer would trip over; pair any convention with a **guardrail test** so
  misuse fails loudly in CI. Settle the backbone before piling features on.
