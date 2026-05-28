# Canlı Demo Runbook

Birine (mülakatçı, hocaya, arkadaşa) ekran paylaşıp canlı demo göstereceğin zaman bu dosya kılavuzun. Sahne sahne ne tıklayacağın, hangi cümleyi sıkıştıracağın, "bunu nasıl yaptın?" sorusu gelirse hangi ADR'a pivot edeceğin yazılı.

---

## 1) Demo öncesi — 10 dk hazırlık

Sıfırdan temiz veri için sıralı çalıştır. Veriler birikmiş + bayatlamışsa karışıklık olur, baştan kur:

```powershell
# 1. Demo seed flag açık mı kontrol et (.env'de DEMO_SEED_ON_STARTUP=true olmalı)
Get-Content .env | Select-String DEMO_SEED

# 2. Stack'i sıfırla + temiz seed
docker compose down -v
docker compose up -d --build api

# 3. API sağlıklı mı bekle
curl http://localhost:8080/health

# 4. Angular dev server (ayrı terminal)
cd web
npm start
# -> http://localhost:4200 açık olsun
```

**Tarayıcıda 3 sekme aç:**

| Sekme | URL | Niçin |
|---|---|---|
| 1 | http://localhost:4200/login | Demo açılış sayfası |
| 2 | http://localhost:8025 | MailHog — welcome mail'leri buradan alacaksın |
| 3 | http://localhost:8080/scalar/v1 | API docs, biri "endpoint'leri nasıl?" diye sorarsa |

> **İpucu:** Tarayıcıyı incognito/private moduna geç. Localstorage'daki eski token'lar veya dil tercihi seni şaşırtmasın. Bildirimleri kapat, Discord/Slack overlay'i yokken kayıt güzel olur.

---

## 2) Kredentialler — yapıştırmaya hazır

```text
Admin:
  e-posta: admin@sitemanagement.local
  şifre:   Str0ng-P@ss-Dev

Resident (Mert — ödenmemiş borcu + admin'in açtığı mesajı olan):
  e-posta: mert.kaya@demo.local
  şifre:   ← MailHog'da "Welcome to SiteManagement" mailinin içinde

Test kartı (PaymentService seeder'ı koyuyor, bakiyeli):
  Kart numarası: 4242 4242 4242 4242
  CVV: 123
  Son kullanma: 12 / 2030
```

Decline yapmak istersen (çok güzel görünür): **CVV'yi 999** yap veya **expiry'yi 01/2020** seç → 402 + animasyonlu hata snackbar'ı.

---

## 3) Demo akışı (yaklaşık 5-7 dk)

### 0:00 — Açılış cümlesi

> "Bu, .NET 10 + DDD ile yazılmış bir apartman sitesi yönetim platformu. Ödeme tarafı ayrı bir microservice, frontend Angular 21. Bir sakinin kendi borcunu kartla ödemesini ve admin tarafıyla nasıl konuştuğunu göstereceğim."

### Sahne 1 — Resident giriş

1. Önce MailHog sekmesine geç → "Mert Kaya" welcome mail'ini aç → şifreyi kopyala
2. Login sayfasına dön
3. E-posta + şifre → Sign in

**Söylenebilir:** "Resident hesabını admin yaratıyor, şifreyi sistem üretiyor, mail ile gidiyor. Public register endpoint'i bilinçli olarak yok."

### Sahne 2 — Resident dashboard

- Üç tile: outstanding / credit / unread messages
- "Açık borç" tile'ında 500 ₺ + 2 ödenmemiş kalem görünmeli

**Söylenebilir:** "Dashboard saf bir read projection. Domain entity sınırı geçmiyor — `IReportQueries` yazıyor, sadece DTO."

### Sahne 3 — Borçlarım sayfası

- Sol menüden "My Bills"
- Tabloda 1 paid + 2 unpaid kalem
- Bir unpaid satırda **Pay by card** butonuna bas

**Söylenebilir:** "Sayfanın URL'i `/api/me/bills` — route'ta resident id yok. ID JWT'de, başka bir sakinin verisine `/api/me/bills?id=...` ile zıplayamazsın. Yapısal IDOR koruması."

### Sahne 4 — Kartla ödeme

