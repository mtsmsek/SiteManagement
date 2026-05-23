# Hafta 3 — Tenancy & Billing Domain (Detaylı Plan)

## 📌 DURUM (son oturum sonu)

- **Gün 1–6: ✅ tamamlandı** ve `main`'e push'landı.
- **Gün 7 (E2E + Self-Review + Doc): ⏳ KALAN İŞ** — tek kalan W3 maddesi. Aşağıdaki Gün 7 bölümüne bak.
  - Yapıldı: `BillingFlowTests` E2E (aç → ata → dağıt → kalemler → öde) var.
  - Kalan: özel `TenancyFlowTests` + **failure-path** testleri (kapalı döneme dağıtım → 409, boş dağıtım, çift aktif atama → 409); **self-review**; **`IQuery` marker + "her request `ICommand` veya `IQuery` olmalı" architecture testi** (şu an sadece Command tarafı marker'lı); `WEEK-3-DETAIL.md` + `ROADMAP.md` W3 ✅ işaretle.
- **PLAN DIŞI — bu hafta fazladan yapıldı** (backend sağlamlaştırma; detay `CLAUDE.md`):
  item **payment recording** · **UI modernizasyon** (kart tabanlı siteler/billing, login dil switcher) · **transactional Outbox** (mail/event commit sonrası) · **soft delete + restore + hard purge** (Site) · **audit** (created/modified by+at, interceptor) · **tüm Application command-handler unit testleri**. ~283 test yeşil.

> Yarın: `git pull` → `CLAUDE.md`'yi oku (konvansiyonlar + gotchas) → Gün 7'yi bitir.

---

> **Hedef:** "Admin bir sakini daireye atayabiliyor; bir ayı açıp tüm dolu dairelere aidat + fatura toplu dağıtabiliyor; bildirimler MailHog'a düşüyor."
>
> **Definition of Done:** Bir admin sıfırdan başlayıp bir ayı tamamen kurabiliyor (ata → dönem aç → toplu dağıt → kapat); mailler MailHog'a düşüyor; full E2E akışı + failure-path testleri yeşil; mimari testler (Clean Architecture sınırları) hâlâ yeşil.

W1-W2'nin desenini birebir izler: **Domain (TDD, BCL-only) → Application (CQRS-lite, MediatR) → Infrastructure (EF mapping + repo + query) → Api (thin controller) → Angular (api → store → component)**. Her gün sonunda commit + CI yeşil.

---

## Mimari Kararlar (oturumda alındı)

| Karar | Seçim | Gerekçe |
|---|---|---|
| **Money modellemesi** | `Money` value object, tek para birimi **TRY** (`decimal Amount`) | Billing + Payment ortak kullanır; negatif/yuvarlama kuralları tek yerde; magic decimal yok |
| **Aidat tutarı** | Daire başı **sabit tutar** (admin dönem açarken tek tutar girer, tüm dolu dairelere eşit) | En basit, PDF'e yeterli; tip-bazlı değişken fiyat W6 "nice-to-have" |
| **Tenancy ↔ Occupancy** | **Event-driven**: `ApartmentAssignment` oluşunca domain event → handler `Apartment.MarkAsOccupied()` çağırır | Aggregate'lar gevşek bağlı (DDD doğrusu); W2'deki "daireye sakin atama" eksiğini kapatır |
| **DuesPeriod vs UtilityBillPeriod** | **Yaklaşım A — ayrı aggregate'lar** (ROADMAP kararı) | PDF'in "aidat" vs "fatura (elektrik/su/doğalgaz)" ayrımına sadık |
| **Bounded context** | `Tenancy` + `Billing` ayrı klasör/namespace | Property/Residency ile aynı modüler-monolit deseni |

### Açık tasarım soruları (W3 başında netleşecek)
- `ApartmentAssignment` aggregate root mu, yoksa `Apartment` içinde entity mi? → **Root** öneriliyor: tarihçe (assignment history) ayrı yaşam döngüsü, daire-sakin many-to-one-over-time.
- Domain event'lerin dispatch noktası: `SaveChangesAsync` öncesi mi sonrası mı? → W3 Gün 1'de `IDomainEventDispatcher` tasarlanacak (şu an event altyapısı var ama dispatch edilmiyor).
- Aynı daireye ikinci aktif assignment engeli: Tenancy aggregate invariant'ı + DB partial unique index.

---

## Gün 1 — Money VO + Domain Event Dispatch Altyapısı

**Hedef:** Para modellemesi hazır; domain event'ler artık gerçekten dispatch ediliyor (W1'de `AggregateRoot` event listesi vardı ama kimse dinlemiyordu).

