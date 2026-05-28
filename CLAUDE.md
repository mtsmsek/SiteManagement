# CLAUDE.md — SiteManagement

> Auto-loaded by Claude Code. This is the source of truth for conventions +
> current status. **Memory (`~/.claude`) does NOT travel between machines — this
> file does.** Read it fully before working; it exists so a fresh session on any
> machine doesn't break the patterns already in place.
>
> **Authority:** if this file disagrees with anything in your local `~/.claude`
> memory (e.g. older notes claiming .NET 8 / SQL Server / Carter), **this file
> wins** — the stack/patterns below are current; update or ignore stale memory.
> First time on a machine? Provision tools via `docs/SETUP-MACHINE.md` first.

## Project
Apartment-site management system. **Portfolio/showcase** project — code quality,
DDD, and senior-level patterns matter as much as features. Author is a
Turkish-speaking senior-minded developer.

## Tech stack (verified)
- **Backend:** .NET 10, ASP.NET Core **controllers** (not Carter) + MediatR.
- **DB:** PostgreSQL 16 (EF Core, code-first, migrations) for the main API.
  **MongoDB 7** backs the **PaymentService** (live since W4 — see below).
- **PaymentService:** a **separate .NET solution** under `payment-service/`
  (`PaymentService.slnx`) — an isolated payment-gateway microservice on Mongo.
  Container `payment-api` on `:8090`. The main API calls it over HTTP via
  **Refit + Polly** (`AddStandardResilienceHandler`). See `WEEK-4-DETAIL.md`.
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
- **Authorization is a pipeline concern, never in handlers (W5).** Every MediatR
  request implements **exactly one** role marker — `IAdminRequest` /
  `IResidentRequest` / `IPublicRequest` — and `AuthorizationBehavior` enforces it
  centrally. **Guardrail:** `AuthorizationConventionsTests` fails the build if a
  request declares zero or many. Resource **ownership** (IDOR) is also a behavior,
  not a handler `if`: a request implements `IOwnedBillItemRequest` /
  `IOwnedConversationRequest` (exposes the resource id) and the matching ownership
  behavior verifies it belongs to `ICurrentUser.ResidentId`. Resident endpoints are
  token-scoped (`/api/me/*`, id never in the route). When adding an endpoint: pick a
  marker (the arch test forces it); never re-check roles/ownership inside a handler.
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
- **Payment integration (W4).** Main API → PaymentService is **synchronous HTTP**
  (Refit + Polly), behind a port: `IPaymentGateway` (Application) ↔
  `PaymentGatewayAdapter` + `IPaymentServiceApi` Refit client (Infrastructure —
  outbound HTTP belongs there, like DB/mail). The gateway is **generic**: it knows
  only card + amount + opaque reference, nothing about dues/site. `PayDuesItem` /
  `PayUtilityItem` charge first (idempotency key `dues-item:{id}` / `utility-item:{id}`),
  mark `Paid` only on success; a decline throws `PaymentRejectedException` → **402**
  (item left Unpaid). PaymentService itself: fake bank (BankAccount + CreditCard),
  Luhn/expiry/balance checks, Mongo unique index on idempotency key. Declines come
  back as **200 + Failed status** (business outcome, not transport error).
- **Credit balance (overpayment, post-W4).** No cash refund: correcting a period's
  amount down (`PUT /api/dues/{id}` / `PUT /api/utility-bills/{id}`) re-rates the
  items and credits any over-paid resident via the `ResidentCreditAccount` aggregate
  (Billing context, references resident by id). Credit is **auto-consumed** on the
  next distribution **only when it fully covers** the new item (no partial
  settlement — see open item below). `SiteDebtSummaryDto.TotalCredit` surfaces it.
  Shared `IResidentCreditService` does the find-or-create + consume. Details +
  decisions in `~/.claude` memory `project-credit-balance` (machine-local).