- Dialog açılır → kart bilgilerini yapıştır
- Pay → snackbar "Paid", kalem satırı **Paid** chip'ine döner

**Söylenebilir:** "Charge başarılı olursa kalem `Paid`'e döner — aynı transaction içinde. Decline olsaydı kalem `Unpaid` kalır, API 402 döner. PaymentService'e Refit + Polly ile gidiyor, idempotency anahtarı kalem ID'si."

**Bonus — decline göster:**
1. İkinci unpaid kalemde Pay by card
2. CVV `999` ya da expiry `01/2020`
3. → kırmızı animasyonlu snackbar "Payment rejected", kalem unpaid kalır

### Sahne 5 — Mesajlaşma + SignalR

1. Sol menüden "Messages"
2. "Dues reminder" thread'inde unread badge → tıkla
3. Mesajları görür, badge temizlenir

**SignalR vurgusu için ikinci tarayıcı pencerelerini hazırla:**

- **Resident pencereyi mesajlaşma sayfasında bırak**
- **Yan tarafta admin pencere aç** (private window) → admin login → admin Messages
- Admin'den bir cevap yaz + Send
- **Resident pencerede mesaj refresh olmadan anında belirsin** ← bunu göster, pause ver

**Söylenebilir:** "SignalR push. Polling yok — message handler `IMessagingNotifier` üzerinden hub'a yazıyor, hub `messaging:resident:{id}` grubuna basıyor. Outbox değil çünkü ephemeral; durable mail aksine Outbox'tan gidiyor."

### Sahne 6 — Admin tarafı

- Admin penceresinde Dashboard
- Tile'lar: 3 sakin / tahakkuk / tahsil / açık bakiye / kredi / tahsilat oranı

**Söylenebilir:** "Admin dashboard system-wide. Aynı `IReportQueries` üzerinden, ama farklı projection. Authorization marker `IAdminRequest`; resident'ın bu endpoint'e gelmesi pipeline'da 403."

### Sahne 7 — Kapatış

- Logout
- Browser'da repo URL'ini göster

**Kapatış cümlesi:**

> "10 ADR (`docs/adr/`) ile mimari kararlar yazılı. %90 satır kapsama, 377 backend testi + 32 web testi. MIT lisanslı, GitHub'da açık."

---

## 4) Muhtemel sorular — pivot noktaları

| Soru | Cevap kısaca | Detay nereden? |
|---|---|---|
| "IDOR'a karşı nasıl koruyorsun?" | Route'ta id taşımıyorum. Token-scoped `/api/me/*`. | [ADR 0009](adr/0009-token-scoped-resident-endpoints.md) |
| "Authorization nerede?" | MediatR pipeline. Her request **tam bir** rol marker'ı; arch test build'i kırıyor unutulursa. | [ADR 0006](adr/0006-authorization-pipeline-and-request-markers.md) |
| "Payment'i neden ayırdın?" | PDF zorunlu kıldı + polyglot persistence göstergesi. Diğer her şey monolith. | [ADR 0003](adr/0003-modular-monolith-and-payment-microservice.md) |
| "Outbox + SignalR neden ikisi de var?" | Outbox durable async (mail). SignalR ephemeral real-time (UI). Farklı garanti seviyeleri. | [ADR 0007](adr/0007-outbox-pattern-for-integration-events.md) |
| "Result\<T\> kullansaydın?" | Bilerek kullanmadım — typed exception + pipeline translation + localize. Handler'da try/catch yok. | [ADR 0004](adr/0004-exception-based-error-handling.md) |
| "Rich domain ne demek?" | Setter'lar private, behaviour aggregate'in üstünde. Anemic değil. | [ADR 0002](adr/0002-rich-domain-model.md) |
| "Soft delete?" | Sadece root'ta (Site). Inner entity'ler root üzerinden erişiliyor zaten. | [ADR 0008](adr/0008-soft-delete-aggregate-root-only.md) |
| "Test stratejisi?" | Domain TDD %95+, App handler unit, E2E Testcontainers + WireMock (payment için consumer-side contract). Architecture testler guardrail. | [ROADMAP §5](../ROADMAP.md) |
| "Localization?" | `IStringLocalizer` + `MessageKey` her exception'da. Tek noktada translate. Arch test eksik anahtarı kırıyor. | — |
| "Demo veri nereden geliyor?" | `DemoSeeder` idempotent. `Demo:SeedOnStartup=true` flag, .env'den. | `Infrastructure/Persistence/Seed/DemoSeeder.cs` |
| "Refresh token?" | Rotation + reuse-detection var. Family invalidation yok, in-memory — README'de known limitation. | [README §Known limitations](../README.md) |
| "Deploy nerede?" | Bilinçli demo-only; helper'lar (PortBinding, DatabaseUrl) yerinde, Railway/Render swap-in. | [README §Demo modu](../README.md) |

