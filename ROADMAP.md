# SiteManagement — Vitrine Projesi Roadmap

> Patika bitirme projesinin DDD/TDD ile modern bir yorumu. Amaç: CV'de gösterilebilecek, deploy edilmiş, production-grade görünümlü bir .NET full-stack uygulama.

---

## 1. Proje Özeti

İki rollü site (apartman kompleksi) yönetim sistemi:

- **Admin**: daire/blok yönetimi, sakin kaydı, aylık aidat ve fatura (elektrik/su/doğalgaz) dağıtımı, gelen ödemeleri ve mesajları takip, borç-alacak raporu.
- **Resident (Sakin)**: kendi borç/faturalarını görme, kredi kartı ile ödeme, admin'e mesaj.
- **Payment Microservice**: ayrı .NET WebApi + MongoDB, fake banka simülasyonu, sadece kredi kartı.

---

## 2. Mimari Kararlar

### Yaklaşım
- **Domain-Driven Design — Rich Domain Model** (zorunlu, anemic değil): entity'ler kendi behavior'larına sahip, invariant'ları aggregate root'un içinde, business logic service'e taşmıyor.
- **Clean Architecture** — Domain → Application → Infrastructure → Api katmanları, bağımlılık tek yönlü (içe doğru).
- **Modüler Monolit** — ana API'de bounded context'ler folder/project bazında ayrı, ileride microservice'e çıkartılabilir.
- **Payment Service ayrı microservice** — PDF gereği + CV vitrine değeri (polyglot persistence, inter-service communication, resilience pattern'leri göstermek için).
- **CQRS-lite** — MediatR ile command/query ayrımı; event sourcing ve ayrı read DB yok.
- **TDD Domain'de** — invariant'lar ve domain service'ler önce test yazılır. Application handler'ları için de unit test yazılır ama TDD zorunlu değil. E2E için TDD anlamsız.
- **Exception-based hata yönetimi + katman translation** — Domain exception'lar Application katmanında yakalanır ve Application exception'ına translate edilir; API katmanı sadece ApplicationException görür.
- **Localization first-class** — sistem baştan iki dilli (tr-TR default + en-US), hata mesajları dahil her şey resource bundle'larda.

### Rich Domain Prensipleri (anti-anemic)

- Setter'lar `private` veya `protected` — state mutation sadece method üzerinden.
- Method isimleri **ubiquitous language**'tan: `apartment.MarkAsOccupied(resident)`, `duesPeriod.Close()`, `invoice.Pay(amount)`, `conversation.MarkMessageAsRead(messageId, byUserId)`.
- Invariant'lar aggregate'in kendi içinde, ctor + factory method + state change method'larında doğrulanır.
- Value object'ler immutable, validation ctor'da, equality by value.
- Domain event'ler aggregate'ın içinden `AddDomainEvent(...)` ile emit edilir, dış handler'lar dinler.
- Domain service'ler **sadece** birden fazla aggregate'i kapsayan logic için — tek aggregate'lik logic aggregate'e ait.

### Bounded Context Haritası

| Context | Aggregate'lar | Sorumluluk |
|---|---|---|
| Identity | AppUser | Auth, role, password |
| Property | Site (Block, Apartment iç), Apartment | Daire/blok/site yönetimi |
| Residency | Resident | Sakinlerin kişisel bilgileri |
| Tenancy | ApartmentAssignment | Daire-sakin ilişkisi (sahip/kiracı), tarihçeli |
| Billing | **DuesPeriod, UtilityBillPeriod (Yaklaşım A — ayrı aggregate'lar)** | Aylık aidat ve fatura |
| Messaging | Conversation, Message | Admin-sakin mesajlaşma |
| Payment (ayrı servis) | BankAccount, CreditCard, PaymentTransaction | Fake banka, kart, ödeme |

### Exception Architecture

```
Domain Layer
├── DomainException (base, abstract)
│   ├── DuplicateApartmentNumberException
│   ├── ApartmentAlreadyOccupiedException
│   ├── InvalidTcNoException
│   ├── InsufficientBalanceException  (Payment domain)
│   └── ... (her bounded context kendi alt class'larını ekler)
│
Application Layer
├── ApplicationException (base, abstract)
│   ├── BusinessRuleViolationException   (genelde DomainException'dan translate)
│   ├── EntityNotFoundException
│   ├── UnauthorizedActionException
│   ├── PaymentRejectedException
│   └── ValidationException              (FluentValidation'dan)
│
API Layer
└── GlobalExceptionMiddleware
    └── ApplicationException → RFC 7807 Problem Details + HTTP status code
        (DomainException buraya GELMEZ — Application'da yakalanıp çevrilir)
```

**Kural**: Handler'lar try/catch YAZMAZ. MediatR `IPipelineBehavior<,>` ile merkezi translation yapılır:

```csharp
// Application/Behaviors/ExceptionTranslationBehavior.cs
public class ExceptionTranslationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IStringLocalizer<ErrorMessages> _localizer;

    public ExceptionTranslationBehavior(IStringLocalizer<ErrorMessages> localizer)
        => _localizer = localizer;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (DomainException ex)
        {
            // Tek noktada DomainException → ApplicationException translation
            throw ex switch
            {
                DuplicateApartmentNumberException
                    => new BusinessRuleViolationException(_localizer[ex.MessageKey], ex),
                InvalidTcNoException
                    => new BusinessRuleViolationException(_localizer[ex.MessageKey], ex),
                ApartmentAlreadyOccupiedException
                    => new BusinessRuleViolationException(_localizer[ex.MessageKey], ex),
                _ => new BusinessRuleViolationException(_localizer[ex.MessageKey], ex)
            };
        }
    }
}

// Handler — tertemiz, try/catch yok
public class MarkApartmentOccupiedHandler : IRequestHandler<MarkApartmentOccupiedCommand>
{
    public async Task Handle(MarkApartmentOccupiedCommand cmd, CancellationToken ct)
    {
        var apartment = await _repo.GetByIdAsync(cmd.ApartmentId)
            ?? throw new EntityNotFoundException(nameof(Apartment), cmd.ApartmentId);

        apartment.MarkAsOccupied(cmd.ResidentId);  // DomainException atabilir → pipeline halleder
        await _uow.SaveChangesAsync(ct);
    }
}

// DI registration
services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(ExceptionTranslationBehavior<,>).Assembly);
    cfg.AddOpenBehavior(typeof(ExceptionTranslationBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));  // FluentValidation için
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));     // request/response log
});
```

### Patterns (uygulanacak)
- Repository per aggregate (generic repo YOK)
- Unit of Work (EF Core DbContext)
- Specification (eğer karmaşık query'ler oluşursa)
- Strategy (BillType bazlı tutar hesaplama)
- Factory (aggregate yaratımı için static factory method'lar)
- Domain Event + Handler (MediatR INotification)
- **Pipeline Behavior** (MediatR — exception translation, validation, logging tek noktada)
- **Exception Translation** (Domain ↔ Application boundary, ExceptionTranslationBehavior ile)
- Adapter (PaymentService HTTP client → domain interface)
- Idempotency Key (ödeme akışında duplicate prevention)

---

## 3. Tech Stack

| Katman | Seçim |
|---|---|
| Runtime | .NET 10 (LTS) |
| Web Framework | ASP.NET Core 10 |
| ORM | EF Core 10 |
| Main DB | PostgreSQL 16 |
| Payment DB | MongoDB 7 |
| Auth | ASP.NET Core Identity + JWT |
| Validation | FluentValidation |
| CQRS Bus | MediatR |
| HTTP Client | Refit + Polly |
| Logging | Serilog |
| Mail (dev) | MailHog |
| Localization (backend) | IStringLocalizer + .resx |
| Localization (frontend) | ngx-translate |
| Test | xUnit, FluentAssertions, NSubstitute, TestContainers |
| Coverage | Coverlet |
| Frontend | Angular (latest, standalone components) |
| UI Lib | Angular Material veya PrimeNG (W6'da karar) |
| Container | Docker + docker-compose |
| CI | GitHub Actions |
| Deploy | Railway |

---

## 4. Solution Yapısı

```
site-management/
├── docker-compose.yml
├── README.md
├── docs/
│   └── adr/                                # Architecture Decision Records
├── src/
│   ├── SiteManagement.Domain/              # Aggregate'lar, VO'lar, domain event'ler, DomainException
│   │   ├── Property/
│   │   ├── Residency/
│   │   ├── Tenancy/
│   │   ├── Billing/
│   │   ├── Messaging/
│   │   └── Shared/
│   ├── SiteManagement.Application/         # Handler'lar, DTO'lar, validator'lar, ApplicationException
│   │   └── Resources/                      # .resx — translated error messages
│   ├── SiteManagement.Infrastructure/      # EF, repository'ler, identity, external clients
│   └── SiteManagement.Api/                 # Controller'lar, middleware (global exception), DI
├── tests/
│   ├── SiteManagement.Domain.Tests/        # TDD ile yazılan invariant testleri
│   ├── SiteManagement.Application.Tests/   # Handler unit testleri (repo mock'lanmış)
│   └── SiteManagement.E2E.Tests/           # TestContainers + WebApplicationFactory ile uçtan uca
├── payment-service/
│   ├── src/
│   │   ├── PaymentService.Domain/
│   │   ├── PaymentService.Application/
│   │   ├── PaymentService.Infrastructure/  # MongoDB
│   │   └── PaymentService.Api/
│   └── tests/
│       ├── PaymentService.Domain.Tests/
│       └── PaymentService.E2E.Tests/
└── web/                                    # Angular projesi
    └── src/
        └── assets/i18n/                    # tr.json, en.json
```

---

## 5. Test Stratejisi (Honeycomb / Testing Trophy)

| Test Tipi | Coverage Hedefi | Araç | Ne Kapsar |
|---|---|---|---|
| **Domain Unit** | %80+ | xUnit + FluentAssertions | Aggregate invariant'ları, VO validation, domain service'leri (TDD ile) |
| **Application Unit** | %60+ | xUnit + NSubstitute | Handler logic'i, repository'ler mock'lanmış |
| **E2E** | Critical journey'ler | xUnit + TestContainers + WebApplicationFactory | Register → login → property → tenancy → billing → payment → messaging full akışları |
| **Integration test ayrı katman** | — | — | **YOK**. E2E zaten coverage'ı sağlıyor. |
| **Infrastructure test** | — | — | **Ad-hoc only**. Sadece karmaşık SQL veya custom mapping olursa. |
| **Frontend E2E** (opsiyonel) | 3-4 critical akış | Playwright | W6'da, vakit kalırsa |

Postman'i ad-hoc keşif için kullan (yeni endpoint denerken). Regression yükünü E2E test'lere ver, CI her PR'da koşar.

---

## 6. 6 Haftalık Plan

### Hafta 1 — Foundation & Deploy
**Hedef:** "git clone & docker compose up & live URL'de register/login çalışıyor, mimari karkasları yerinde"

- Solution iskelet (Clean Architecture katmanları)
- docker-compose: Postgres + Mongo + MailHog + API
- ASP.NET Core Identity + JWT + Serilog + Swagger + health
- **Exception Architecture** (Domain/Application/API katmanlarında, middleware ile)
- **Localization Setup** (backend .resx + Angular ngx-translate)
- GitHub Actions CI
- İlk Railway deploy
- README v1

**Definition of Done:** Live URL'de register + login çalışıyor. Exception mimarisi yerinde (yapay test exception ile doğrulandı). Türkçe/İngilizce mesaj geçişi çalışıyor. CI yeşil.

---

### Hafta 2 — Property & Residency
**Hedef:** "Admin login olup site/blok/daire/sakin oluşturabiliyor"

- Property Domain (TDD) — Site/Block/Apartment aggregate'ları (rich domain)
- Residency Domain (TDD) — Resident + value object'ler (TcNo, Email, Phone, VehicleInfo)
- EF mapping + ilk migration + repository implementations
- Application — MediatR handler'ları + FluentValidation + exception translation
- API + Angular admin sayfaları (login, layout, CRUD ekranları)
- E2E test altyapısı (TestContainers + WebApplicationFactory) + ilk akış

**Definition of Done:** Admin Angular UI'dan property + residency işlemlerini yapabiliyor, E2E test geçiyor, domain unit coverage %80+.

---

### Hafta 3 — Tenancy & Billing Domain ✅
**Hedef:** "Admin sakin atayıp aylık aidat/fatura'ları toplu dağıtabiliyor"

- Tenancy aggregate (TDD) — sahip/kiracı, tarihçe
- DuesPeriod aggregate (TDD) — aidat dönemi
- UtilityBillPeriod aggregate (TDD) — fatura dönemi (Yaklaşım A: ayrı aggregate)
- Bulk distribution command'ları
- Domain events + MailHog notification
- API + Angular sayfaları
- E2E test: ata → ay başlat → toplu dağıt → mail kontrolü

**Definition of Done:** Bir admin sıfırdan başlayıp bir ayı tamamen kurabiliyor; mailler MailHog'a düşüyor; E2E geçiyor.

---

### Hafta 4 — Payment Microservice ✅
**Hedef:** "Resident borcunu görüp kartla ödeyebiliyor"

- ✅ PaymentService solution iskelet + Mongo (ayrı `PaymentService.slnx`, `payment-api:8090`)
- ✅ PaymentService Domain (TDD) — rich aggregate'lar (BankAccount, CreditCard, PaymentTransaction); fake banka (Luhn/expiry/balance), idempotency unique index
- ✅ PaymentService API endpoint'i (`POST /api/payments`, API-key korumalı)
- ✅ Ana API → PaymentService Refit + Polly (`AddStandardResilienceHandler`) entegrasyonu; `IPaymentGateway` port + `PaymentGatewayAdapter` ACL
- ✅ Pay-by-card (dues + utility item) — charge first, başarılıysa `Paid`; decline → **402**, item `Unpaid`
- ✅ Angular kart ödeme dialog'u + belirgin hata snackbar'ı
- ✅ İki-katmanlı E2E: PaymentService gerçek Mongo+HTTP üzerinde; ana API pay-by-card WireMock stub ile (consumer contract)
- ✅ **Ekstra (W4 sonrası):** overpayment → resident **credit balance** (`ResidentCreditAccount`), dönem tutarı düzeltince otomatik kredi + sonraki faturada otomatik mahsup

**Definition of Done:** ✅ Kartla ödeme → status `Paid`; transaction MongoDB'de; E2E + failure-path (yetersiz bakiye / red kart / idempotent / gateway-down) yeşil; her iki solution'ın testleri yeşil.

> **W5 köprüsü:** PDF'in "resident login → kendi borcunu öder" yarısı (resident portal + IDOR koruması) **Hafta 5'e** taşındı; W4'te akış **admin** tarafından tetikleniyor (tüm pay endpoint'leri şimdilik admin-only).

---

### Hafta 5 — Resident Portal + Messaging & Reports ✅
**Hedef:** "Tüm functional requirement'lar tamam"

- ✅ **Resident portal** — sakin login → "borçlarım" → **kendi** kalemini kartla öder; **IDOR korumalı** (token-scoped `/api/me/*` + iki resource-ownership pipeline behavior, her iki yön E2E)
- ✅ **Authorization pipeline** — rol marker'ları (`IAdminRequest`/`IResidentRequest`/`IPublicRequest`) + `AuthorizationBehavior`; "her request authz deklare etmeli" arch guardrail'i; handler'larda authz kodu yok
- ✅ Messaging Domain (TDD) — `Conversation` aggregate
- ✅ Messaging API (admin `/api/conversations` + resident `/api/me/conversations`) + per-side unread; **Angular resident messaging UI** (admin UI ertelendi)
- ✅ Admin dashboard (site/sakin sayısı, tahakkuk/tahsil, açık bakiye, kredi, tahsilat oranı) + resident dashboard (açık borç + kredi + okunmamış mesaj)
- ✅ **Hijyen:** E2E↔compose DB izolasyonu + PaymentService architecture testleri

**Definition of Done:** ✅ Sakin kendi dashboard'ı + borç ödeme + mesajlaşma; admin dashboard + mesajlaşma (backend). Tüm testler yeşil (Domain 222, App 83, Arch 18, E2E 34; PaymentService 54; web 25).

> **W6'ya devreden:** admin messaging Angular sayfası (backend hazır), credit partial settlement, SignalR/real-time, deploy + polish.

---

### Hafta 6 — Polish & Ship
**Hedef:** "Vitrine'e koyulabilir hale gelmiş"

- Test coverage doldurma (domain %80+, app %60+, E2E critical journey'ler tam)
- README v2 (diyagramlar, ADR'lar)
- UI polish + performance + security
- Final deploy + smoke test
- LinkedIn postu + demo video

**Definition of Done:** Repo public, README prezentabl, deployed URL canlı, demo video kayıtlı, LinkedIn postu hazır.

---

## 7. Genel Definition of Done

Her task için:
- Domain test'leri yazıldı ve geçiyor (yeni domain logic varsa)
- E2E test eklendi (yeni endpoint varsa ve critical journey'in parçasıysa)
- Swagger'da endpoint görünüyor
- Mesajlar localize edilebilir (hardcoded Türkçe string yok)
- Exception'lar doğru katmanda fırlatılıyor, doğru katmanda yakalanıyor
- Local'de docker compose up ile çalışıyor
- PR review edildi (Mehmet tarafından, özellikle rich domain + exception translation için)
- CI yeşil
- Main'e merge edildi

---

## 8. Bilinen Riskler & Watch List

| Risk | Etki | Mitigation |
|---|---|---|
| DDD perfectionism (aggregate refactor'a saplanmak) | Süre kayması | Her hafta deploy edilebilir bir şey çıkar |
| Anemic model'e kayma | Mimari hedef kaçar | Code review checklist: setter public mi? Logic service'te mi? |
| Angular learning curve | Frontend yavaşlar | UI library + Material starter şablonlar + Claude'a daha çok güven |
| Payment microservice scope creep | Hafta 4 sarkar | Sadece PDF'in istediği kadar — fake bank, fake card, başka şey yok |
| Exception translation unutulur | Domain leak | Application.Tests'te explicit test: "domain method exception fırlatınca handler'dan ApplicationException döner" |
| Localization sonradan yamanır | Refactor cehennemi | W1'de tüm altyapı yerinde, ilk gün hardcoded string yazma |
| Tatil sonrası momentum kaybı | 9 günlük tatilde ara | Tatilin son 2 gününü "ısınma" olarak tut, ufak fix'ler yap |

---

## 9. Kararlar Günlüğü (ADR Index — W6'da `docs/adr/` altına detaylanacak)

1. **DDD + Clean Architecture** — CV vitrine + mülakat hazırlığı
2. **Rich Domain (not anemic)** — ubiquitous language, encapsulation, domain integrity
3. **Modüler Monolit** — microservice premature optimization; ama Payment ayrı (PDF zorunlu + CV değeri)
4. **PostgreSQL** — PDF SQL Server diyor ama açık kaynak + ücretsiz cloud tier + modern
5. **Yaklaşım A: DuesPeriod ve UtilityBillPeriod ayrı aggregate'lar** — PDF'in aidat vs fatura ayrımına sadık
6. **Exception-based hata yönetimi + katman translation** — Result Pattern değil; .NET dünyasında daha yaygın; layer purity korunur
7. **Localization first-class** — sonradan yamanmaz, baştan tasarımda
8. **MailHog (dev)** — ücretsiz SMTP test; prod'da SendGrid/Postmark ileride
9. **Refit + Polly** — HTTP client temizliği + resilience pattern
10. **TDD sadece Domain'de zorunlu** — her şeyi TDD yaparsak süre yetmez, asıl değer Domain'de
11. **Test stratejisi: Domain unit + App unit + E2E** — integration test ayrı katman yok (Honeycomb yaklaşımı)
12. **ngx-translate** (Angular i18n yerine) — runtime dil switch, JSON tabanlı, daha esnek

---

## 10. Tatil Stratejisi (9 günlük bayram)

Tatilden önce: en geç **Hafta 2 sonunu bitir** (Property + Residency deployed).

Tatil içinde (eğer enerjin varsa):
- 2 gün: Hafta 3 (Tenancy + Billing domain — yoğun TDD haftası, sakin ortamda iyi gider)
- 2 gün: Hafta 4 (Payment microservice — yeni teknoloji öğrenme keyfi)
- 1 gün ayırma: gez, dinlen, hiçbir şey yapma

Tatil sonrası: Hafta 5 + 6 normal tempoda.

Eğer tatilde 4-5 gün gerçekten koyabilirsen, toplam süre 6 hafta yerine 4-5 haftaya iner.

---

## 11. Sonraki Adımlar

1. Bu güncel roadmap'i tekrar review et, son itirazların varsa söyle.
2. GitHub'da `site-management` adında repo aç (private başlasın).
3. W1'in ilk task'ı: solution iskeletini kuralım. "Hazırım" dediğinde Task #1'i in_progress'e alıp ilk commit'i atacağız.