**Domain:**
- [ ] `Domain/Shared/ValueObjects/Money.cs` — `decimal Amount`, sabit `Currency = "TRY"`; factory `Money.Of(decimal)` (negatif reddeder → `NegativeMoneyException`), `Zero`; metotlar: `Add`, `Subtract`, `Multiply(int)`, karşılaştırma; `ValueObject`'ten türer
- [ ] `Domain/Shared/Exceptions/NegativeMoneyException.cs`
- [ ] TDD: `MoneyTests` — sıfır, toplama/çıkarma, negatif red, eşitlik (value semantics), yuvarlama (2 ondalık)

**Domain event altyapısı (genelleştirme):**
- [ ] `Domain/Shared/IDomainEvent.cs` zaten var → `OccurredOn` ekle (yoksa)
- [ ] `Application/Abstractions/Events/IDomainEventDispatcher.cs` (port)
- [ ] `Infrastructure/Events/MediatRDomainEventDispatcher.cs` — aggregate'ların `DomainEvents`'ini MediatR `INotification` olarak publish eder, sonra `ClearDomainEvents()`
- [ ] `EfUnitOfWork.SaveChangesAsync` → SaveChanges'ten **sonra** tracked aggregate'ların event'lerini topla + dispatch (transactional outbox W6'ya bırakılır; şimdilik in-process)
- [ ] TDD: dispatcher event'i doğru notification'a map ediyor + sonra temizliyor

**Commit:** `feat(domain): money value object + in-process domain event dispatch`

---

## Gün 2 — Tenancy Domain (TDD)

**Hedef:** `ApartmentAssignment` aggregate'ı, sahip/kiracı + tarihçe, invariant'lar tam.

**Domain — `Domain/Tenancy/`:**
- [ ] `TenantType.cs` enum — `Owner` (malik) / `Tenant` (kiracı)
- [ ] `ValueObjects/AssignmentPeriod.cs` — `StartDate` + nullable `EndDate`; aktif mi (`IsActive`), tarih aralığı geçerliliği (`EndDate >= StartDate`)
- [ ] `ApartmentAssignment.cs : AggregateRoot<Guid>`:
  - Shadow FK: `ApartmentId`, `ResidentId` (Property/Residency aggregate'larına id ile referans — cross-aggregate object reference yok, W2 deseni)
  - `TenantType`, `AssignmentPeriod`
  - Factory `Assign(apartmentId, residentId, type, startDate)` → `Empty` daireye atama; domain event `ResidentAssignedToApartment` yayar
  - `End(endDate)` → assignment'ı kapatır (taşınma); event `ResidentMovedOut` yayar
  - Invariant: bitmiş assignment tekrar kapatılamaz
- [ ] `Events/ResidentAssignedToApartment.cs`, `ResidentMovedOut.cs` (`IDomainEvent`)
- [ ] `Exceptions/` — `AssignmentAlreadyEndedException`, `InvalidAssignmentPeriodException`, `TenancyMessageKeys.cs`
- [ ] TDD: `ApartmentAssignmentTests` — atama, kapatma, çift-kapatma reddi, period validasyonu, event yayımı

**Property entegrasyonu (event handler):**
- [ ] `Apartment.MarkAsOccupied()` → **opsiyonel** `MarkAsOccupied(Guid? currentResidentId)` overload veya ayrı; karar Gün 2'de (occupancy'nin sakini bilmesi gerekli mi, yoksa sadece bayrak mı?) → öneri: occupancy sadece bayrak kalsın, "kim oturuyor" sorusu Tenancy query'sinden gelsin (separation of concerns)

**Commit:** `feat(domain): tenancy bounded context — apartment assignment with history (TDD)`

---

## Gün 3 — Billing Domain (TDD) — DuesPeriod + UtilityBillPeriod

**Hedef:** İki ayrı aggregate, aylık dönem + kalem (item) yapısı, ödeme durumu.