## Dev workflow
```powershell
Copy-Item .env.example .env          # .env is gitignored — recreate per machine (REQUIRED)
docker compose up -d --build         # api :8080, payment-api :8090, postgres :5432,
                                     #   mongo :27017, mailhog :8025
cd web; npm install; npm start       # Angular dev server :4200
```
**Fresh machine?** `git pull`/clone is NOT enough on its own — you also must
`Copy-Item .env.example .env` (gitignored; dev values inside are ready to use).
`node_modules` / `bin` / `obj` are gitignored too → recreated by `npm install` +
build. `~/.claude` memory does NOT travel — this file + the `*-DETAIL.md` docs
are the source of truth. Docker volumes (DB data) don't travel either, but that's
fine: migrations + bootstrap-admin seed run automatically on `docker compose up`.
- Bootstrap admin (from .env): `admin@sitemanagement.local` / `Str0ng-P@ss-Dev`.
  No public registration; the first admin is seeded on startup.
- **Migrations auto-apply on startup** (`DatabaseInitializer.MigrateAndSeedAsync`).
  Add one: `dotnet ef migrations add <Name> -p src/SiteManagement.Infrastructure -s src/SiteManagement.Api`.
- **`npm run gen:api`** (in `web/`) regenerates Angular types from the running
  API's OpenAPI — the **API must be up on :8080** first. After backend endpoint
  changes: rebuild the api container, then gen:api.
- Frontend i18n: `web/public/i18n/{tr,en}.json` — add keys to **both**.

## Testing & gotchas
- **Two solutions:** main `SiteManagement.slnx` and `payment-service/PaymentService.slnx`.
  `dotnet build`/`test` with no args fails (two solutions found) — always pass the
  `.slnx` and use `-m:1` (e.g. `dotnet test SiteManagement.slnx --nologo -m:1`).
- Main suites: `SiteManagement.Domain.Tests`, `.Application.Tests` (handler units),
  `.ArchitectureTests` (conventions/guardrails), `.E2E.Tests` (full HTTP flows).
  PaymentService suites: `PaymentService.Domain.Tests`, `.ArchitectureTests`
  (NetArchTest layer guardrails — Domain BCL-only/no-Mongo etc.), `.E2E.Tests`.
- **E2E uses Testcontainers → Docker MUST be running** or E2E tests fail with
  "Docker is either not running or misconfigured" (NOT a code bug).
- **Two-service E2E:** PaymentService.E2E runs the real gateway over Mongo
  (Testcontainer) + HTTP. The main API's pay-by-card E2E stubs the gateway with
  **WireMock.Net** over real HTTP (consumer-side contract test, no compile-time
  dep on PaymentService) — `CustomWebApplicationFactory` takes an optional payment
  base URL + API key; only that suite wires the gateway.
- **E2E is isolated from the compose DB** (fixed W5 Day 1). The connection string
  is resolved lazily inside the `AddDbContext` options lambda
  (`PersistenceExtensions`), so the E2E factory's in-memory Testcontainer override
  wins. Guardrail: `ConnectionStringIsolationTests` asserts the resolved
  `AppDbContext` uses the fixture container, not ambient config. (Before: the
  string was read eagerly at registration → E2E ran against the local compose
  Postgres and truncated its data + bootstrap admin.)
- Docker Desktop / the `npm start` dev server occasionally stop on this setup —
  if `:4200` or `:8080` is down, restart them; it's environmental.

## Current status (updated 2026-05-28)
**Done:** **W1–W6 complete — `v1.0.0` shipped.**

**W6 — Polish & Ship (closed out, Day 7 done):**
- **Admin messaging UI** (W5 borcu): `features/messaging/` — `MessagingApi` (admin
  `/api/conversations*`), `AdminMessagingStore` (signal-based, `MyMessagesStore`
  aynası), `AdminMessaging` component (iki panel inbox/thread + unread badge
  admin tarafı), `StartConversationDialog` (resident `MatSelect` picker + subject
  + body). Backend: `ConversationListItemDto`'ya `ResidentName` join + admin
  inbox satırlarının ismi DTO'dan alır.
