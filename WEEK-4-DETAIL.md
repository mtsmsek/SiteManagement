# Hafta 4 — Payment Microservice (Detaylı Plan)

> **Hedef:** "Resident borcunu görüp kredi kartıyla ödeyebiliyor; ödeme ayrı bir
> microservice'te (MongoDB) işlenir, başarılıysa ana API ilgili billing item'ı
> `Paid` yapar."
>
> **Definition of Done:** Resident login → borç listesi → kart bilgisi → öde →
> item `Paid`; `PaymentTransaction` MongoDB'de; iki container birden ayakta full
> E2E geçiyor; failure-path (yetersiz bakiye, reddedilen kart, çift ödeme) testleri
> de yeşil; mimari testler (her iki solution'da) yeşil.

W1-W3 desenini izler ama **ayrı bir solution** (`payment-service/`) — kendi
Domain/Application/Infrastructure/Api katmanları, kendi testleri, MongoDB ile.
Ana API ona **Refit + Polly** ile HTTP üzerinden konuşur.

---

## Mimari Kararlar (oturumda alındı)

| Karar | Seçim | Gerekçe |
|---|---|---|
| **Servis iletişimi** | **Senkron**: ana API `PayItemCommand`'da PaymentService'i Refit+Polly ile çağırır, sonucu bekler; başarılıysa **aynı istekte** BillingItem'ı `Paid` yapar | Tek kullanıcı akışı, PDF'e yeterli; mesaj kuyruğu/webhook karmaşası yok |
| **Servis sınırı** | PaymentService **generic bir ödeme gateway'i** — kart + tutar + referans alır, onaylar/reddeder, `PaymentTransaction` yazar. Aidat/fatura/site kavramını **bilmez** | Gerçek bir payment gateway gibi; bağımsız deploy edilebilir; polyglot persistence + temiz sınır vitrini |
| **Fake banka** | Mongo'da `BankAccount` (bakiye) + `CreditCard` (no/cvv/sktarihi) rich aggregate'lar; ödeme = kart geçerli mi + bakiye yeterli mi → onayla/reddet | ROADMAP'teki aggregate'larla birebir; rich domain gösterir |
| **Idempotency** | Her ödeme isteği bir **idempotency key** taşır; aynı key ikinci kez gelirse yeni ödeme yapılmaz, ilk sonuç döner | Çift tıklama/retry'da çift tahsilat yok; ROADMAP maddesi; guardrail test'le kilitlenir |
| **Ödeme yönü** | Kart → site (sakin borcunu öder). `BankAccount` = sakinin (kartın) hesabı; bakiye düşer | PDF "resident kartla öder" |

### Açık tasarım soruları (W4 başında netleşecek)
- **Para birimi paylaşımı:** ana API'deki `Money` VO'yu PaymentService'e kopyalayalım mı, yoksa PaymentService kendi `Money`'sini mi tutsun? → Öneri: **kendi kopyası** (servisler kod paylaşmaz, sadece DTO/HTTP sözleşmesi paylaşır — gerçek microservice prensibi).
- **Idempotency key'i kim üretir:** ana API mi (billing item id + deneme), client mi? → Öneri: **ana API üretir** (item başına deterministik ya da GUID), PaymentService saklar.
- **BankAccount/CreditCard seed:** test/demo için kartlar nasıl oluşur? → Öneri: PaymentService'te seed + bir "hesabıma bakiye yükle" admin/test endpoint'i (fake olduğu için).
- **PaymentService auth:** ana API → PaymentService çağrısı nasıl korunur? → Öneri: basit API-key header (servis-servis), JWT değil (PaymentService kullanıcı bilmez).

---

## Gün 1 — PaymentService Solution İskeleti + Mongo Bağlantısı

**Hedef:** `payment-service/` ayrı solution ayağa kalkıyor, Mongo'ya bağlanıyor, health veriyor, compose'a ekli.

- [ ] `payment-service/` klasörü + `PaymentService.slnx`:
  - `PaymentService.Domain` (BCL-only), `.Application`, `.Infrastructure` (Mongo), `.Api`
  - test: `PaymentService.Domain.Tests`, `.E2E.Tests`
- [ ] Katman referansları + `Directory.Packages.props` paylaşımı (kök props'u kullan ya da kendi props'u — karar Gün 1)
- [ ] MongoDB driver (`MongoDB.Driver`) — Infrastructure'a
- [ ] `Api/Program.cs` lean; Mongo connection options (`.env`'den), health check (`/health` + Mongo ping)
- [ ] `docker-compose.yml`: `payment-api` servisi (mongo'ya depends_on healthy; kendi portu örn. `:8090`)
- [ ] Shared DDD primitive'leri (Entity/AggregateRoot/ValueObject) — ana API'den **kopya** (servis bağımsızlığı) ya da minimal kendi versiyonu
- [ ] Smoke: `docker compose up` → payment-api `/health` 200, Mongo bağlı

**Commit:** `feat(payment): payment-service solution skeleton + mongo + compose`

---

## Gün 2 — PaymentService Domain (TDD)

**Hedef:** Rich aggregate'lar, fake banka mantığı, invariant'lar tam.

**Domain — `PaymentService.Domain/`:**
- [ ] `ValueObjects/Money.cs` (kendi kopyası — TRY, non-negative, rounding) + `CardNumber` (16 hane, Luhn?), `Cvv`, `ExpiryDate` (gelecekte mi)
- [ ] `BankAccount : AggregateRoot` — `Balance` (Money), `Debit(amount)` (yetersizse `InsufficientBalanceException`), `Credit(amount)`
- [ ] `CreditCard : AggregateRoot` (veya BankAccount içinde) — kart bilgisi + hangi BankAccount'a bağlı; `Validate()` (skt geçmiş mi, cvv)
- [ ] `PaymentTransaction : AggregateRoot` — `IdempotencyKey`, `Reference` (ana API'nin billing item id'si, opaque string), `Amount`, `Status` (Pending/Succeeded/Failed), `FailureReason`; factory + state geçişleri
- [ ] `Exceptions/` — `InsufficientBalanceException`, `CardRejectedException` (skt/cvv), `PaymentMessageKeys`
- [ ] TDD: `BankAccountTests` (debit/credit/yetersiz bakiye), `CreditCardTests` (geçerlilik), `PaymentTransactionTests` (state geçişleri, idempotency invariant)

**Commit:** `feat(payment): domain — bank account, credit card, transaction (TDD)`

---

## Gün 3 — PaymentService Application + Infrastructure (Mongo)

**Hedef:** Ödeme işleme use-case'i + Mongo repository'leri.

**Application:**
- [ ] `ProcessPaymentCommand(IdempotencyKey, CardNumber, Cvv, ExpiryDate, Amount, Reference)` → handler:
  - idempotency: bu key ile transaction var mı? varsa onun sonucunu dön (yeni ödeme yapma)
  - kartı bul + doğrula → reddse `CardRejectedException`
  - BankAccount.Debit → yetersizse `InsufficientBalanceException`
  - `PaymentTransaction` Succeeded yaz, sonucu dön
- [ ] FluentValidation (kart no/cvv/tutar şekil kontrolü)
- [ ] Exception → ProblemDetails (ana API'nin desenini bu serviste de kur — minimal)

**Infrastructure (Mongo):**
- [ ] `MongoDbContext` / collection erişimi; `IBankAccountRepository`, `IPaymentTransactionRepository` (Mongo impl)
- [ ] Idempotency key üzerinde **unique index** (son savunma — W3'teki partial-index dersi)
- [ ] Seed: demo BankAccount + CreditCard (fake bakiye)

**Tests:** Application handler unit (NSubstitute repo) — idempotency tekrar, yetersiz bakiye, red kart.

**Commit:** `feat(payment): process-payment use case + mongo persistence + idempotency`

---

## Gün 4 — PaymentService API + Ana API Entegrasyonu (Refit + Polly)

**Hedef:** PaymentService HTTP endpoint'i + ana API'nin onu resilient çağırması.

**PaymentService.Api:**
- [ ] `POST /api/payments` — `ProcessPaymentRequest` → `ProcessPaymentResponse` (transactionId, status). API-key header ile korunur.
- [ ] (test/demo) `POST /api/accounts/{id}/topup` — fake bakiye yükle
- [ ] Scalar/OpenAPI

**Ana API entegrasyonu — `SiteManagement.Infrastructure`:**
- [ ] `IPaymentGateway` (Application port) — `Task<PaymentResult> ChargeAsync(...)` (domain-friendly; HTTP detayı bilmez)
- [ ] Refit client `IPaymentServiceApi` + `IPaymentGateway` adapter (ROADMAP "Adapter pattern")
- [ ] Polly resilience: retry (transient) + timeout + circuit breaker (`Microsoft.Extensions.Http.Resilience`)
- [ ] `PayItemCommand` (ana API): mevcut `MarkItemPaid` zincirini genişlet → önce `IPaymentGateway.ChargeAsync` (idempotency key = item bazlı) → başarılıysa `period.MarkItemPaid(itemId)` → kaydet. Reddse `PaymentRejectedException` → ProblemDetails (lokalize)
- [ ] Config: PaymentService base URL + API-key (`.env`, compose)

**Commit:** `feat(api): pay-item via payment-service (refit + polly adapter)`

---

## Gün 5 — Angular Ödeme Akışı

**Hedef:** Resident-facing değil (henüz portal yok) — **admin** bir item'ı kartla "öde" diyebiliyor (PDF'in resident akışı W5 resident portal'da tamamlanır; W4'te akış admin üzerinden gösterilir).

- [ ] `npm run gen:api` (ana API'nin yeni pay endpoint'i)
- [ ] Billing item tablosunda "Öde" butonu → kart formu dialog (no/cvv/skt/tutar readonly) → `PayItemCommand` çağır → başarı/hata snackbar → item `Paid` görünür
- [ ] Hata gösterimi: yetersiz bakiye / red kart / gateway down (Polly tükendi) → lokalize mesaj
- [ ] i18n `payment.*` (tr+en)

**Commit:** `feat(web): card payment dialog for billing items`

---

## Gün 6 — İki-Container E2E + Failure Paths

**Hedef:** Ana API + PaymentService birlikte ayakta, full pay flow + hata yolları test.

- [ ] E2E altyapısı: PaymentService'i de Testcontainers ile ayağa kaldır (ya da WebApplicationFactory ikilisi) — ana API onun gerçek URL'sine konuşsun
- [ ] **Happy path:** site→sakin→ata→dönem→dağıt→**öde**→item `Paid` + Mongo'da `PaymentTransaction` Succeeded
- [ ] **Failure paths:**
  - yetersiz bakiye → ödeme 402/409, item `Unpaid` kalır (atomik: başarısız ödeme item'ı Paid yapmaz)
  - reddedilen kart (skt geçmiş) → hata, item `Unpaid`
  - **idempotency:** aynı key ile iki kez öde → tek tahsilat, ikinci çağrı ilk sonucu döner
  - **gateway down:** PaymentService kapalıyken öde → Polly retry+circuit → ana API temiz hata (item `Unpaid`)
- [ ] PaymentService kendi Domain + Application testleri yeşil

**Commit:** `test(e2e): two-service payment flow + failure paths (insufficient/rejected/idempotent/gateway-down)`

---

## Gün 7 — Self-Review + Doc

- [ ] **Self-review:** PaymentService Domain BCL-only mu? Money negatif/rounding testli mi? Idempotency guardrail (unique index + handler) çalışıyor mu? Ana API PaymentService'in iç tiplerine değil sadece HTTP sözleşmesine mi bağlı (adapter sınırı)? Servisler kod paylaşmıyor mu (sadece DTO)?
- [ ] Architecture test: PaymentService kendi katman sınırları (Domain → Application → Infra/Api); ana API'de `IPaymentGateway` Application'da, Refit impl Infrastructure'da
- [ ] **Manuel uçtan uca:** UI'dan kartla öde → MailHog değil (bu ödeme), Mongo Compass/shell'de transaction'ı gör → item Paid
- [ ] `WEEK-4-DETAIL.md` + `ROADMAP.md` W4 ✅ + `CLAUDE.md` status

**Commit:** `docs: close out W4` (+ self-review düzeltmeleri)

---

## Hafta 4 Çıktısı (Definition of Done)

- [ ] Ana API'den (UI üzerinden) bir billing item kredi kartıyla ödenebiliyor
- [ ] Ödeme ayrı PaymentService'te işleniyor, `PaymentTransaction` MongoDB'de
- [ ] Başarılı ödeme → item `Paid` (atomik: başarısız ödeme item'ı değiştirmez)
- [ ] Refit + Polly (retry/timeout/circuit breaker) ile resilient servis çağrısı
- [ ] Idempotency: çift ödeme önleniyor (unique index + handler)
- [ ] İki-container E2E + failure-path testleri yeşil; her iki solution'ın testleri yeşil
- [ ] **W5 köprüsü:** resident portal gelince bu pay akışı resident'ın kendi borcuna bağlanacak (şimdilik admin tetikliyor)

### Riskler / kapsam kontrolü
- **Scope creep (ROADMAP riski):** "sadece fake bank, fake card, başka şey yok." Gerçek gateway, 3D secure, taksit, iade → **W4 dışı.**
- **İki solution karmaşası:** PaymentService bilinçli olarak ana API'den kod paylaşmaz (DDD primitive'leri kopya). "DRY ihlali mi?" → hayır, microservice bağımsızlığı kasıtlı; bunu README/commit'te belirt.
- **Dağıtık tutarlılık:** ödeme başarılı ama ana API kaydı patlarsa? → senkron akışta: önce charge, sonra MarkItemPaid; MarkItemPaid patlarsa idempotency key sayesinde retry güvenli (charge tekrar etmez). Bu inceliği Gün 4'te netleştir.