**Domain — `Domain/Billing/`:**
- [ ] `ValueObjects/BillingMonth.cs` — yıl+ay (örn. 2026-05); geçmiş/gelecek kuralları; `ToString` "2026-05"
- [ ] `BillingItemStatus.cs` enum — `Unpaid` / `Paid` (W4 Payment bunu `Paid`'e çevirecek)
- [ ] `UtilityType.cs` enum — `Electricity` / `Water` / `NaturalGas`
- [ ] **`DuesPeriod.cs : AggregateRoot<Guid>`** (aidat dönemi):
  - `SiteId` (shadow FK), `BillingMonth`, `Money PerApartmentAmount`, `IsClosed`
  - `private List<DuesItem> _items` → `IReadOnlyCollection<DuesItem> Items`
  - `DuesItem : Entity<Guid>` — `ApartmentId`, `ResidentId`, `Money Amount`, `BillingItemStatus`
  - Factory `Open(siteId, month, perApartmentAmount)`; `AddItemFor(apartmentId, residentId)` (dolu daire başına); `Close()` (dağıtım bitince kilitler); `MarkItemPaid(itemId)`
  - Invariant: kapalı döneme item eklenemez; aynı daire için çift item olamaz; kapalı dönem tekrar kapatılamaz
  - Event: `DuesPeriodClosed` (→ notification maili)
- [ ] **`UtilityBillPeriod.cs : AggregateRoot<Guid>`** (fatura dönemi):
  - `SiteId`, `BillingMonth`, `UtilityType`, `Money TotalAmount`, `IsClosed`
  - `UtilityBillItem : Entity<Guid>` — `ApartmentId`, `ResidentId`, `Money Amount`, `BillingItemStatus`
  - Factory `Open(siteId, month, utilityType, totalAmount)`; `DistributeEqually(apartments[])` (toplam tutarı dolu daire sayısına böl — kuruş yuvarlama artığı son daireye); `Close()`; `MarkItemPaid`
  - Event: `UtilityBillPeriodClosed`
- [ ] `Exceptions/` — `PeriodAlreadyClosedException`, `DuplicateBillingItemException`, `EmptyDistributionException` (dolu daire yokken dağıtım), `BillingMessageKeys.cs`
- [ ] TDD: `DuesPeriodTests`, `UtilityBillPeriodTests` — açma, item ekleme, eşit dağıtım + kuruş artığı, çift-item reddi, kapalı dönem reddi, ödeme işaretleme, event yayımı

**Commit:** `feat(domain): billing bounded context — dues + utility bill periods (TDD)`

---

## Gün 4 — EF Mapping + Migration + Repositories + Queries

**Hedef:** Tenancy + Billing Postgres'e maplenmiş, migration koştu, repo + read-side query'ler hazır.

**Infrastructure:**
- [ ] `Persistence/Configurations/Tenancy/ApartmentAssignmentConfiguration.cs` — shadow FK'lar, `AssignmentPeriod` complex/owned, xmin concurrency token (W2 deseni); aktif assignment için **partial unique index** (`ApartmentId WHERE EndDate IS NULL`)
- [ ] `Persistence/Configurations/Billing/DuesPeriodConfiguration.cs` + `DuesItem` OwnsMany; `Money` owned conversion (Amount decimal(18,2))
- [ ] `Persistence/Configurations/Billing/UtilityBillPeriodConfiguration.cs` + `UtilityBillItem` OwnsMany
- [ ] `SchemaConstants` güncelle (tablo/FK/owned key isimleri — magic string yok)
- [ ] Repositories: `IApartmentAssignmentRepository`, `IDuesPeriodRepository`, `IUtilityBillPeriodRepository` (`IRepository<TRoot>` türevleri) + EF impl
- [ ] Read-side query port'ları: `ITenancyQueries` (daire → aktif sakin, sakin → daireleri), `IBillingQueries` (dönem listesi, sakin borçları, site borç özeti) + EF impl (DTO projeksiyon, W2 deseni)
- [ ] `dotnet ef migrations add TenancyAndBilling` + design-time factory ile
- [ ] TDD/Integration: repo round-trip + partial unique index ihlali → conflict (E2E altyapısında)

**Commit:** `feat(infra): ef mapping + migration + repositories for tenancy & billing`

---

## Gün 5 — Application Handlers + API Endpoints

**Hedef:** Komut/sorgu handler'ları + thin controller'lar; toplu dağıtım + event→mail çalışıyor.

**Application — Tenancy:**
- [ ] `AssignResidentCommand` (apartmentId, residentId, type, startDate) → assignment oluştur (transactional); event handler `Apartment.MarkAsOccupied` tetikler
- [ ] `EndAssignmentCommand` (taşınma) → daire `Empty`'e döner (event handler)
- [ ] FluentValidation kuralları (reusable `CommonValidationRules` genişlet — id, tarih, enum)

**Application — Billing:**
- [ ] `OpenDuesPeriodCommand` (siteId, month, perApartmentAmount)
- [ ] `DistributeDuesCommand` (duesPeriodId) → site'ın dolu dairelerine (Tenancy query'sinden) item üret (bulk)
- [ ] `OpenUtilityBillPeriodCommand` + `DistributeUtilityBillCommand` (eşit dağıtım)
- [ ] `CloseDuesPeriodCommand` / `CloseUtilityBillPeriodCommand` → event → **MailHog notification** (her sakine "X ayı aidatınız/faturanız: Y TRY")
- [ ] Query'ler: `ListDuesPeriodsQuery`, `GetSiteDebtSummaryQuery`, `ListResidentBillsQuery`