- **SignalR real-time messaging**: `MessagingHub` (`[Authorize]`, OnConnectedAsync
  → role-based group join: `messaging:admins` ya da `messaging:resident:{N}`
  resident_id claim'inden), `MessagingHubNotifier` (`IHubContext`). JWT bearer
  WS query-string handshake (`PostConfigure<JwtBearerOptions>` + `?access_token=`).
  Application port: `IMessagingNotifier` + 3 payload (`MessageReceived`,
  `ConversationStarted`, `MessageRead`); 6 handler tek satır push. **Push-only**
  (sadece server→client); gönderme HTTP'den. **Tek instance** — Redis backplane
  YOK ("future"). FE: `@microsoft/signalr` + `MessagingHubService` (auth effect
  ile auto connect/disconnect, `accessTokenFactory`, withAutomaticReconnect);
  stores `merge(...)` ile her event'te `refreshOnPush`.
- **Coverage harness**: `coverlet.runsettings` + `scripts/coverage.ps1`
  (Coverlet + ReportGenerator HTML local one-shot). CI workflow: `dotnet test
  --settings coverlet.runsettings` + reportgenerator step + summary →
  `$GITHUB_STEP_SUMMARY` + `coverage-html` artifact. **Line 90.2% / Branch
  82.3% / Method 84.9%** — roadmap bantlarının üzerinde, threshold yok.
- **DemoSeeder** (`Infrastructure/Persistence/Seed/DemoSeeder.cs`, idempotent):
  domain factory + repo + `IUnitOfWork.SaveChangesAsync` (MediatR pipeline
  bypass — startup'ta `ICurrentUser` yok; `EfUnitOfWork`'ün event dispatch
  loop'u assignment'tan apartment'i Occupied yapar). 1 site + 3 daire + 3
  sakin (welcome mail outbox üzerinden) + 2026-05 dues period (1 paid + 2
  unpaid) + 1 admin-açık conversation. `Demo:SeedOnStartup=true` flag ile
  açılır; `.env.example` + `docker-compose.yml` propagate.
- **PaymentService downstream health probe** (`PaymentServiceHealthCheck`):
  typed HttpClient + 2 sn timeout; failure `Unhealthy` (orchestrator outage'i
  charge fail beklemeden görür). Postgres'in yanına eklendi.
- **UI polish:** ortak `<app-empty-state>` component (`shared/empty-state/`) —
  5 sayfaya uygulandı (site-list, resident-list, my-bills, my-messages,
  admin-messaging; 7 boş-veri noktası). Mobile responsive: admin & resident
  layout'ta CDK `BreakpointObserver` → `sidenavMode "over"` + hamburger butonu
  (`aria-label="common.menu"`). A11y sweep: tüm `matIconButton` zaten
  `aria-label`'lı, gap yok.
- **i18n parity guardrail** (`core/i18n/i18n-parity.spec.ts`): tr/en anahtar
  setlerini düzleştirip eşitler; eksik anahtar → build red. **Web 32 (+1).**
- **Dep sweep:** Angular 21.2.13→.15, CDK/Material 21.2.11→.13, .NET 10.0.5→
  .0.8, Serilog.Sinks.Console 6.0→6.1.1 (patch). **Defer (major):** jsdom 28→29,
  TS 5.9→6.0, Serilog.AspNetCore 9→10 — README "future".
- **Security headers** (`SecurityHeadersMiddleware` — Production only):
  X-Content-Type-Options nosniff, X-Frame-Options DENY, Referrer-Policy
  no-referrer + `UseHsts()`. CSP YOK (Angular ayrı origin + Scalar inline scripts).
- **Rate-limit** (.NET 10 built-in `AddRateLimiter`): `login-policy` fixed
  5/dk per-IP, `pay-by-card-policy` sliding 10/dk per-user (6 segment).
  5 endpoint'e `[EnableRateLimiting(...)]`: AuthController.Login + 2 admin
  pay-by-card + 2 resident pay-by-card. **E2E:** `LoginRateLimitTests` — 7
  ardışık bad-cred → 429 (fresh factory per test → izole limiter state).
- **Refresh token audit:** rotation ✅ (`InMemoryRefreshTokenStore.ConsumeAsync`
  TryRemove + handler-side StoreAsync), reuse-detection ✅ (consumed token →
  null → 401), FE silent refresh ✅ (`AuthService.tryRefresh`). **Defer
  ("future"):** family invalidation, EF-backed persistent store, JWT 60→15dk
  darıltma.
- **10 ADR** (`docs/adr/`): 0001 DDD/Clean, 0002 Rich Domain, 0003 Modular
  Monolith + Payment, 0004 Exception-based (no `Result<T>`), 0005 CQRS-lite,
  0006 Authz Pipeline + Markers, 0007 Outbox, 0008 Soft Delete root-only,
  0009 Token-Scoped `/api/me/*` (IDOR yapısal), 0010 Refit + Polly. MADR
  şablonu (`0000-template.md`) + index.
- **README v2**: pitch + 3 mermaid (bounded context + pay-by-card sequence
  + outbox sequence) + Demo mode bölümü + Known limitations (refresh in-memory,
  family invalidation yok, credit partial settlement yok, file attach yok,
  in-app notification yok, audit UI yok, API versioning yok, SignalR scale-out yok,
  CSP yok, mobile-first değil) + ADR linkleri.
- **Demo video script** (`docs/DEMO-VIDEO-SCRIPT.md`, TR 2-3 dk: hazırlık +
  sahne planı + anahtar mesajlar). **LinkedIn post** taslağı
  (`docs/LINKEDIN-POST.md`, 3 versiyon: kısa/orta/uzun + görsel hazırlık).
- **Deploy YOK** — bilinçli karar. Vitrin için demo-only + video; live URL
  ücret + maintenance overhead değer/maliyet oranını bozuyor. Kod tarafında
  deploy helper'lar (`PortBindingExtensions`, `DatabaseUrlExtensions`) yerinde,
  Railway/Render/Fly.io ile uyumlu — sonradan istenirse aktive edilebilir.

**Tests (W6 final, 2026-05-28):** Domain 222, Application 89, Architecture 18,
E2E 37; PaymentService Domain 46, Architecture 4, E2E 4; web (Vitest) 32.



**W5 — Resident portal + Messaging + Dashboards (closed out, Day 7 done):**
- **Authorization pipeline (new backbone).** Every MediatR request declares
  **exactly one** role marker — `IAdminRequest` / `IResidentRequest` /
  `IPublicRequest` (in `Application.Abstractions.Messaging`) — enforced centrally
  by `AuthorizationBehavior` and by an architecture test
  (`AuthorizationConventionsTests`): forgetting authz is a build error. **Handlers
  carry zero authz code.** Resource **ownership** (IDOR) is two more pipeline
  behaviors — `ResidentBillOwnershipBehavior` (`IOwnedBillItemRequest`) and
  `ConversationOwnershipBehavior` (`IOwnedConversationRequest`) — that check the
  item/conversation belongs to the caller. Role in the pipeline, ownership in the
  pipeline, work in the handler. Controllers keep `[Authorize(Roles=…)]` as
  defense-in-depth. `UnauthorizedActionException` carries a localized `MessageKey`.
- **Resident portal.** Token-scoped `/api/me/*` (id never in the route): `me/bills`,
  pay own dues/utility item by card (`PayMyDuesItem`/`PayMyUtilityItem`, split from
  the admin commands), `me/conversations*`, `me/dashboard`. IDOR proven both ways
  by E2E (`ResidentPortalFlowTests`, `MessagingFlowTests`). Shared charge logic in
  `IBillItemPaymentService`; card rules in `CommonValidationRules`.
- **Messaging** (new bounded context, in the main API). `Conversation` aggregate
  (TDD) with inner `Message`s, per-side unread, `MarkRead` touches only the other
  side. Admin `/api/conversations` + resident `/api/me/conversations`. EF owned
  collection + `AddMessagingConversations` migration. **Polling, no SignalR.**
- **Dashboards (read-side).** Resident (`GetMyDashboardQuery` composes Billing +
  Messaging reads) + admin (`IReportQueries` system-wide totals + collection rate).
  Angular `/resident/*` (residentGuard, role-based login redirect), resident +
  admin dashboards as landing pages, my-bills (reuses card dialog), messaging UI.
- **Hygiene:** E2E↔compose DB isolation fixed; PaymentService architecture tests.

**W1–W3** (Property, Residency, Tenancy, Billing
dues+utility; Angular admin pages; outbox; soft delete/restore/purge on Site;
audit metadata; `IQuery<T>` marker + guardrails). **W4 closed out (Day 7 done):**
the MongoDB **PaymentService** (separate solution, fake bank, idempotency),
**Refit + Polly** integration behind `IPaymentGateway`, **pay-by-card** for dues +
utility items (402 on decline, item stays Unpaid), Angular **card payment dialog**,
and the **two-layer E2E** (PaymentService over real Mongo+HTTP; main API pay-by-card
via a WireMock stub). Plus, after W4: the **credit-balance / overpayment** feature
(correct a period's amount → resident credit → auto-applied to the next bill when it
fully covers; `PUT` endpoints + `TotalCredit` in the debt summary), and a
**prominent animated error snackbar** (icon + error colour painted on the MDC
surface directly + scale-in/glow, 8s — the `--mdc-snackbar-container-color` token
no longer drives the surface; custom `ErrorSnackbar` via `openFromComponent`).
**W4 self-review verdict:** code architecturally sound, no real defects; see
`WEEK-4-DETAIL.md` Day 7 for the boundary/402/idempotency findings.
**Tests green (all suites, 2026-05-28):** main Domain 222, Application 83,
Architecture 18, E2E 34; PaymentService Domain 46, Architecture 4, E2E 4;
web (Vitest) 25.

**Project complete (`v1.0.0` shipped). Known limitations / future work** (all
documented in the README "Known limitations" section):

- **Refresh token in-memory + no family invalidation** — `InMemoryRefreshTokenStore`
  loses tokens on restart; reuse-detection 401s but doesn't "blow up the family."
  Production move: EF-backed store + family column.
- **JWT lifetime 60 min** — silent refresh works; 15-min tightening was rejected
  (inflates refresh traffic + hurts dev UX for no measurable defence gain).
- **Credit partial settlement** — overpayment credit auto-applies only when it
  fully covers the next item; partial application (apply 300, owe 100) needs a
  `creditApplied` state on the item (domain + migration + UI). Bilinçli defer.
- **Mesajda dosya ekleme yok** — storage strategy (S3/disk/blob) ayrı karar.
- **In-app notification hub yok** — Outbox-driven email var, in-app bildirim çanı
  + history yok.
- **Audit log UI yok** — `AuditableEntity` veri tutuyor, admin sayfası eksik.
- **API versiyonlama yok** — `/api/` versiyonsuz.
- **SignalR tek instance** — Redis backplane yok; horizontal scale için backplane
  şart.
- **CSP header yok** — Angular ayrı origin + Scalar inline scripts (Dev). Prod
  host gerçekten bundle'ı sunduğunda yazılır.
- **No live deployment** — bilinçli karar (vitrin için demo + video).
- Optional UI polish (flow hints on the billing "Distribute/Close" actions).

## Collaboration style (author preferences)
- **Respond in Turkish.**
- For **foundational/architecture decisions, discuss in prose and decide together
  before building** — the author rejects "magic"/implicit behavior a future
  maintainer would trip over; pair any convention with a **guardrail test** so
  misuse fails loudly in CI. Settle the backbone before piling features on.
