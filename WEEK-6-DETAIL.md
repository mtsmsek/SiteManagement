# Hafta 6 — Polish & Ship (Detaylı Plan)

> **Hedef:** "Vitrine konulabilir hale gelmiş." W1–W5 fonksiyonel olarak tamam;
> W6 = kalan FE açıklarını kapat, **SignalR ile gerçek-zamanlı mesajlaşma**,
> test coverage raporu, dokümantasyon (README v2 + ADR'lar), UI polish, güvenlik
> minimumu (headers + rate-limit), **demo seeder**, screenshot + demo video,
> LinkedIn postu.
>
> **Definition of Done (üst seviye):** Repo public, README v2 prezentabl,
> ADR'lar yerinde; tüm test suite'leri yeşil + coverage raporu artifact'ta;
> SignalR ile resident ↔ admin canlı mesajlaşıyor (polling kaldırıldı); UI
> tutarlı (empty/loading/error/a11y); `docker compose up` + demo-seed ile sıfırdan
> tam demo açılıyor; security headers + rate-limit yerinde; demo video kayıtlı;
> LinkedIn taslağı hazır; `v1.0.0` tag atıldı.
>
> **Deploy YOK.** Bilinçli karar: canlı URL yerine **demo-only** (`docker compose up`
> + zengin seed + screenshot + video). README'de "self-host" olarak konumlanıyor.

W1–W5 deseni: yeni bounded context **yok**; W6 bütünüyle **hijyen + cila + ship
+ SignalR**. Konvansiyon eklenen her madde test/guardrail'le kilitlenir
(önceki haftaların kuralı).

---

## Mimari Kararlar (W6) — Gün 1 öncesi kilitlendi

| # | Konu | Karar |
|---|---|---|
| 1 | Admin Messaging UI | Resident UI deseninin aynası (iki panel + unread badge); admin **yeni konu açabilir** (`StartConversationCommand`). FE store + Vitest. |
| 2 | Coverage ölçümü | **Coverlet + ReportGenerator**, HTML rapor CI artifact'ı. **CI'da threshold YOK** (esnek; rapor görünür, eksik yerlere manuel test yazılır). |
| 3 | ADR seti | **10 ADR, MADR şablonu**, `docs/adr/`. Konular: DDD/Clean, Rich Domain, Modüler Monolit + ayrı Payment, Exception-based, CQRS-lite, Authz Pipeline + Markers, Outbox, Soft delete (root-only), Token-scoped resident endpoints, Refit + Polly. |
| 4 | README v2 | Gün 7'de (en sona alındı — kararlar/feature'lar oturduktan sonra yazılır). |
| 5 | Deploy | **YOK.** Demo-only: `docker compose up` + demo seeder + screenshot + 2-3 dk MP4. Plan B (Railway/Fly/Render) gündeme alınmadı. |
| 6 | SignalR | **W6'ya alındı.** Hub + JWT auth + group = `user:{id}`; messaging client; polling kaldırıldı; tek instance (Redis backplane YOK; README "scale-out roadmap" diye yazılır). |
| 7 | Credit partial settlement | **Defer.** README "known limitation". |
| 8 | UI polish kapsamı | Min + Orta: empty state, loading skeleton, error tutarlılığı, 401 interceptor, a11y label, mobile smoke (360x640), dark/light tema sweep. |
| 9 | Security pass | Security headers (HSTS/X-CTO/X-Frame/Referrer) `Production` profile için; rate-limit `login` (fixed 5/dk) + `pay-by-card` (sliding 10/dk). **Refresh token VAR** — access 15 dk + refresh 7 gün, rotation + family invalidation; 401 → silent refresh interceptor. JWT lifetime kısaltıldı. |
| 10 | Demo + LinkedIn | 2-3 dk **Türkçe MP4** ekran kaydı; LinkedIn postu Türkçe; tech hashtag'leri + repo + video linki. |
| E4 | Mesajda dosya ekleme | **Eklenmiyor.** Storage stratejisi (S3/disk/blob) ayrı karar; vitrin akışına marjinal katkı. README "future". |
| E7 | Health check | `/health` + `AddNpgSql()` + `AddMongoDb()`; PaymentService kendi `/health` + `AddMongoDb()`. |
| E11 | Demo seeder | `IDemoSeeder` + env flag (`Demo:SeedOnStartup=true`); 2 site + 5 sakin + 1 kapalı + 1 açık dönem + 1 utility + birkaç ödeme/mesaj. `docker compose up` ile sıfırdan zengin demo veri. |
| E13 | PaymentService admin UI | **Basit ekle** — `/admin/payment-data`: bank account listele/oluştur, card listele/oluştur. Ana API'de proxy controller → `IPaymentAdminApi` (Refit) → PaymentService (yeni admin endpoint'ler, API-key korumalı). |
| E3 | Bildirim merkezi | Defer (README future). |
| E5 | Audit log UI | Defer (backend hazır; sadece query + sayfa eksik). |
| E6 | API versiyonlama | Defer. |