---

## 5) Bir şey patlarsa — fallback

**Senaryo:** Docker yavaş açtı, dev server build ediyor, demo başlamasına 1 dk var.

→ `docs/demo-video.mp4`'ü aç, oynatırken canlı anlat. Aynı akışı izliyor, panik yok.

**Senaryo:** Live'da bir click yanlış yere düştü, sayfa açılmadı.

→ Sakin ol. "Reset edip baştan deneyelim" yerine: doğrudan URL bar'a `/resident/dashboard` veya `/admin/messaging` yazıp navigate et. Hız geri gelir.

**Senaryo:** SignalR push çalışmadı (admin'den mesaj attın, resident pencerede belirmedi).

→ Genelde tarayıcı sekmesinin focus'u kayıp WebSocket reconnect olmamıştır. Resident sekmeye **bir kez tıkla** + sayfayı **F5 ile refresh et** → hub yeniden bağlanır, sonraki mesaj anında gelir. Veya doğrudan "şu an reconnect oluyor, normalde anında geliyor; bir refresh sonra göstereyim" deyip devam et.

**Senaryo:** Tanımadığın bir teknik soru geldi.

→ "Onu hatırlamıyorum tam, ama mimari kararı ADR'da yazmıştım, bir saniye" → `docs/adr/`'a gidip ilgili dosyayı aç. Hazırlıklı görünür + dürüst.

---

## 6) Demo bitti — kapatma

```powershell
docker compose down
# volume'leri silmek istersen: docker compose down -v
```

Tarayıcı sekmelerini kapat, dev server'ı Ctrl+C ile durdur. Bir sonraki demo için `.env`'deki `DEMO_SEED_ON_STARTUP=true` kalsın.

---

## 7) Mini hile kâğıdı (single page)

Mülakat masasına kâğıt götürebileceksen tek sayfa:

```
PROJE: SiteManagement   |   v1.0.0   |   github.com/mtsmsek/SiteManagement

STACK
  Backend:  .NET 10 + ASP.NET Core + EF Core 10 + MediatR + FluentValidation
            + Refit + Polly + Serilog + Scalar
  DB:       PostgreSQL 16 (main) + MongoDB 7 (payment microservice)
  Frontend: Angular 21 (standalone, signals) + Material 3 + ngx-translate
  Tests:    xUnit + FluentAssertions + NSubstitute + Testcontainers + WireMock
  Real-time: SignalR (messaging)

ÖNE ÇIKAN KARARLAR (her biri için ADR var: docs/adr/)
  1. DDD + Clean Architecture (Domain → App → Infra → Api)
  2. Rich domain (anti-anemic)
  3. Modular monolith + tek microservice (payment)
  4. Exception + MessageKey (no Result<T>), pipeline translation
  5. CQRS-lite via MediatR
  6. Authz pipeline + arch-test guardrail (unutmak = build hatası)
  7. Outbox pattern (integration events durable)
  8. Soft delete sadece root'ta
  9. Token-scoped /api/me/* — yapısal IDOR
 10. Refit + Polly (payment integration)

ÖZ KAYNAK
  377 backend test + 32 web Vitest
  Line coverage: 90.2%, Branch 82.3%
  10 ADR yazılı, README'de mermaid diyagram x3

LOGIN
  admin@sitemanagement.local / Str0ng-P@ss-Dev
  mert.kaya@demo.local / (welcome mail'den, MailHog :8025)
  Test kartı: 4242 4242 4242 4242  CVV 123  12/2030
```

---

İyi şanslar kral. Sakin ol, sahne planını bil, soruya saplanma → ADR'a yönlendir. 👊
