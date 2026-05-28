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

## Gün 3 — Test Coverage + Critical-Path E2E

**Hedef:** Coverlet ile rapor üret, eksik critical-path E2E'leri ekle.

- [ ] **Coverlet + ReportGenerator** kur (`coverlet.collector` paketleri varsa kontrol;
      yoksa ekle). `runsettings` veya `dotnet test --collect:"XPlat Code Coverage"`.
- [ ] HTML rapor: `dotnet tool install -g dotnet-reportgenerator-globaltool` +
      `reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html`.
- [ ] CI workflow'a: test → coverage artifact upload (HTML klasörü). Threshold yok.
- [ ] **Eksik critical-path E2E'leri belirle ve ekle:**
  - [ ] **Credit balance journey:** distribute → overpay → distribute again → otomatik mahsup.
  - [ ] **Soft delete + restore + purge** Site E2E.
  - [ ] **Welcome mail outbox** (varsa kontrol; yoksa ekle).
  - [ ] **Rate-limit smoke** (Gün 6'da rate-limit kurulunca ek 1 E2E).
- [ ] **Domain test gap doldur:** coverage raporundan görüp eksik invariant test'leri ekle (özellikle `Conversation` ve `ResidentCreditAccount`).
- [ ] **Application test gap doldur:** behavior testleri tam mı (`AuthorizationBehavior`,
      `ResidentBillOwnershipBehavior`, `ConversationOwnershipBehavior`, `TransactionBehavior`,
      `ExceptionTranslationBehavior`).
- [ ] Rapor PR'da görünür olsun (artifact link README'ye eklenecek Gün 7'de).

**Commit:** `test: coverage harness + critical-path E2E backfill`

---

## Gün 4 — Demo Seeder + Health Check + PaymentService Admin UI

**Hedef:** Klonla → `docker compose up` → tüm demo verisi hazır, zengin. Bonus:
PaymentService için minimal admin UI'ı (test kartı/hesap oluşturma).

### Demo Seeder (ana API)
- [ ] `Infrastructure/Persistence/Seeding/DemoSeeder.cs` (`IDemoSeeder`) —
      idempotent. `Demo:SeedOnStartup=true` ENV flag ile aktif.
- [ ] İçerik:
  - 2 site (farklı şehir/adres)
  - Her site için 2 blok, blok başına 3 daire
  - 5 sakin (TC + telefon + plaka; bilinen şifre — log'a yazılır + welcome mail outbox)
  - Daire atamaları (3 sahip + 2 kiracı)
  - 2026-04 dues period **kapalı** + items üretilmiş + bir kısmı **ödenmiş** (kart kullanıp PaymentService stub'a vurmadan — direct status seed)
  - 2026-05 dues period **açık** + items üretilmiş + 1 kalem ödenmiş, 4 kalem ödenmemiş
  - 2026-05 utility (elektrik 200 TL) period kapalı + dağıtılmış
  - 1-2 conversation (admin başlatmış + resident cevap vermiş; biri unread)
  - 1 resident credit account (overpayment senaryosu — 150 TL kredi)
- [ ] `Program.cs` startup'ta `IDemoSeeder.SeedAsync()` çağrısı flag açıksa.
- [ ] **Önemli:** prod'da kapalı kalır; `.env.example`'da default kapalı + demo için
      `.env`'de açık. README quickstart "demo modu" diye anlatır.

### PaymentService Admin UI
- [ ] **PaymentService backend** — yeni admin endpoint'ler (`Admin` API-key korumalı):
  - `GET /api/bank-accounts` — listele
  - `POST /api/bank-accounts` — oluştur (holder name, currency, initial balance)
  - `GET /api/cards` — listele
  - `POST /api/cards` — oluştur (bank account id, holder name, expiry, cvv; kart no Luhn ile generate)
- [ ] **PaymentService TDD:** komut handler + endpoint testi (`PaymentService.E2E.Tests`).
- [ ] **Ana API proxy** — `IPaymentAdminApi` (Refit) + `Api/Controllers/Admin/PaymentAdminController` (`[Authorize(Roles=Admin)]`). FE → ana API → PaymentService.
- [ ] **Angular:** `/admin/payment-data` sayfası, iki sekme (Hesaplar + Kartlar), liste + dialog'lar.
- [ ] i18n `admin.paymentData.*`.

### Health Check
- [ ] **Ana API:** `AddHealthChecks().AddNpgSql(connStr).AddMongoDb(...)` —
      wait, ana API Mongo kullanmıyor, sadece **PaymentService bağlantısını** kontrol et
      (Refit ile `GET /health`). Endpoint: `GET /health` + JSON çıktı.
- [ ] **PaymentService:** `AddHealthChecks().AddMongoDb(connStr)`. Endpoint: `GET /health`.
- [ ] README'de health URL'leri.

### Demo Seeder Seed üzerinden PaymentService veri
- [ ] PaymentService için demo seeder de — 1 bank account + 1 test kartı
      (`4242 4242 4242 4242`, expiry/cvv bilinen; yeterli bakiyeli). README'de açıkça yaz.

**Commit:** `feat(seed): demo seeder + payment admin UI + health checks`

---

## Gün 5 — UI Polish + i18n + Dependency Sweep + ADR'lar (1–5)

**Hedef:** Tutarlı UI; eksik çeviri kalmaz; paketler güncel + ilk 5 ADR yazılı.

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

## Gün 6 — Security Pass (headers + rate-limit + refresh token)

**Hedef:** Alt-sınır güvenlik hijyeni + refresh token ile temiz auth UX.

### Security headers
- [ ] **Middleware** (`Production` profile için):
  - `Strict-Transport-Security` (HSTS)
  - `X-Content-Type-Options: nosniff`
  - `X-Frame-Options: DENY`
  - `Referrer-Policy: no-referrer`

### Rate-limit
- [ ] **.NET 10 built-in `AddRateLimiter`:**
  - `login` endpoint → fixed window, 5/dk (IP key).
  - `pay-by-card` endpoint'leri (admin + resident) → sliding window, 10/dk (user key).
- [ ] E2E: login spam → 429.

### Refresh token (yeni)
**Kararlar (Gün 6 başında kilitlenecek mini başlıklar):**
- **Access lifetime:** 15 dk (kısa, sızıntı riski düşer).
- **Refresh lifetime:** 7 gün (sliding window).
- **Storage:** `RefreshToken` tablo (Identity DbContext) — `{ Id, UserId, TokenHash (SHA-256), ExpiresAt, RevokedAt?, ReplacedByTokenHash?, FamilyId }`. Plain token client'a; **hash** sunucuda.
- **Rotation:** her refresh → eski revoke + yeni üret (aynı family). Reuse detection: revoked bir token tekrar gelirse → **tüm family invalidate** (compromise sinyali).
- **Logout:** refresh token revoke (family kapat).
- **FE storage:** `localStorage` (httpOnly cookie alternatifini değerlendirme: SPA + Scalar için karmaşık; localStorage + kısa access kabul edilebilir vitrin için — README'de "tradeoff" diye yaz).

**Backend:**
- [ ] `Domain/Identity/RefreshToken.cs` (entity, aggregate root değil — Identity'nin altı).
- [ ] EF config + migration (`AddRefreshTokens`).
- [ ] `IRefreshTokenService` (Application): `IssueAsync(userId)`, `RotateAsync(plainToken)` (reuse → family revoke), `RevokeAsync(plainToken)`.
- [ ] `POST /api/auth/refresh` — body `{ refreshToken }`; başarı: yeni `{ accessToken, refreshToken, accessTokenExpiresAt }`; başarısızlık (expired/revoked/family compromise) → **401**.
- [ ] `POST /api/auth/logout` — `[Authorize]`, refresh token revoke.
- [ ] `LoginCommand` response'una `refreshToken` + `accessTokenExpiresAt` ekle.
- [ ] **TDD:** `RefreshTokenServiceTests` (issue, rotate happy, reuse → family revoke, expired → 401).
- [ ] **E2E:** `AuthRefreshTests` — login → access expire simüle → refresh → yeni token ile request 200; revoked token tekrar → 401 + family invalidate.

**Frontend:**
- [ ] `auth.service` → `accessToken`, `refreshToken`, `accessTokenExpiresAt` storage.
- [ ] HTTP interceptor — 401 yakala → **tek refresh promise** (race koruması) → yeni access ile orijinal request retry. Refresh de 401 alırsa → logout + login redirect.
- [ ] Logout: `POST /api/auth/logout` + clear storage.
- [ ] Vitest: interceptor (3 test — happy refresh, refresh 401 → logout, eşzamanlı 401 tek refresh).

### JWT lifetime config
- [ ] `Jwt:AccessTokenLifetimeMinutes=15`, `Jwt:RefreshTokenLifetimeDays=7` (`appsettings` + `.env.example`).

**Commit:** `feat(security): headers + rate-limit` + `feat(auth): refresh token with rotation + family invalidation`

---

## Gün 7 — ADR'lar (6–10) + README v2 + Screenshot + Demo Video + LinkedIn + Tag

**Hedef:** Tam vitrin paketi + son 5 ADR.

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

## Hafta 6 Çıktısı (Definition of Done)

- [ ] Admin messaging UI tamam (W5 borcu kapandı)
- [ ] SignalR ile canlı mesajlaşma çalışıyor; polling kaldırıldı
- [ ] Test coverage HTML raporu CI artifact'ında; eksik critical-path E2E'ler eklendi (credit balance + soft delete + welcome mail + rate-limit smoke)
- [ ] Demo seeder ile `docker compose up` sonrası zengin demo veri hazır
- [ ] Health check endpoint'leri yerinde (ana API + PaymentService)
- [ ] PaymentService admin UI tamam (bank account + card CRUD)
- [ ] UI polish (empty/loading/a11y/mobile + i18n eşitlik) tamam
- [ ] Security headers + rate-limit yerinde; rate-limit E2E
- [ ] **Refresh token** + rotation + family invalidation; FE silent refresh interceptor; auth E2E
- [ ] 10 ADR yayında (`docs/adr/`)
- [ ] README v2 + mermaid diyagramlar + screenshot + ADR linkleri + known limitations
- [ ] Demo video kayıtlı (TR, 2-3 dk MP4); LinkedIn taslağı hazır
- [ ] Tüm testler yeşil; `v1.0.0` etiketi atıldı

### Riskler / kapsam kontrolü

- **SignalR scope creep:** tek instance kabul edildi (Redis backplane yok); presence/typing/typed events eklemek W6 dışı. Push-only event'lerle sınırlı kal.
- **Coverage rabbit-hole:** %100 peşinde koşma — rapor görünür, eksik critical path yer alıyor; "branch coverage 100%" hedefi yok.
- **README perfectionism:** 3 mermaid + 3-5 screenshot + 10 kısa ADR yeter; daha fazlası yığma.
- **Demo seeder idempotency:** her startup'ta sıfırdan kurmuyor (clear+seed mantığı YOK); ilk açılışta seed eder, sonraki açılışlarda tekrarlamaz. Sıfırlama → manuel `docker compose down -v`.
- **Demo video re-take:** ilk çekim "kabul edilebilir" olsun; mükemmel peşinde yarım gün gitmesin.
- **Defer kalemler README'de net:** "yok" denilen şeyler dürüstçe listelensin (refresh token, credit partial, file attach, bildirim, audit UI, API versiyonlama); vitrin değerini düşürmez — bilinçli karar gösterir.
- **PaymentService admin UI scope:** sadece **oluşturma + listele** (silme/güncelleme yok); test verisi üretmek için minimum.
- **i18n key parity testi sertleştirme riski:** yeni feature eklerken iki taraf da güncellenmeli; alışkanlık zaten yerinde, sadece test'le kilitleniyor.