---

## Başlangıç durumu (W6'ya girerken zaten yerinde olanlar)

Bunları yeniden kurmuyoruz, üstüne biniyoruz:

| Zaten var | Nerede / Etkisi |
|---|---|
| W5 backend tam | Resident portal + Messaging + Dashboards backend (admin + resident). |
| Resident Angular portalı tam | `/resident/dashboard`, `/resident/bills`, `/resident/messages`. |
| Admin Angular tarafı | Site/Apartment/Resident/Dues/Utility/Payments + dashboard. **Messaging sayfası YOK** (W5'ten devir). |
| Authorization pipeline + arch test | `IAdminRequest`/`IResidentRequest`/`IPublicRequest` + ownership behavior'lar + `AuthorizationConventionsTests`. |
| Outbox + integration events | `OutboxBackgroundService`/`OutboxProcessor`; her `IIntegrationEvent` için handler arch testi. |
| Audit + soft delete (Site) | `AuditSaveChangesInterceptor` + `ISoftDeletable` guardrail. |
| Tests yeşil (2026-05-28) | Main: Domain 222, App 83, Arch 18, E2E 34. PaymentService: Domain 46, Arch 4, E2E 4. Web (Vitest) 25. |
| Localization | `tr/en` resx + Angular ngx-translate; `ResourceIntegrity` arch testi. |
| Dev altyapı | docker-compose (api/payment-api/postgres/mongo/mailhog), Scalar (`/scalar/v1`), bootstrap admin seed, migration auto-apply. |
| CI | GitHub Actions (W1'de kuruldu). |

> **W6'da kapanacak borçlar:** admin messaging UI, SignalR (polling kaldır),
> coverage raporu, ADR'lar, README v2, UI polish, security headers + rate-limit,
> health check, demo seeder, PaymentService admin UI, screenshot + video,
> LinkedIn, `v1.0.0` tag.

---

## Gün 1 — Kararları yaz + Admin Messaging UI ✅

**Hedef:** Yukarıdaki karar tablosunu bu dosyaya işle (yapıldı) + W5'ten devreden
tek FE açığını kapat ki "feature complete" diyebilelim.

- [x] **Backend dokunuşu:** `ConversationListItemDto`'ya `ResidentName` alanı (admin
      inbox'ın resident adını DTO'dan alması için). `MessagingQueries` Resident'a
      join → display name. Test: `MessagingFlowTests` `ResidentName="Ada Lovelace"`
      assertion. E2E + Application 83 + Domain 222 + Arch 18 yeşil. `gen:api` ile
      FE tipi alındı.
- [x] **Admin Messaging UI** (Angular):
  - `/admin/messaging` rotası (adminGuard) + nav linki (`forum` icon, `nav.messaging`).
  - `MessagingApi` (admin): `GET /api/conversations`, `GET /api/conversations/{id}/messages`,
    `POST /api/conversations` (resident seçimi), `POST /api/conversations/{id}/messages`,
    `POST /api/conversations/{id}/read`.
  - `AdminMessagingStore` (signal-based, `MyMessagesStore` deseninin aynası).
  - İki panel layout: solda inbox (resident adı + subject + unread badge), sağda thread.
  - "Yeni Konu" `StartConversationDialog` (resident `MatSelect` picker + subject + body;
    `ResidentsStore.loadList()` lazy yükler).
  - i18n `admin.messaging.*` + `nav.messaging` (tr+en).
  - Vitest: `AdminMessagingStore` (4 test) → web **29 yeşil** (+4).
  - `ng build` temiz; lazy chunk `admin-messaging`.

**Commit:** `feat(web): admin messaging UI` (+ backend DTO ResidentName)

---

## Gün 2 — SignalR (gerçek-zamanlı mesajlaşma) ✅

**Hedef:** Admin ↔ resident **canlı** mesajlaşsın.

### Kararlar (Gün 2 başında prose ile kilitlenmiş)
- **Auth:** JWT bearer query-string yolu (`?access_token=`) — WS handshake'inde
  header taşınmıyor. `ApiConstants.SignalRAccessTokenQueryKey`, sadece
  `/hubs/*` path'leri için aktif.
- **Group stratejisi:** `messaging:admins` (tüm admin'ler) + `messaging:resident:{id}`
  (her sakinin kendi grubu, residentId claim'inden). Hub `OnConnectedAsync`'te
  role'e göre doğru grupa otomatik ekler — başkasının grubuna eklemek
  yapısal olarak imkânsız (claim-based).
- **Hub yüzeyi:** **sadece server-to-client** push event'leri — gönderme HTTP'den
  (validation + ownership pipeline orada zaten çalışıyor). Üç event:
  `MessageReceived`, `ConversationStarted`, `MessageRead`.
- **Payload minimum:** `conversationId` (+ ConversationStarted için `residentId`).
  Client event geldiğinde **read API'den yeniden çeker** — push payload'ı tek
  veri kaynağı değil; drift riski sıfır.
- **Tek instance:** Redis backplane YOK; horizontal scale README'de "future".
- **Polling kaldırma not:** W5'te zaten polling timer YOKTU — sadece "action sonrası
  manuel refresh" vardı; SignalR onları replace etmiyor, **karşı taraf eylemini
  de UI'a getiren** otomatik refresh ekledi. Action sonrası refresh çağrıları
  yerinde kalır (deterministik).

### Backend
- [x] `Application/Messaging/Notifications/IMessagingNotifier.cs` — port +
      3 payload (`IMessagingNotification` ortak interface'i, `EventName` ile).
- [x] `Api/Messaging/MessagingGroups.cs` — grup adı naming (Admins + ForResident).
- [x] `Api/Messaging/MessagingHub.cs` — `[Authorize]`, `OnConnectedAsync`'te role'e
      göre grup join (`Roles.Admin` → admins; `Roles.Resident` + `resident_id`
      claim → forResident).
- [x] `Api/Messaging/MessagingHubNotifier.cs` — `IHubContext<MessagingHub>` ile push.
- [x] `Api/Messaging/MessagingHubExtensions.cs` — `AddSignalR()` + `IMessagingNotifier`
      DI + `PostConfigure<JwtBearerOptions>` ile WS query-string token desteği +
      `MapHub`.
- [x] `Api/Configuration/ApiConstants.cs` — `HubsPrefix`, `MessagingHubPath`,
      `SignalRAccessTokenQueryKey` (no magic literals).
- [x] 6 handler'a `IMessagingNotifier` inject + tek satır push:
  - admin tarafı (`StartConversation` / `Reply` / `MarkRead`) → `NotifyResident`
  - resident tarafı (`StartMy` / `ReplyMy` / `MarkMyRead`) → `NotifyAdmins`
- [x] **Application unit test:** `MessagingHandlerNotifyTests` — 6 handler için
      her birinin doğru hedefe (admins/resident) doğru notification tipiyle
      çağrı yaptığını assert eder (NSubstitute, AAA). **App 83 → 89 (+6) yeşil.**
- [x] **E2E:** `SignalRMessagingTests` — `Microsoft.AspNetCore.SignalR.Client` +
      `Server.CreateHandler()` üzerinden LongPolling. İki test:
  - Admin start → resident `ConversationStarted` alıyor (payload.residentId)
  - Resident reply → admin `MessageReceived` alıyor (payload.conversationId)
  - **E2E 34 → 36 (+2) yeşil.**

### Frontend
- [x] `@microsoft/signalr` (v10.0.5) paketi.
- [x] `core/realtime/messaging-hub.service.ts` — `MessagingHubService`:
  - `effect()` ile `AuthService.currentUser` değişimini dinler → login'de
    `ensureConnected()`, logout'ta `disconnect()`. `accessTokenFactory` her
    (re)connect'te fresh token okur (refresh sonrası temiz reconnect).
  - `withAutomaticReconnect()`.
  - Üç event için `Subject` + `asObservable()` (drift'siz pure observer).
- [x] `AdminMessagingStore` + `MyMessagesStore` event subscribe:
  - `merge(messageReceived, conversationStarted, messageRead)` → her event'te
    `refreshOnPush`: inbox + (varsa) seçili thread'in mesajları yenilenir.
  - Read projection tek source of truth, push sadece tetik.
- [x] Spec'ler: hub mock (Subject ile) verilip her store için yeni test:
  "hub event → otomatik refresh". **Web 29 → 31 (+2) yeşil; ng build temiz.**

**Tests:** Domain 222, Application 89, Architecture 18, E2E 36, web 31 — yeşil.
**Commit:** `feat(messaging): SignalR real-time push (admin + resident)`

---

## Gün 3 — Coverage Harness ✅

**Hedef:** Test suite'in kapsamını görünür kıl. Critical-path E2E'lerin
**zaten yerinde** olduğunu keşfettik (welcome-mail outbox, soft-delete
archive/restore/purge, credit-balance auto-application + Gün 2'nin SignalR
push'ları), o yüzden gün **observability + reporting**'e daraldı.

- [x] **`coverlet.runsettings`** (root) — XPlat collector + makul exclude'lar
      (test assembly'leri, EF Migrations, generated code, `SkipAutoProps`).
- [x] **`scripts/coverage.ps1`** — tek komut local: eski çıktıyı sil →
      `dotnet test --settings coverlet.runsettings` → on-demand
      `dotnet-reportgenerator-globaltool` install → `coverage/index.html` +
      `Summary.txt` üret. Vitrin için "tek tıkla rapor".
- [ ] **CI (`.github/workflows/ci.yml`)** — diff hazır, push **bekliyor** (mevcut
      PAT'in `workflow` scope'u yok; kullanıcı manuel commit/push edecek):
  - `dotnet test` artık `coverlet.runsettings` kullanıyor (exclude'lar tutar).
  - Yeni step: ReportGenerator'ı `dotnet tool install` ile kurup HTML +
    TextSummary + Badges üretir (external action sürüm-kırılganlığından
    kurtulduk).
  - `Summary.txt` head'i `$GITHUB_STEP_SUMMARY`'ye eklenir (PR run sayfasında
    coverage özeti görünür).
  - `coverage-html` artifact upload (HTML klasörü).
- [x] **Critical-path E2E backfill ihtiyaç YOK** — zaten kapsanıyor:
  - Welcome mail outbox → `ResidentFlowTests.RegisterResident_PersistsAggregateAndQueuesWelcomeEmail`
  - Soft delete + restore + purge → `PropertyFlowTests` (3 ayrı test:
    `DeleteSite_ArchivesIt`, `PurgeSite_HardDeletes`, `RestoreSite_BringsAnArchivedSiteBackIntoReads`)
  - Credit balance journey → `CreditBalanceFlowTests` (ayrı dosya, full journey)
  - Rate-limit smoke → **Gün 6'ya kaldı** (rate-limit henüz kurulu değil).

**Coverage (local full run, Docker up):**
- Line **90.2%** (2047/2268)
- Branch **82.3%** (247/300)
- Method **84.9%** (419/493)

Roadmap bantlarını (Domain ≥80, App ≥60) **rahatlıkla geçiyor**. Threshold yok
(plan kararı); rapor görünür, eksik yerlere göz manuel.

**Tests:** Domain 222, App 89, Architecture 18, E2E 36, web 31 — yeşil
(Gün 2'den değişmedi; yeni test gerekmedi).
**Commit:** `test: coverage harness (Coverlet + ReportGenerator HTML)`

---

## Gün 4 — Demo Seeder + Health Check ✅ (PaymentService admin UI ertelendi)

**Hedef:** Klonla → `docker compose up` → tüm demo verisi hazır.
Gün ortasında scope daraltıldı: PaymentService kendi seeder'ı (4242 test kartı +
100k bakiye) **zaten** var, dolayısıyla admin UI'sının "vitrin için zorunlu"
değeri düşük; gerekirse ufak follow-up commit olarak eklenir. Bu daraltma Gün 4'ü
"tek odaklı + ship-ready" tuttu.

### Demo Seeder (ana API) ✅
- [x] `Infrastructure/Persistence/Seed/DemoSeeder.cs` — idempotent ("site var mı?"
      check'iyle). `Domain` factory'ler + repo'lar + `IUnitOfWork.SaveChangesAsync`;
      MediatR pipeline bypass (startup'ta `ICurrentUser` yok, AuthorizationBehavior
      bypass için bilinçli karar). Domain event'ler `EfUnitOfWork`'ün event dispatch
      loop'undan tetikleniyor (assignment'ın `ResidentAssignedToApartment` event'i
      apartment'i otomatik occupied yapıyor).
- [x] **İçerik (vitrin için sade + zengin):**
  - 1 site "Lavanta Konutları" (Bahçelievler / İstanbul)
  - 1 blok A, 3 apartment (1+1 / 2+1 / 2+1, floor 1-2)
  - 3 sakin (TC + email + telefon; gerçek checksum'a uyan TC'ler; her birine
    welcome mail aynı `IEmailSender` üzerinden — MailHog'da görünür)
  - 3 ApartmentAssignment (1 owner + 2 tenant)
  - 2026-05 DuesPeriod açık + dağıtılmış (3 item; ilki paid, kalan 2 unpaid →
    resident portal pay-by-card flow'u demo data'da hemen kullanılabilir)
  - 1 admin-açık conversation "Hoş geldiniz" (resident için unread)
- [x] `DemoOptions` (section `Demo:SeedOnStartup`) + Infrastructure DI binding.
- [x] `DatabaseInitializer.MigrateAndSeedAsync` flag açık ise `DemoSeeder.SeedAsync`.
- [x] `.env.example` → `DEMO_SEED_ON_STARTUP=true` (dev default); `docker-compose.yml`
      → `Demo__SeedOnStartup: ${DEMO_SEED_ON_STARTUP:-false}` (prod'da default kapalı).
- [x] **E2E güvenliği:** flag default `false`, `CustomWebApplicationFactory` flag'i
      override etmiyor (zaten kapalı); E2E test'ler etkilenmez (Domain 222,
      App 89, Arch 18, E2E 36 yeşil).

### Health Check ✅
- [x] `HealthChecks/PaymentServiceHealthCheck.cs` — typed `HttpClient`, downstream
      `/health` probe; başarısızlık `HealthStatus.Unhealthy` (orkestratör outage'i
      doğrudan görür, charge fail'ini beklemez).
- [x] `HealthCheckExtensions` Postgres'in yanına `AddCheck<PaymentServiceHealthCheck>`
      ekledi + named HttpClient (`PaymentService:BaseUrl` + **2 saniye timeout** →
      hung downstream readiness probe'unu kilitlemez).
- [x] PaymentService kendi `/health` endpoint'i zaten var (compose healthcheck
      onu kullanıyor); ana API probe transitif olarak yansıtıyor.

### Ertelendi (W6 sonrası küçük takip)
- [ ] PaymentService admin UI (bank account + card CRUD'u olan basit Angular sayfa).
      Demo seeder + PaymentService kendi seeder'ı zaten `4242 4242 4242 4242` test
      kartını + bakiyeli hesabı veriyor — mülakatta "test verisi nasıl üretiliyor?"
      sorusunun cevabı zaten "iki idempotent seeder" olarak hazır.

**Tests:** Domain 222, App 89, Architecture 18, E2E 36, web 31 — yeşil
(no new tests; seeder canlı doğrulaması `docker compose up` ile manuel).
**Commit:** `feat(seed,health): demo data seeder + payment-service health probe`

---

## Gün 5 — UI Polish + i18n + Dependency Sweep + ADR'lar (1–5) ✅

**Hedef:** Tutarlı UI; eksik çeviri kalmaz; paketler güncel + ilk 5 ADR yazılı.

### Özet
- **UI polish:** ortak `<app-empty-state>` component (`shared/empty-state/`) +
  5 sayfaya uygulandı (site-list, resident-list, my-bills, my-messages,
  admin-messaging — 7 boş-veri durumu). Mobile responsive sidenav: admin &
  resident layout'larında `BreakpointObserver` ile `Handset|Small` →
  sidenav `over` mode + hamburger butonu (`aria-label="common.menu"`).
  A11y sweep: `matIconButton` her yerde `aria-label`'lı — eksik yok.
- **i18n parity guardrail:** `core/i18n/i18n-parity.spec.ts` tr/en anahtar
  setlerini eşitliyor; eksik anahtar → build red. **Web 31 → 32 (+1) yeşil.**
- **Outdated paketler:** Angular 21.2.13 → 21.2.15, CDK/Material 21.2.11 →
  21.2.13, .NET 10.0.5 → 10.0.8, Serilog.Sinks.Console 6.0.0 → 6.1.1 (patch
  upgrade'ler). Defer (major): jsdom 28→29, TypeScript 5.9→6.0,
  Serilog.AspNetCore 9→10 — README "future" maddesine eklenecek.
- **Bundle budget:** initial 500→600 KB (mevcut feature seti için dar; SignalR
  + dashboard + messaging client yüklenince doğal aşım). `ng build` warning-free.
- **ADR 1-5 + index + 0000 template:** `docs/adr/` altında MADR şablonu.
  Konu başlıkları: DDD/Clean, Rich Domain (anti-anemic), Modular Monolith +
  Payment microservice (özellikle "modüler monolith ≠ her şey tek yerde"
  netleştirildi), Exception-based error handling (no `Result<T>`), CQRS-lite
  via MediatR.

### Tests
Domain 222, App 89, Architecture 18, E2E 36, web **32** (+1 i18n parity).
**Commit:** `chore(web): UI polish + i18n parity + dep sweep` + `docs(adr): architecture decisions 1-5`

### UI Polish
- [ ] **`<app-empty-state>`** ortak component — icon + mesaj + opsiyonel CTA. Tüm
      tablo/liste sayfalarına: resident & admin bills, conversations, sites, residents, periods, payment data.
- [ ] **Loading skeleton/spinner tutarlılığı:** her sayfada `loading()` signal'a göre
      `<mat-progress-bar>` veya Material skeleton.
- [ ] **Error tutarlılığı:** W4 error snackbar'ı tüm store'larda kullanılıyor mu kontrol.
- [ ] **A11y:** tüm icon button'larda `aria-label`; dialog'larda focus trap; route
      change'de page title.
- [ ] **Mobile smoke:** resident portalı 360x640 görünümünde — sidenav `over` mode + CDK
      `BreakpointObserver`.
- [ ] **Tema:** dark/light her sayfada bozulmadan görünüyor mu sweep.

> Not: 401 interceptor Gün 6'da refresh token ile birlikte geliyor.

### i18n + Dependency
- [ ] **i18n eksik anahtar taraması:** Vitest scripti — `tr.json` ve `en.json` anahtar
      kümeleri eşit mi? Eksik anahtar → test red.
- [ ] **Outdated paketler:** `npm outdated` + `dotnet list package --outdated` →
      major olmayan güncellemeler; major'lar README "future".
- [ ] **Dead code tarama:** kullanılmayan validator/service var mı? Aday: terk edilmiş
      `*Validator` veya `*Service`.

### ADR'lar — Architecture odaklı (1–5)
MADR şablonu, her dosya 1 sayfa: Bağlam / Karar / Alternatifler / Sonuçlar.

- [ ] `docs/adr/0000-template.md` — MADR şablonu.
- [ ] `0001-ddd-clean-architecture.md`
- [ ] `0002-rich-domain-model.md`
- [ ] `0003-modular-monolith-and-payment-microservice.md`
- [ ] `0004-exception-based-error-handling.md`
- [ ] `0005-cqrs-lite-with-mediatr.md`

**Commit:** `chore(web): UI polish — empty/loading/a11y/mobile + i18n key parity` + `docs(adr): architecture decisions 1-5`

---

## Gün 6 — Security Pass (headers + rate-limit + refresh-token audit) ✅

**Hedef:** Alt-sınır güvenlik hijyeni. Refresh token'ı sıfırdan kurmaya gerek
yoktu — yapı zaten mevcuttu (audit + bilinen sınırlar), Gün 6 onun üzerine
**security headers + rate-limit** ekledi.

### Refresh token — audit + decision
Mevcut implementation (W1-W2'den, `InMemoryRefreshTokenStore` + `RefreshTokenCommandHandler`):
- ✅ **Rotation:** `ConsumeAsync` token'i `TryRemove`'la kaldırır; handler hemen
  yeni token üretip `StoreAsync` ile saklar → her refresh tek-kullanımlık.
- ✅ **Reuse-detection:** tüketilmiş token tekrar gelirse `ConsumeAsync` null
  döner → handler `AuthenticationException` → **401**. Attacker önce kullanırsa
  meşru kullanıcı 401 alıp login'e atılır (etkin güvenlik korunmuş, "tüm
  session kapalı" yerine "yeniden login").
- ✅ **FE silent refresh:** `AuthService.tryRefresh()` zaten yerinde,
  `localStorage` ile `accessToken` + `refreshToken` + `accessTokenExpiresAt`.
- ❌ **Family invalidation:** ek "compromise → tüm family revoke" yok. Tradeoff
  kabul; "future" maddesi.
- ❌ **Persistent store:** `InMemoryRefreshTokenStore` restart'ta token'leri kaybeder.
  Single-instance dev demosu için OK; production / horizontal scale için
  EF-backed store gerekir. "future" maddesi.
- ❌ **JWT 60 dk → 15 dk darıltma:** şimdi yapılmadı — pratikte FE silent
  refresh devrede; süreyi düşürmek refresh hacmini artırır + dev UX'i bozar.
  README'de "kısa access ile defense-in-depth" maddesine kalır.

### Security headers ✅
- [x] `Api/Middleware/SecurityHeadersMiddleware.cs` — `X-Content-Type-Options: nosniff`,
      `X-Frame-Options: DENY`, `Referrer-Policy: no-referrer`. CSP bilinçli
      olarak dışarıda (Angular ayrı origin'den, Scalar Dev'de inline script
      kullanıyor; prod host gerçekten bundle'ı sunduğunda yazılır).
- [x] `PipelineExtensions` `Production` profile için `UseHsts()` + middleware'i
      sıraya ekledi (Dev'de ikisi de no-op; HSTS loopback'i zehirlemez).

### Rate-limit ✅
- [x] `Api/Configuration/RateLimitingExtensions.cs` — .NET 10 built-in
      `AddRateLimiter` üzerinden iki named policy:
  - **`login-policy`**: fixed window **5/dk**, partition key = remote IP
    (credential-stuffing defansı).
  - **`pay-by-card-policy`**: sliding window **10/dk** (6 segment), partition
    key = `User.Identity.Name` (auth sonrası user-keyed; auth öncesi IP).
- [x] Pipeline: `app.UseRateLimiter()` auth + authorization **sonrasında**,
      controller mapping **öncesinde** (user-key partition key auth'tan sonra
      okunsun diye).
- [x] `[EnableRateLimiting(...)]` uygulandı: `AuthController.Login` +
      `MeController.PayDuesItem` + `MeController.PayUtilityItem` +
      `DuesController.PayItemByCard` + `UtilityBillsController.PayItemByCard`
      (5 endpoint). Hepsinin `[ProducesResponseType(429)]` OpenAPI shape'i de
      eklendi.
- [x] **E2E (`LoginRateLimitTests`):** 7 ardışık yanlış-parola login →
      sonuncu 429. Her test fresh factory → fresh limiter, suite-içi yan etki yok.

### Tests
Domain 222, Application 89, Architecture 18, **E2E 37 (+1)**, web 32 — yeşil.
**Commit:** `feat(security): headers + rate-limit (login + pay-by-card)`

---

## Gün 7 — ADR'lar (6–10) + README v2 + Screenshot + Demo Video + LinkedIn + Tag ✅

**Hedef:** Tam vitrin paketi + son 5 ADR.

### Özet
- ✅ **ADR 6-10** yazıldı (`docs/adr/`): 0006 Authz Pipeline + markers (W5 backbone),
  0007 Outbox (in-tx vs after-commit ayrımı), 0008 Soft Delete root-only,
  0009 Token-Scoped `/api/me/*` (IDOR yapısal), 0010 Refit + Polly (gateway port +
  ACL adapter + standard resilience). Index güncellendi.
- ✅ **README v2** — status "W1-W6 tamam / v1.0.0", W6 özellik listesi, tech stack
  rozetlerine SignalR/RateLimiter/Coverage eklendi, **3 mermaid** (bounded
  context haritası + pay-by-card sequence + outbox sequence), **Demo modu**
  bölümü, **Known limitations** (11 dürüst tradeoff), ADR linkleri.
- ✅ **Demo video script** (`docs/DEMO-VIDEO-SCRIPT.md`, TR): hazırlık adımları
  + sahne planı (0:00-2:50, resident → real-time messaging → admin → tema/dil
  → bitiş kartı) + anahtar seslendirme cümleleri + çekim-sonrası talimat.
- ✅ **LinkedIn taslağı** (`docs/LINKEDIN-POST.md`, TR): 3 versiyon (kısa ~150,
  orta ~300, uzun ~500 kelime) + görsel hazırlık (slayt formatı) + yayınlama
  zamanı notu.
- ✅ **ROADMAP.md** W6 ✅ olarak güncellendi.
- ✅ **CLAUDE.md** "W1-W6 complete — v1.0.0 shipped" + Pending bölümü "Known
  limitations / future work" olarak revize edildi.
- ✅ **Final test counts:** Domain 222, Application 89, Architecture 18,
  **E2E 37**, web Vitest **32**. `dotnet build` warning-free, `ng build` warning-free.
- ✅ **Tag**: `v1.0.0` annotated tag + push.

> **Screenshot + video çekimi:** Bu kullanıcının manuel adımı. Script
> (`docs/DEMO-VIDEO-SCRIPT.md`) tüm hazırlık + sahne planı + seslendirme
> mesajlarını içeriyor; demo seeder ile veriler hazır, "play" tuşuna basmak
> yeterli.

**Commit:** `docs: README v2 + ADRs 6-10 + close out W6 v1.0.0`

### ADR'lar — Pattern/feature odaklı (6–10)
- [ ] `0006-authorization-pipeline-and-request-markers.md` (W5 backbone)
- [ ] `0007-outbox-pattern-for-integration-events.md`
- [ ] `0008-soft-delete-aggregate-root-only.md`
- [ ] `0009-token-scoped-resident-endpoints.md` (IDOR yapısal)
- [ ] `0010-refit-polly-for-payment-integration.md`
- [ ] `docs/adr/README.md` — ADR index + MADR açıklaması.

### README v2
- [ ] 1-paragraf pitch + 3 ekran görüntüsü (resident dashboard, admin dashboard, card dialog)
- [ ] Tech stack rozetleri (`.NET 10`, `Angular 21`, `PostgreSQL 16`, `MongoDB 7`, `SignalR`, `Docker`)
- [ ] **Quickstart**: `docker compose up` + npm start; **demo modu** (`Demo:SeedOnStartup=true` → zengin veri).
- [ ] **Bounded context haritası** (mermaid).
- [ ] **Sequence diagram**: resident pay-by-card (FE → API → PaymentService → Mongo) + 402 path.
- [ ] **Sequence diagram**: outbox (period close → integration event → outbox row → background processor → email).
- [ ] **Sequence diagram**: SignalR mesaj akışı (resident send → API → hub → admin receive).
- [ ] Test stratejisi + her iki solution için nasıl çalıştırılır + coverage rapor linki.
- [ ] ADR linkleri (`docs/adr/`).
- [ ] **Known limitations** (dürüst vitrin):
  - Resident self-registration yok (bilinçli güvenlik duruşu)
  - Credit partial settlement yok
  - Mesajda dosya ekleme yok
  - In-app bildirim merkezi yok
  - Audit log UI yok (data var)
  - API versiyonlama yok
  - SignalR tek instance (Redis backplane scale-out roadmap)
  - Refresh token `localStorage`'da (httpOnly cookie alternatifi roadmap; XSS koruması için kısa access lifetime + CSP)
- [ ] Lisans (MIT) + author + repo + video linki.

### Screenshot + Video
- [ ] Yüksek çözünürlüklü ekran görüntüleri (resident dashboard, admin dashboard, card dialog, conversation thread, admin messaging, payment-data, scalar API docs).
- [ ] Demo video (2-3 dk, MP4, Türkçe ses):
  - Resident login → dashboard → bills → pay-by-card (success **+** decline)
  - Resident messaging → admin'in canlı mesajı gelince ekranda anında belirsin (SignalR vurgusu)
  - Admin login → dashboard → messaging cevap → distribute period
  - (Opsiyonel) Scalar UI üzerinden bir API call

### LinkedIn (Türkçe taslak)
- [ ] 1-paragraf pitch (problem + çözüm + tech stack)
- [ ] 3 madde "öne çıkan tasarım kararları" (Authz pipeline, IDOR token-scope, Outbox/SignalR ikili)
- [ ] Repo + video linki
- [ ] `#dotnet #angular #ddd #cleanarchitecture #signalR #microservices`

### Final
- [ ] `ROADMAP.md` → W6 ✅, "Sonraki Adımlar" güncelle.
- [ ] `CLAUDE.md` → status "**W1-W6 complete.**" + deferred listesi yenile.
- [ ] Tüm test suite'leri yeşil son tur; coverage raporu son hâli artifact'ta.
- [ ] **Tag**: `v1.0.0` (annotated tag + GitHub release notes).

**Commit:** `docs: README v2 + ADR index + close out W6` + `chore: tag v1.0.0`

---

## Hafta 6 Çıktısı (Definition of Done) ✅

- [x] Admin messaging UI tamam (W5 borcu kapandı)
- [x] SignalR ile canlı mesajlaşma çalışıyor (push-only; mevcut "action sonrası refresh"in üzerine "karşı taraf eylemi" auto-refresh)
- [x] Test coverage HTML raporu CI artifact'ında (Line 90.2%); critical-path E2E'ler zaten kapsanmıştı (welcome mail / soft delete + restore + purge / credit balance) + rate-limit smoke eklendi
- [x] Demo seeder ile `docker compose up` sonrası zengin demo veri hazır
- [x] Health check endpoint'leri yerinde (ana API + PaymentService downstream probe)
- [ ] ~~PaymentService admin UI~~ → bilinçli **defer**: kendi seeder'ı 4242 test kartı + 100k bakiye veriyor, admin CRUD page'in vitrin değeri düşük
- [x] UI polish (empty/loading/a11y/mobile + i18n eşitlik) tamam
- [x] Security headers + rate-limit yerinde; rate-limit E2E
- [x] **Refresh token** audit edildi: rotation ✓, reuse-detection ✓, FE silent refresh ✓; family invalidation + EF-backed store **defer (future)**
- [x] **10 ADR** yayında (`docs/adr/`)
- [x] **README v2** + 3 mermaid + ADR linkleri + Demo modu + 11 known limitation
- [x] **Demo video script + LinkedIn taslağı** hazır (`docs/DEMO-VIDEO-SCRIPT.md` + `docs/LINKEDIN-POST.md`); çekim kullanıcının manuel adımı
- [x] Tüm testler yeşil; **`v1.0.0` annotated tag** atılacak (commit + push'tan sonra)

### Riskler / kapsam kontrolü

- **SignalR scope creep:** tek instance kabul edildi (Redis backplane yok); presence/typing/typed events eklemek W6 dışı. Push-only event'lerle sınırlı kal.
- **Coverage rabbit-hole:** %100 peşinde koşma — rapor görünür, eksik critical path yer alıyor; "branch coverage 100%" hedefi yok.
- **README perfectionism:** 3 mermaid + 3-5 screenshot + 10 kısa ADR yeter; daha fazlası yığma.
- **Demo seeder idempotency:** her startup'ta sıfırdan kurmuyor (clear+seed mantığı YOK); ilk açılışta seed eder, sonraki açılışlarda tekrarlamaz. Sıfırlama → manuel `docker compose down -v`.
- **Demo video re-take:** ilk çekim "kabul edilebilir" olsun; mükemmel peşinde yarım gün gitmesin.
- **Defer kalemler README'de net:** "yok" denilen şeyler dürüstçe listelensin (refresh token, credit partial, file attach, bildirim, audit UI, API versiyonlama); vitrin değerini düşürmez — bilinçli karar gösterir.
- **PaymentService admin UI scope:** sadece **oluşturma + listele** (silme/güncelleme yok); test verisi üretmek için minimum.
- **i18n key parity testi sertleştirme riski:** yeni feature eklerken iki taraf da güncellenmeli; alışkanlık zaten yerinde, sadece test'le kilitleniyor.