**Domain events → notification:**
- [ ] `DuesPeriodClosed` / `UtilityBillPeriodClosed` notification handler'ları → `IEmailSender` ile sakinlere mail (W2'deki `RecordingEmailSender` E2E'de yakalar)

**Api — thin controllers:**
- [ ] `AssignmentsController` — POST assign, POST `{id}/end`
- [ ] `DuesController` — POST open, POST `{id}/distribute`, POST `{id}/close`, GET list
- [ ] `UtilityBillsController` — paralel
- [ ] `[Authorize(Roles = Admin)]`; ProblemDetails (W2 mimarisi otomatik)

**Commit:** `feat(app+api): tenancy assignment + billing distribution commands and endpoints`

---

## Gün 6 — Angular: Tenancy + Billing Admin Sayfaları

**Hedef:** Admin tarayıcıdan sakin atayıp dönem açıp dağıtabiliyor.

- [ ] `npm run gen:api` — yeni endpoint'ler için tipleri yenile
- [ ] `features/tenancy/` — site detayındaki daireye "Sakin Ata" dialog (sakin seç + tip + tarih); "Taşındı/Boşalt" aksiyonu. **W2'deki "daireyi dolu işaretle ama sakin atayamıyorum" sorununu çözer** — occupy artık atama ile gelir
- [ ] `features/billing/` — site detayında "Aidat Dönemi" + "Fatura Dönemi" sekmeleri:
  - Dönem aç dialog (ay + tutar / fatura tipi + toplam)
  - "Dağıt" butonu → item tablosu (daire, sakin, tutar, durum)
  - "Kapat" → onay + mail bilgilendirmesi
  - Site borç özeti kartı (toplam tahakkuk / tahsil edilmemiş)
- [ ] signal store + api katmanı (W2 `SitesStore` deseni)
- [ ] i18n: `tenancy.*` + `billing.*` (tr + en)

**Commit:** `feat(web): tenancy assignment + billing distribution admin pages`

---

## Gün 7 — E2E + Self-Review + Doc

**Hedef:** Full akış testli, kapanış.

- [ ] **E2E test** (`TenancyFlowTests`, `BillingFlowTests`):
  - Ata → daire `Occupied` doğrula (event çalıştı)
  - Aidat dönemi aç → dağıt → item sayısı = dolu daire sayısı → kapat → MailHog'da mail sayısı = sakin sayısı
  - Fatura dönemi: eşit dağıtım + kuruş artığı doğrula
  - **Failure path:** kapalı döneme dağıtım → 409; dolu daire yokken dağıtım → 400/409; çift atama → 409
- [ ] **Manuel uçtan uca**: admin → site → sakin → **ata** → ay aç → dağıt → kapat → MailHog'da bildirim
- [ ] **Self-review** (W2 deseni 6 soru + billing-özel): Money negatif/yuvarlama testli mi? Event'ler dispatch ediliyor mu? Aggregate sınırları korunuyor mu (cross-aggregate object reference yok)?
- [ ] Architecture tests güncelle (Tenancy/Billing katman + CQRS konvansiyonları)
- [ ] `WEEK-3-DETAIL.md` işaretle + ROADMAP W3 ✅

**Commit:** `test(e2e): tenancy + billing full flow + failure paths` + `docs: close out W3`

---

## Hafta 3 Çıktısı (Definition of Done)

- [ ] Admin sakini daireye atayabiliyor → daire otomatik `Occupied` (event-driven)
- [ ] Admin bir ayın aidat + fatura dönemini açıp dolu dairelere toplu dağıtabiliyor
- [ ] Dönem kapatınca her sakine MailHog'a bildirim düşüyor
- [ ] Sakin borçları + site borç özeti query'leri çalışıyor
- [ ] Money VO + domain event dispatch altyapısı yerinde (W4 Payment bunları kullanacak)
- [ ] Domain coverage %80+ korunuyor; tüm testler (Domain + App + E2E + Architecture) yeşil
- [ ] **W4 hazırlığı:** `BillingItem.Status` (`Unpaid`) + `MarkItemPaid` → W4'te PaymentService ödeme yapınca `Paid`'e çevrilecek bağlantı noktası hazır

### Riskler / kapsam kontrolü
- **Scope creep:** Tip-bazlı değişken aidat, gecikme faizi, kısmi ödeme → **W3 dışı** (W6 nice-to-have). W3 sadece "sabit aidat + eşit fatura dağıtımı".
- **Event dispatch karmaşası:** In-process yeterli; transactional outbox W6'ya. Test edilebilirlik için dispatcher port arkasında.
- **Tenancy ↔ Property coupling:** Sadece event + id referansı; object reference yok (architecture test bunu zorlar).
