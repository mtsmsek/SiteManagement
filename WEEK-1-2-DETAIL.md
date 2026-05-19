# Hafta 1 & 2 — Gün Gün Detaylı Plan

> Her gün bir hedef + checklist + commit önerisi. Tatile (bayram) kadar buranın bitmesi hedef. Yetişemezse W2'nin son birkaç günü tatil başlangıcına sarkabilir.

---

## HAFTA 1 — Foundation & Deploy

### Gün 1 (Pazartesi) — Repo + Solution İskelet ✅

**Hedef:** GitHub'da temiz başlangıç, `dotnet build` solution hatasız geçiyor.

- [x] GitHub'da `SiteManagement` adında public repo aç (vitrin projesi, private değil) → https://github.com/mtsmsek/SiteManagement
- [x] Local'e clone
- [x] `dotnet new sln -n SiteManagement` _(net10 default'u `.slnx` üretiyor — yeni XML formatı)_
- [x] Class library projeleri:
  - `dotnet new classlib -n SiteManagement.Domain -o src/SiteManagement.Domain`
  - `dotnet new classlib -n SiteManagement.Application -o src/SiteManagement.Application`
  - `dotnet new classlib -n SiteManagement.Infrastructure -o src/SiteManagement.Infrastructure`
- [x] WebApi projesi:
  - `dotnet new webapi -n SiteManagement.Api -o src/SiteManagement.Api --use-controllers`
- [x] Test projeleri:
  - `dotnet new xunit -n SiteManagement.Domain.Tests -o tests/SiteManagement.Domain.Tests`
  - `dotnet new xunit -n SiteManagement.Application.Tests -o tests/SiteManagement.Application.Tests`
  - `dotnet new xunit -n SiteManagement.E2E.Tests -o tests/SiteManagement.E2E.Tests`
- [x] Tüm projeleri solution'a ekle (PowerShell: `dotnet sln add (Get-ChildItem -Recurse -Filter *.csproj | Select-Object -ExpandProperty FullName)`)
- [x] Project reference'lar:
  - Application → Domain
  - Infrastructure → Application, Domain
  - Api → Application, Infrastructure
  - Domain.Tests → Domain
  - Application.Tests → Application
  - E2E.Tests → Api (+ Infrastructure indirect)
- [x] NuGet'ler — Central Package Management (Directory.Packages.props):
  - Application: `MediatR`, `FluentValidation`, `FluentValidation.DependencyInjectionExtensions`, `Microsoft.Extensions.Localization.Abstractions`
  - Infrastructure: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `BCrypt.Net-Next`, `Refit`, `Refit.HttpClientFactory`, `Microsoft.Extensions.Http.Resilience`
  - Api: `Serilog.AspNetCore` + `Serilog.Sinks.{Console,File}`, `Microsoft.AspNetCore.Authentication.JwtBearer`, **`Scalar.AspNetCore`** (Swashbuckle yerine — .NET 10 + Microsoft.OpenApi 2.0 ile uyumsuzluk), `AspNetCore.HealthChecks.NpgSql`
  - Test'ler: `FluentAssertions` 6.12.2 (8.x ticari lisansa geçti), `NSubstitute`, `Testcontainers.PostgreSql`, `Microsoft.AspNetCore.Mvc.Testing`
- [x] Repo-scope `NuGet.config` — sadece `nuget.org` (deterministik build, başkası clone edince çalışsın)
- [x] `Directory.Packages.props` — Central Package Management aktif
- [x] `TreatWarningsAsErrors=true` tüm src projelerinde (production-grade discipline)
- [x] `.gitignore` (.NET için standart + Node/Angular hazır)
- [x] `LICENSE` (MIT)
- [x] `README.md` placeholder
- [x] `dotnet build` solution hatasız geçiyor (0 uyarı / 0 hata)
- [x] `git add .`, commit, push

**Commit:** `chore: initial solution skeleton with clean architecture layers` → `2cc4a58`

---

### Gün 2 (Salı) — Docker Compose ✅

**Hedef:** `docker compose up` ile Postgres + Mongo + MailHog + API hep birlikte ayağa kalkıyor.

- [x] Solution root'una `docker-compose.yml`:
  - `postgres:16-alpine` (env: POSTGRES_USER, POSTGRES_PASSWORD, POSTGRES_DB; port 5432; volume: postgres-data; **healthcheck: pg_isready**)
  - `mongo:7` (env: MONGO_INITDB_ROOT_USERNAME, MONGO_INITDB_ROOT_PASSWORD; port 27017; volume: mongo-data; **healthcheck: db.adminCommand('ping')**)
  - `mailhog:latest` (port 1025 SMTP, 8025 UI)
  - `api` (build: kök context + `src/SiteManagement.Api/Dockerfile`; depends_on: postgres healthy; ports: 8080:8080; **healthcheck: curl /health**)
- [x] `.env.example` (Postgres, Mongo, MailHog, API, JWT placeholder credentials)
- [x] `.env` gitignore'da (`.env` gitignored, `.env.example` istisna)
- [x] API için multi-stage `Dockerfile` (sdk:10.0 → publish → aspnet:10.0; non-root `$APP_UID`; layer-cache için önce csproj sonra src kopyalama; healthcheck için curl kurulu)
- [x] `appsettings.Development.json`'da default connection string + Smtp config; compose env var'ları (`ConnectionStrings__DefaultConnection`, `Smtp__Host`, `Jwt__*`) override eder
- [x] `docker compose up --build -d` çalıştır → tüm container'lar `(healthy)`
- [x] `docker compose ps` ile tüm container'ların `running` olduğunu doğrula
- [x] MailHog UI: http://localhost:8025 → 200
- [x] `/health` smoke endpoint: http://localhost:8080/health → `{"status":"ok"}`
- [x] Scalar API docs UI: http://localhost:8080/scalar/v1 → 200 (Swashbuckle yerine)
- [x] OpenAPI JSON: http://localhost:8080/openapi/v1.json → 200
- [ ] _(Opsiyonel, manuel)_ Postgres'e DBeaver/pgAdmin ile bağlan (test connection)
- [ ] _(Opsiyonel, manuel)_ Mongo'ya Compass ile bağlan
- [x] README'ye "Local Setup" bölümü ekle
- [x] PC kurulum talimatları: [docs/SETUP-MACHINE.md](docs/SETUP-MACHINE.md)

**Commit:** `chore: docker-compose with postgres, mongo, mailhog and api service` + `fix: replace Swashbuckle with Scalar for .NET 10 / OpenApi 2.0 compatibility`

---

### Gün 3 (Çarşamba) — ASP.NET Core Identity + EF Core ✅

**Hedef:** Migration koştu, Identity tabloları Postgres'te oluştu, roller seed'lendi.

- [x] `Infrastructure/Persistence/AppDbContext.cs` → `IdentityDbContext<AppUser, AppRole, Guid>` (tablo isimleri `Users`, `Roles`, `UserRoles`, ... — AspNet* prefix'i temizlendi)
- [x] `Infrastructure/Identity/AppUser.cs : IdentityUser<Guid>` + `FullName` alanı
- [x] `Infrastructure/Identity/AppRole.cs : IdentityRole<Guid>` + name-aware ctor
- [x] `Infrastructure/Persistence/Seed/IdentitySeeder.cs`: Admin + Resident rollerini sabit Guid'lerle (Domain'deki `Roles.AdminId`/`ResidentId`) seed eder
- [x] `Infrastructure/DependencyInjection.cs` → `AddInfrastructure(IConfiguration)`:
  - `AddDbContext<AppDbContext>` + Npgsql + migrations assembly
  - `AddIdentity<AppUser, AppRole>` + password policy (sabit'ler `IdentityPasswordPolicy`)
  - `AddEntityFrameworkStores<AppDbContext>` + `AddDefaultTokenProviders`
- [x] `Infrastructure/Persistence/AppDbContextFactory.cs` (`IDesignTimeDbContextFactory`) — EF tool API'yi boot'lamadan migration ekleyebilsin
- [x] `dotnet ef migrations add Initial --project src/SiteManagement.Infrastructure --startup-project src/SiteManagement.Api --output-dir Persistence/Migrations` çalıştırıldı
- [x] `Api/Configuration/DatabaseInitializer.cs` → `MigrateAndSeedAsync` startup'ta otomatik koşar (Program.cs içinde `await DatabaseInitializer.MigrateAndSeedAsync(app.Services)`)
- [x] `dotnet ef` tool 10.0.8'e güncellendi (8.0.x net10 ile uyumsuzdu)
- [x] _(compose smoke'da)_ Postgres'te `Users`, `Roles` ve diğer Identity tablolarının oluştuğunu doğrula

**Commit:** `feat: aspnet identity with postgres, role seeding`

---

### Gün 4 (Perşembe) — JWT + Auth Endpoints + Serilog + (Scalar) ✅

**Hedef:** API'de register + login + refresh çalışıyor, Scalar UI'da Bearer auth görünüyor.

- [x] `Api/Controllers/Auth/AuthController.cs` — thin controller, MediatR `ISender`'a delege:
  - POST `/api/auth/register` (email, password, fullName → Admin oluşturur)
  - POST `/api/auth/login` (email, password → access + refresh)
  - POST `/api/auth/refresh` (refresh token → yeni access + refresh)
- [x] **CQRS-lite uygulandı**: business logic Application'da `RegisterCommand`/`LoginCommand`/`RefreshTokenCommand` handler'larında, controller sıfır iş yapar
- [x] `Infrastructure/Auth/TokenService.cs : ITokenService` — HS256 JWT + crypto-random refresh token; `TimeProvider` injection (test'lenebilir)
- [x] `Application/Abstractions/Auth/ITokenService.cs` — Application portu, JWT detayı bilmez
- [x] `Application/Abstractions/Auth/IUserAuthService.cs` + `Infrastructure/Auth/UserAuthService.cs` — `UserManager` Application'a sızmıyor
- [x] `Application/Abstractions/Auth/IRefreshTokenStore.cs` + `Infrastructure/Auth/InMemoryRefreshTokenStore.cs` (W2'de EF-backed'a geçilecek)
- [x] `appsettings.json`'a Jwt section (Key, Issuer, Audience, AccessTokenMinutes, RefreshTokenDays)
- [x] `Infrastructure/Auth/AuthExtensions.cs` → `AddJwtAuth(IConfiguration)` (JwtBearer scheme + TokenValidationParameters; auth wiring Api'ye sızmadı)
- [x] **Serilog setup** Api'de `Configuration/LoggingExtensions.cs` → `AddSerilogLogging()`; config `appsettings.json`'daki `Serilog` section'ından okunuyor; `UseSerilogRequestLogging()` pipeline'da
- [x] **API docs UI**: Swashbuckle yerine Scalar (W1 Gün 2 fix'i); `Configuration/OpenApiExtensions.cs` → `AddSiteManagementOpenApi()` Bearer scheme'i OpenAPI dokümanına eklediği için Scalar Authorize panelinde Bearer button'u var
- [x] Health: `app.MapHealthChecks("/health")` + Postgres probe (`AspNetCore.HealthChecks.NpgSql`); ayrı `/health/live` liveness endpoint
- [x] **Lean Program.cs**: 20 satır, sadece extension method çağrıları. Inline wireup yok.
- [x] _(compose smoke'da)_ register → login → token al → /health Bearer ile çağır → 200

**Commit:** `feat: jwt auth (register/login/refresh) + serilog + scalar bearer + health-with-db`

---

### Gün 5 (Cuma) — Exception Architecture ✅

**Hedef:** Domain ↔ Application ↔ Api exception katmanları doğru çalışıyor, test'lerle doğrulanmış.

- [x] `Domain/Shared/Exceptions/DomainException.cs` (abstract; ctor: `messageKey`, `params object[] args`)
- [x] `Application/Shared/Exceptions/ApplicationException.cs` (abstract; `StatusCode` taşır)
- [x] HTTP status code sabitleri `Application/Shared/Exceptions/HttpStatus.cs` (Application'da `Microsoft.AspNetCore.Http` reference'ı yok)
- [x] Alt class'lar:
  - `BusinessRuleViolationException` (409, `MessageKey` ile birlikte)
  - `EntityNotFoundException` (404)
  - `UnauthorizedActionException` (403)
  - `AuthenticationException` (401)
  - `ValidationException` (400, FluentValidation `ValidationFailure`'dan `errors` dict üretir)
- [x] `Application/Behaviors/ExceptionTranslationBehavior.cs` (MediatR `IPipelineBehavior<,>`): handler içinde fırlatılan `DomainException` → `IStringLocalizer<ErrorMessages>` ile lokalize → `BusinessRuleViolationException` olarak rethrow
- [x] `Application/Behaviors/ValidationBehavior.cs` (FluentValidation pipeline) — pipeline failures'ı `ValidationException` olarak fırlatır
- [x] `Application/Behaviors/LoggingBehavior.cs` (structured request/response log + elapsed ms)
- [x] DI'a behavior'ları doğru sırayla register: Logging (outer) → Validation → ExceptionTranslation (innermost)
- [x] `Api/Middleware/GlobalExceptionMiddleware.cs`: `ApplicationException` → RFC 7807 ProblemDetails JSON (`application/problem+json`) + uygun HTTP status. `ValidationException.Errors` → `errors` extension. `BusinessRuleViolationException.MessageKey` → `messageKey` extension. Domain exception'lar pipeline'da çevrildiği için buraya HİÇ gelmez.
- [x] **Magic literal yok**: `ProblemDetailsErrorsKey`, `ProblemDetailsMessageKey`, `ProblemDetailsTraceIdKey` sabit; `ValidationMessages` ve `ValidationLimits` Application'da; `CommonValidationRules` extension'ları (`ValidEmailAddress`, `ValidPassword`, `ValidFullName`, `RequiredText`) tekrarı engelliyor
- [x] Unit test (`Application.Tests/Behaviors/ExceptionTranslationBehaviorTests.cs`):
  - Domain exception → `BusinessRuleViolationException` lokalize mesajla
  - Application exception bypass (kendisi)
  - Success path return değiştirmez
- [x] Unit test (`Application.Tests/Auth/LoginCommandHandlerTests.cs`): invalid → 401, valid → tokens issued + refresh stored
- [x] `Doubles/AuthDoubles.cs` — sample factory'ler (`SampleUser`, `SampleTokens`), test'lerde tekrar yok
- [x] AAA pattern + `// arrange/act/assert` yorum + boş satır ayrımı her testte

**Commit:** _(W1 Gün 3-4-5 birleşik commit)_ `feat: jwt auth (register/login/refresh) + serilog + scalar bearer + health-with-db` ya da daha doğru `feat: identity + jwt auth + layered exception architecture (W1 D3-D5)`

---

### Gün 6 (Cumartesi) — Localization (Backend) ✅

**Hedef:** Hata mesajları kültüre göre tr-TR / en-US dönüyor.

- [x] `Application/Shared/Resources/ErrorMessages.{tr,en}.resx` — Auth + Generic + Validation header mesajları (Türkçe / İngilizce)
- [x] `Application/Shared/Validation/ValidationMessages.{tr,en}.resx` — FluentValidation mesajları (`Validation.Required`, `Validation.EmailInvalidFormat`, vs.)
- [x] Resource key sabitleri **tek noktada**:
  - `Application/Shared/Resources/ErrorMessageKeys.cs` — Auth / Error / Validation header key'leri
  - `Application/Shared/Validation/ValidationMessages.cs` — FluentValidation key'leri
- [x] `Application/Shared/Validation/CommonValidationRules.cs` — validator'lar sadece **key** basıyor (`WithMessage(ValidationMessages.Required)`); Application'da `IStringLocalizer` referansı yok
- [x] `Application/Shared/Exceptions/ValidationException.cs` artık `Failures` taşıyor (her bir failure için key + FluentValidation placeholder dict — `{PropertyName}`, `{MaxLength}`)
- [x] `AuthenticationException`, `BusinessRuleViolationException` artık `MessageKey` taşıyor; handler'lar `throw new AuthenticationException(ErrorMessageKeys.AuthInvalidCredentials)` yazıyor
- [x] **Mimari karar — Yaklaşım B**: validator'lar key basar, **GlobalExceptionMiddleware translate eder**. Validator'lar tamamen sade ve framework-clean; localize iş tek noktada. (Yaklaşım A — her validator localizer inject — daha boilerplate'liydi)
- [x] `Api/Configuration/LocalizationExtensions.cs` → `AddSiteManagementLocalization()` — Supported = [tr-TR, en-US], Default = tr-TR
- [x] `Api/Configuration/PipelineExtensions.cs` — `app.UseRequestLocalization(...)` pipeline'ın **en üstünde** (middleware'lardan önce ki culture context'i set olsun)
- [x] `Api/Middleware/GlobalExceptionMiddleware.cs`:
  - `IStringLocalizer<ErrorMessages>` + `IStringLocalizer<ValidationMessages>` inject
  - `AuthenticationException.MessageKey` ve `BusinessRuleViolationException.MessageKey` → localized `detail`
  - `ValidationException.Failures` → her message key localize + `{PropertyName}/{MaxLength}/...` placeholder substitution (compiled regex)
  - Missing key → key'i raw döndürür (developer-friendly fallback)
- [x] Live smoke (Postman/curl):
  - `Accept-Language: en-US` → `"Invalid email or password."`, `"Email is required."`
  - `Accept-Language: tr-TR` → `"E-posta veya şifre hatalı."`, `"Email alanı zorunludur."`
  - Header yok → tr-TR default
  - Unsupported culture (`fr-FR`) → tr-TR fallback
- [ ] _(W2'ye bırakıldı)_ E2E test: `Accept-Language` header smoke (TestContainers altyapısı W2 sonu)
- [ ] _(W2'ye bırakıldı)_ Domain MessageKey sabitleri (Apartment.AlreadyOccupied, TcNo.Invalid, ...) — W2 Property/Residency aggregate'leriyle birlikte gelecek

**Commit:** `feat: backend localization (tr-TR, en-US) with resx + request culture middleware (W1 Day 6)`
  - Header yok → Türkçe
- [ ] FluentValidation'ın validation mesajlarını da lokalize et

**Commit:** `feat: backend localization (tr-TR, en-US) with resx and request culture middleware`

---

### Gün 7 (Pazar) — CI + Deploy + README v1

**Hedef:** GitHub Actions yeşil, Railway'de live URL açılıyor, README sunulabilir.

- [x] `.github/workflows/ci.yml`:
  - Trigger: pull_request, push to main, workflow_dispatch
  - Concurrency group: aynı branch'e gelen yeni push eskisini iptal eder
  - Postgres 16 service container (5432 exposed, healthcheck `pg_isready`)
  - NuGet cache (`actions/cache@v4`, key = csproj + `Directory.Packages.props` hash)
  - Steps: checkout → setup-dotnet@v4 (10.0.x) → restore → build (Release, warnings = errors) → test (TRX + Coverlet code coverage)
  - Test sonuçları artifact olarak yüklenir (`actions/upload-artifact@v4`)
- [x] Railway uyumluluğu — kodda iki extension:
  - `Api/Configuration/PortBindingExtensions.cs` → `$PORT` env varsa `ASPNETCORE_URLS=http://+:$PORT`'a bind
  - `Api/Configuration/DatabaseUrlExtensions.cs` → `postgresql://user:pass@host:port/db` formatlı `DATABASE_URL`'yi Npgsql connection string'ine çevirir (SSL Mode = Require)
  - 3 unit test (`E2E.Tests/Configuration/DatabaseUrlExtensionsTests.cs`) converter logic'i için
- [x] `docs/DEPLOY-RAILWAY.md` — adım adım Railway deploy talimatları (hesap, repo bağlama, PostgreSQL add-on, env variables, domain, smoke test, troubleshooting, maliyet uyarısı)
- [ ] _(W6'ya bırakıldı — bilinçli karar)_ Railway hesabı + ilk deploy → public URL: vitrin polish'iyle (W6) birlikte yapılacak; kart/hesap/maliyet yükünü vitrin son halinde alalım, ara dönemde sürekli kredi yememesi için
- [ ] _(Opsiyonel)_ Main branch protection: PR'da review + status check zorunlu (tek başına çalışıyorsan şimdilik kapalı)
- [x] **Architecture testleri** — yeni proje `SiteManagement.ArchitectureTests` (NetArchTest):
  - **Layer dependency koruması**: Domain BCL-only, Application no-EF / no-AspNetCore / no-Identity, Infrastructure → Api referans yok
  - **CQRS naming**: Her `IRequest<>` `Command`/`Query` ile bitiyor; orphan request yok (her birinin handler'ı var); her command'ın validator'ı var; handler'lar `sealed`
  - **Resource key bütünlüğü**: Her `ErrorMessageKeys` ve `ValidationMessages` const'unun hem tr hem en resx'te karşılığı var; iki resx aynı key set'ine sahip (drift kontrolü)
  - 15 test, hepsi yeşil — proje sağlığının uzun vadeli garantörü
- [x] README v1:
  - Proje özeti + CI badge + License badge + .NET badge
  - "Bugün ne çalışıyor" özet listesi
  - Tech stack tablosu
  - Mimari özeti (katmanlar + akış)
  - Local çalıştırma adımları (clone → .env → docker compose → curl /health → auth smoke)
  - Lokalizasyon smoke örneği (`Accept-Language: en-US`)
  - Production deploy bölümü (Railway pointers)
  - CI bölümü

**Commit:** `chore: github actions ci, railway deploy prep, readme v1 (W1 Day 7)`

---

### Hafta 1 Çıktısı

- Live URL: https://site-management.up.railway.app (örnek)
- Register + login + refresh çalışıyor
- Exception mimarisi yerinde, lokalize hatalar dönüyor
- CI yeşil
- README v1 prezentabl

**Hafta 1 sonu self-review:**
- Domain layer'da framework bağımlılığı var mı? (Olmamalı)
- Application layer'da EF Core / ASP.NET reference var mı? (Olmamalı)
- Handler'lar try/catch içeriyor mu? (Sadece EntityNotFoundException atan tek satırlar olmalı)
- Hardcoded Türkçe string kaldı mı kodda? (Sıfır olmalı)

---

## HAFTA 2 — Property & Residency

### Gün 1-2 (Pazartesi-Salı) — Property Domain (TDD)

**Hedef:** Property bounded context — rich aggregate'lar, invariant test'leri tam, %85+ domain coverage.

- [ ] `Domain/Property/ValueObjects/`:
  - `ApartmentType` (string, format: `[1-9]+\+[0-9]+`, factory `ApartmentType.From("2+1")`)
  - `ApartmentNumber` (int, range 1-999)
  - `Floor` (int, range -5 to 50, negatif = bodrum)
  - `BlockName` (string, max 50 char)
- [x] `Domain/Shared/` — DDD omurgası (W1'de yoktu, W2 başlangıcında oturtuldu):
  - `Entity<TId>` — Id-based equality, transient-id guard, EF-friendly protected ctor
  - `AggregateRoot<TId> : Entity<TId>` — domain event listesi sadece root'ta tutulur
  - `ValueObject` — `GetEqualityComponents()` ile component-based equality
  - `IDomainEvent` — `OccurredOnUtc` taşır; W3-W5'te concrete event'ler eklenecek
- [x] `Domain/Property/Exceptions/`: 11 concrete `DomainException`:
  - `InvalidApartmentTypeException`, `ApartmentNumberOutOfRangeException`, `FloorOutOfRangeException`
  - `InvalidBlockNameException`, `InvalidSiteNameException`
  - `ApartmentAlreadyOccupiedException`, `ApartmentNotOccupiedException`
  - `DuplicateApartmentNumberException`, `ApartmentNotFoundInBlockException`
  - `DuplicateBlockNameException`, `BlockNotFoundInSiteException`
- [x] `Domain/Property/PropertyMessageKeys.cs` — tüm key sabitleri tek noktada (no magic strings)
- [x] `Domain/Property/PropertyLimits.cs` — numeric sabitler (ApartmentNumber 1..999, Floor -5..50, BlockName ≤50, SiteName ≤120) tek noktada
- [x] Localization: `Property.*` MessageKey'leri için tr + en `.resx` entry'leri (placeholder format'ları `{0}/{1}/{2}` ile)
- [x] **TDD — önce test, sonra kod** (4 VO + 3 aggregate test class'ı):
  - `Domain.Tests/Property/ValueObjects/ApartmentTypeTests.cs` — N+M parsing, equality, invalid input cases
  - `ApartmentNumberTests.cs` — range invariant
  - `FloorTests.cs` — signed range, `IsBasement` derived property
  - `BlockNameTests.cs` — trim + length + case-insensitive equality
  - `Property/ApartmentTests.cs` — `Create` default empty, `MarkAsOccupied`/`MarkAsEmpty` transitions, `ChangeType`
  - `BlockTests.cs` — `AddApartment` duplicate-number rejection, `RemoveApartment` not-found, `Apartments` read-only encapsulation
  - `SiteTests.cs` — name validation, `AddBlock` case-insensitive duplicate rejection, `RemoveBlock`, `GetBlock`, `Blocks` read-only encapsulation
- [x] Production: 4 VO (`ApartmentType`, `ApartmentNumber`, `Floor`, `BlockName`), `OccupancyStatus` enum, `Apartment : Entity<Guid>`, `Block : Entity<Guid>`, `Site : AggregateRoot<Guid>`
- [x] `Domain.Tests/Doubles/PropertyDoubles.cs` — paylaşılan factory'ler (`SampleApartment`, `SampleApartmentType`, vs.) — test'lerde tekrar yok
- [x] AAA pattern + `// arrange/act/assert` yorumları her testte
- [x] **Coverage**: Domain.Property %86.5+ (Apartment/Block/Site 100%, VO'lar 100%, Entity/AggregateRoot base'lerinin uncovered branch'leri: domain event'ler henüz raise edilmiyor, W3-W5'te kapanacak)
- [x] Architecture testleri yeşil (Domain hala framework-free, ResourceIntegrity drift yok)
- [x] 92/92 test yeşil (Domain 69, Application 5, E2E 3, Architecture 15)

**Commit:** `feat(domain): property bounded context with rich aggregates + tdd invariants (W2 Day 1-2)`

---

### Gün 3 (Çarşamba) — Residency Domain (TDD)

**Hedef:** Resident aggregate ve value object'ler tamam, TC validation çalışıyor. ✅

- [x] `Domain/Residency/ResidencyLimits.cs` — numeric sabitler (TcNoLength=11, NameComponentMaxLength=60, EmailMaxLength=254, PhoneNumberMaxLength=16, VehicleNoteMaxLength=120) tek noktada
- [x] `Domain/Residency/Exceptions/ResidencyMessageKeys.cs` — 8 stable resource key
- [x] `Domain/Residency/ValueObjects/`:
  - `TcNo` — 11 hane + leading-digit + **gerçek T.C. checksum** algoritması (odd*7-even mod 10, sum mod 10)
  - `FullName.Create(firstName, lastName)` — iki parça, trim + length cap
  - `Email` — `local@domain.tld` + lowercase normalize + length cap (`GeneratedRegex`)
  - `PhoneNumber` — TR formatı (5XXXXXXXXX / 05XXXXXXXXX / +905XXXXXXXXX, noise stripping); canonical `+90XXXXXXXXXX`
  - `PlateNumber` — TR plaka NN[A-Z]{1,3}NNNN, **gerçek il kodu range'i 01..81**, uppercase + whitespace strip
  - `VehicleInfo` — `PlateNumber` (required) + optional note (whitespace → null, length cap)
- [x] `Domain/Residency/Exceptions/` — 8 concrete `DomainException`:
  - `InvalidTcNoException`, `InvalidFullNameException`, `InvalidEmailException`, `InvalidPhoneNumberException`, `InvalidPlateNumberException`
  - `DuplicateVehiclePlateException`, `VehicleNotFoundException`, `VehicleNoteTooLongException`
- [x] Localization: `Residency.*` MessageKey'leri için tr + en `.resx` entry'leri (8 anahtar)
- [x] **TDD — önce test, sonra kod**:
  - `Domain.Tests/Residency/ValueObjects/TcNoTests.cs` — algorithm-valid sample'lar (10000000146, 12345678950), bad-shape vakaları, checksum failure (12345678901)
  - `FullNameTests.cs`, `EmailTests.cs`, `PhoneNumberTests.cs` (5 input form'u → canonical), `PlateNumberTests.cs` (province 01..81 boundary, normalisation), `VehicleInfoTests.cs`
  - `Residency/ResidentTests.cs` — Create, UpdateContactInfo, ChangeFullName, AddVehicle (case-insensitive duplicate plate rejection), RemoveVehicle success+not-found, Vehicles read-only encapsulation
- [x] `Domain/Residency/Resident.cs : AggregateRoot<Guid>`:
  - Id (auto), TcNo (immutable), FullName, Email, Phone, Vehicles (`IReadOnlyCollection<VehicleInfo>`)
  - Factory `Resident.Create(...)`
  - Behaviors: `UpdateContactInfo(email, phone)`, `ChangeFullName(name)`, `AddVehicle(vehicle)`, `RemoveVehicle(plate)`
- [x] `Domain.Tests/Doubles/ResidencyDoubles.cs` — paylaşılan factory'ler (`SampleTc`, `SampleResident`, `SampleVehicle`, ...) — test'lerde tekrar yok
- [x] AAA pattern + `// arrange/act/assert` yorumları her testte
- [x] **Coverage**: Domain assembly **%88.9** (W1 sonu %86.5 → +%2.4 artış)
- [x] Architecture testleri yeşil (Domain framework-free, ResourceIntegrity drift yok, CQRS conventions intact)
- [x] **172/172 test** yeşil (Domain 149, Application 5, E2E 3, Architecture 15)

**Commit:** `feat(domain): residency bounded context with value objects and tc validation (W2 Day 3)`

---

### Gün 4 (Perşembe) — EF Mapping + Migration + Repositories ✅

**Hedef:** Property + Residency tabloları Postgres'te oluştu, repository + read-query port'ları hazır, CQRS read/write tam ayrı.

- [x] **Mimari kararlar (tartışılarak alındı, AI hazırladığı roadmap'te eksikti):**
  - Repository ports **Application'da, Domain'de değil** — Domain BCL-only kalsın
  - **Generic `IRepository<TRoot> where TRoot : AggregateRoot<Guid>`** (Microsoft / Ardalis / Jason Taylor template'leri yaklaşımı) — boilerplate azaltır, compile-time aggregate-root garantisi verir
  - **CQRS read tarafı tam ayrılmış**: Repository **command-only** (`ListAsync` yok); query handler'lar Infrastructure'daki `ISiteQueries` / `IResidentQueries` port'ları üzerinden projection yapar — **EF Application'da yok**
  - **Shadow foreign keys** (Block.SiteId C# tarafında yok, EF config'le tanımlanır) — Domain temizliği için klasik "parent reference" pattern'i kullanılmadı

- [x] `Application/Abstractions/Persistence/`:
  - `IRepository<TRoot>` — command-side `GetByIdAsync` + `AddAsync` + `Remove` (NO ListAsync, NO Update — root state değişikliği change tracker'la)
  - `ISiteRepository : IRepository<Site>`, `IResidentRepository : IRepository<Resident>` + `FindByTcNoAsync(TcNo)`
  - `IUnitOfWork.SaveChangesAsync()` — handlers'ın commit boundary'si

- [x] **Read-side port'ları** (`Application/{Property,Residency}/Queries/`):
  - `ISiteQueries` + `SiteListItemDto` + `SiteDetailsDto` (BlockSummary + ApartmentSummary)
  - `IResidentQueries` + `ResidentListItemDto` + `ResidentDetailsDto` (VehicleDto)
  - DTO'lar Application'da yaşar, EF tipi yok

- [x] `Infrastructure/Persistence/Configurations/Property/`:
  - `SiteConfiguration` — `Sites` table, blocks 1:N **required** + cascade, `DomainEvents` ignored, backing field `_blocks`
  - `BlockConfiguration` — `Blocks` table, apartments 1:N **required** + cascade, BlockName converter, backing field `_apartments`
  - `ApartmentConfiguration` — `Apartments` table, ApartmentNumber/Floor/ApartmentType converters, OccupancyStatus → string

- [x] `Infrastructure/Persistence/Configurations/Residency/`:
  - `ResidentConfiguration` — `Residents` table, TcNo + Email **unique index**, FullName **flattened** via `ComplexProperty` (`FullName_FirstName`/`FullName_LastName`), Vehicles `OwnsMany` → `ResidentVehicles` child table with surrogate int PK + (`ResidentId`, `Plate`) unique index

- [x] Value object converter'lar (8 tane, `Infrastructure/Persistence/Converters/`):
  - Property: `ApartmentTypeConverter`, `ApartmentNumberConverter`, `FloorConverter`, `BlockNameConverter`
  - Residency: `TcNoConverter`, `EmailConverter`, `PhoneNumberConverter`, `PlateNumberConverter`

- [x] **No magic literals**:
  - `SchemaConstants` (Infrastructure) — table names, FK column names (`SiteId`, `BlockId`, `ResidentId`), backing field names, owned surrogate key
  - `PropertyLimits` — yeni eklenenler: `ApartmentTypeMaxLength`, `OccupancyStatusMaxLength`
  - `ResidencyLimits` — yeni eklenen: `PlateNumberMaxLength`

- [x] `AppDbContext` — `DbSet<Site>`, `DbSet<Resident>` + `ApplyConfigurationsFromAssembly` ile otomatik discovery; Identity tablo isimleri `AspNet*` prefix'siz

- [x] Repository implementations (`Infrastructure/Persistence/Repositories/`):
  - `SiteRepository` — `GetByIdAsync` site → blocks → apartments eager-load
  - `ResidentRepository` — `GetByIdAsync` + `FindByTcNoAsync` vehicles eager-load
  - `EfUnitOfWork` — `SaveChangesAsync` AppDbContext'e delegate

- [x] Read implementations (`Infrastructure/Persistence/Queries/`):
  - `SiteQueries` — `.AsNoTracking().Select(...)` DTO projection, no eager Include zinciri
  - `ResidentQueries` — aynı pattern

- [x] DI registration — `PersistenceExtensions.AddPersistence()` ayrı extension, `AddInfrastructure` ona delegate eder (Program.cs lean kalır)

- [x] `dotnet ef migrations add PropertyAndResidency -p src/SiteManagement.Infrastructure -s src/SiteManagement.Api --output-dir Persistence/Migrations`

- [x] Startup'ta otomatik migrate (DatabaseInitializer.MigrateAndSeedAsync W1'den beri yazılı, yeni migration'ı otomatik uyguladı)

- [x] **Postgres'te tablolar doğrulandı:**
  - `Sites`, `Blocks`, `Apartments`, `Residents`, `ResidentVehicles` + Identity tabloları + `__EFMigrationsHistory`
  - `Blocks.SiteId` ve `Apartments.BlockId` **NOT NULL** (`IsRequired()` eklendiği için), cascade delete aktif
  - Unique index'ler: `IX_Residents_TcNo`, `IX_Residents_Email`, `IX_ResidentVehicles_ResidentId_Plate`
  - FullName flattened columns: `FullName_FirstName`, `FullName_LastName`

- [x] **Architecture testleri yeşil** (Application hala EF Core / ASP.NET Core / Identity referansından bağımsız — query'ler bile Application'da `DbSet` görmüyor)
- [x] 172/172 test yeşil, `/health` Healthy, compose stack ayakta

**W2 Gün 5'e bırakılan:** Round-trip + DB-constraint test'leri için TestContainers altyapısı zaten Gün 5'te command handler test'leriyle kurulacak; iki kez kurmamak için bu commit'te yer almıyor.

**Commit:** `feat(infra): ef mappings, migrations and repositories for property and residency (W2 Day 4)`

---

### Gün 5 (Cuma) — Application Handlers + API Endpoints ✅

**Hedef:** API endpoint'leri Scalar'da görünüyor, smoke test ile full akış çalışıyor.

**Mimari kararlar (oturum sırasında tartışılarak alındı):**
- **Public register endpoint silindi** — `POST /api/auth/register` privilege escalation hole'üydü; bootstrap admin env'den startup'ta seed edilir (`AdminBootstrapOptions`)
- **`IUserAuthService` split** — `RegisterAdminAsync` + `RegisterResidentUserAsync(residentId, ...)`; admin/resident kayıt flow'ları imza seviyesinde ayrı
- **`AppUser.ResidentId` nullable FK** — admin'ler null, resident'lar Resident.Id'ye işaret eder; unique index `WHERE ResidentId IS NOT NULL` ile filtered
- **`AuthClaims.ResidentId` JWT claim** — `TokenService.IssueTokens(..., residentId)` ile gömülür; resident endpoint'leri claim'den okur, body'den almaz (IDOR koruması)
- **`ICurrentUser` port** — Application/Abstractions; Api implementation `HttpContext.User` claim'lerini okur. Resident self-service endpoint'leri için W3'te kullanılacak altyapı
- **`IUnitOfWork.BeginScopeAsync` + `IUnitOfWorkScope`** — multi-write command'lar (RegisterResident: Resident + AppUser + welcome email) için explicit transaction scope. `await using` pattern + try/catch-free rollback
- **Strict atomicity** — RegisterResident'ta welcome email **transaction içinde**; SMTP fail olursa rollback (sistem şifreyi bilen tek taraf, mail gitmezse resident kullanılamaz)
- **`MarkAsAdded<T>` workaround** — `site.AddBlock(...)` ile child eklendiğinde EF tracker yanlışlıkla "modified" sayıyor, INSERT yerine UPDATE atıyor; IUnitOfWork'e port ekledik, AddBlock/AddApartment handler'lar bunu çağırır
- **`xmin` concurrency token** — Site/Block/Resident tablolarında Postgres'in sistem kolonu mapped; boş UPDATE'leri önler, real optimistic concurrency verir. Migration no-op (xmin zaten Postgres'te var)
- **`CommonValidationRules` genişletildi** — `ValidId`, `InRange`, `ValidTcNo`, `ValidPhoneNumber`, `ValidPlateNumber`, `ValidNameComponent` (her command'da tekrar yazmak yerine)
- **`CreatedAtAction` 201 + Location header** — REST best practice

- [x] `Application/Property/Commands/`: CreateSite, AddBlock, AddApartment, MarkApartmentOccupied, MarkApartmentEmpty, DeleteSite
- [x] `Application/Property/Queries/`: GetSiteByIdQuery, ListSitesQuery (her ikisi de `ISiteQueries` projection üzerinden)
- [x] `Application/Residency/Commands/`: RegisterResident (transactional), UpdateContactInfo, AddVehicle, RemoveVehicle
- [x] `Application/Residency/Queries/`: GetResidentByIdQuery, ListResidentsQuery
- [x] FluentValidation validator'lar her command için, `CommonValidationRules` kullanılarak
- [x] Handler'lar: temiz, try/catch yok, sadece domain çağrısı + EntityNotFound throw + UoW commit
- [x] `Api/Controllers/Sites/SitesController.cs` ([Authorize(Roles="Admin")]) — POST sites, POST sites/{}/blocks, GET sites, GET sites/{}, DELETE sites/{}
- [x] `Api/Controllers/Apartments/ApartmentsController.cs` — POST blocks/{}/apartments, POST apartments/{}/occupy, POST apartments/{}/vacate
- [x] `Api/Controllers/Residents/ResidentsController.cs` — POST residents, GET residents, GET residents/{}, PUT residents/{}/contact, POST residents/{}/vehicles, DELETE residents/{}/vehicles/{plate}
- [x] `ProducesResponseType` annotation'ları her action'da (200/201/204/400/401/404/409 vakaları)
- [x] **EF Core 10 bug fix'leri** — owned-collection projection'unda `OrderBy().Select()` çalışmıyor, ResidentQueries.GetByIdAsync için 2-step (Include + client-side projection)
- [x] **Live smoke test (compose stack)** — full flow doğrulandı:
  - Bootstrap admin login → access token
  - POST site → 201
  - POST block → 201
  - POST apartment → 201
  - POST apartment/occupy → 204
  - POST resident → 201 + welcome email MailHog'a düştü
  - POST vehicle → 201
  - GET site/{id} → full hierarchy
  - GET resident/{id} → vehicles dahil

**W2 Gün 6-7'e bırakılan:** TestContainers altyapısı + 2-3 integration test (Resident round-trip, TcNo unique constraint, AddBlock/Apartment flow).

**Commit'ler (3 part):**
- `063c10a refactor: foundation for W2 day 5 + plug privilege-escalation hole` (UoW scope, UserAuthService split, AppUser.ResidentId, ICurrentUser, bootstrap admin)
- `6235ef0 feat(app): property + residency command and query handlers (W2 day 5, part 1)` (CommonValidationRules, helpers, command/query handlers)
- `a78dfab feat(api): sites + apartments + residents controllers (W2 day 5, part 2) — WIP` (controllers)
- `<final> fix: ef concurrency token + add-child workaround + owned-collection query split (W2 day 5, part 3)` (xmin tokens, MarkAsAdded, ResidentQueries 2-step)

---

### Gün 6-7 (Cumartesi-Pazar) — Angular Setup + Admin Sayfaları

**Hedef:** Tarayıcıdan admin login + property/resident CRUD çalışıyor.

**Gün 6:**
- [ ] `web/` klasörü: `ng new site-management-web --standalone --routing --style=scss`
- [ ] Angular Material kur (`ng add @angular/material`), Indigo/Pink theme
- [ ] ngx-translate kur (`@ngx-translate/core`, `@ngx-translate/http-loader`)
- [ ] `assets/i18n/tr.json`, `assets/i18n/en.json` (auth + common + sites + residents key'leri)
- [ ] Language switcher component (topbar'a koymak için)
- [ ] `core/services/auth.service.ts`:
  - register, login, refresh, logout
  - JWT'yi localStorage'a koy
- [ ] `core/interceptors/auth.interceptor.ts` (HTTP'ye Bearer header ekle, 401'de refresh dene)
- [ ] `core/guards/auth.guard.ts`, `core/guards/role.guard.ts`
- [ ] `features/auth/login/login.component.ts` (reactive form)
- [ ] `layouts/admin-layout.component.ts` (mat-sidenav + mat-toolbar)
- [ ] Routing: `/login` (public), `/admin/*` (Admin role)
- [ ] CORS backend'te Angular dev URL'i için açık (http://localhost:4200)

**Gün 7:**
- [ ] `features/admin/sites/`:
  - `site-list.component.ts` (mat-table + create/edit dialog)
  - `site-form.component.ts` (reactive form)
- [ ] `features/admin/blocks/` (site detayında)
- [ ] `features/admin/apartments/`:
  - `apartment-list.component.ts` (filter by block, status badge)
  - `apartment-form.component.ts`
- [ ] `features/admin/residents/`:
  - `resident-list.component.ts`
  - `resident-form.component.ts` (TcNo input + validation feedback)
- [ ] Backend'den hata gelirken (RFC 7807 ProblemDetails) Angular'da güzel toast/snackbar göster
- [ ] Manuel test: admin oluştur → login → site oluştur → blok ekle → daire ekle → sakin oluştur → şifre console'dan al (mail simülasyonu sonraki haftada)

**Commit (Gün 6):** `feat(web): angular setup with material, auth and i18n`
**Commit (Gün 7):** `feat(web): admin pages for property and residency`

---

### E2E Test Altyapısı (W2 Gün 6 — Angular öncesi yapıldı) ✅

**Hedef:** TestContainers + WebApplicationFactory yerinde, integration test'ler yeşil.

**Mimari kararlar (oturum sırasında):**
- **TestContainers + xunit collection fixture** — tek Postgres container session boyunca paylaşılır; `ResetDomainDataAsync` her test öncesi domain tablolarını TRUNCATE + bootstrap olmayan Identity user'ları siler. Hızlı (~7 saniye 8 test) + izole.
- **RecordingEmailSender** — `IEmailSender` test-only impl, gerçek SMTP yerine in-memory queue. Welcome email assertion'larını mümkün kılar.
- **CustomWebApplicationFactory** — `WebApplicationFactory<Program>` üzerinde `ConfigureAppConfiguration` ile in-memory config override (connection string, JWT key, bootstrap admin, SMTP).
- **`AuthFlow.LoginAsBootstrapAdminAsync` helper** — login + token boilerplate'ini tek yere çek; her test sadece kendi senaryosuna odaklanır.

**JWT eager-read bug fix:**
- `AuthExtensions.AddJwtAuth` `configuration.GetSection().Get<JwtOptions>()` **eager** çağırıyordu (DI register time)
- `WebApplicationFactory.ConfigureAppConfiguration` bu çağrıdan sonra çalıştığı için test'lerin in-memory JWT key'i `TokenValidationParameters`'a hiç gelmiyordu → token üretimi + doğrulama farklı key'lerle yapılıyordu
- Fix: `services.AddOptions<JwtBearerOptions>().Configure<IOptions<JwtOptions>>((bearer, jwt) => ...)` ile **lazy bind** — `IOptions<JwtOptions>` runtime'da resolve olduğundan in-memory override doğru şekilde okunuyor

- [x] `tests/SiteManagement.E2E.Tests/Infrastructure/`:
  - `PostgresFixture` — `Testcontainers.PostgreSql` ile `postgres:16-alpine` container; session boyunca paylaşılır
  - `ApiCollection` — xunit `ICollectionFixture<PostgresFixture>` collection definition
  - `CustomWebApplicationFactory` — in-memory config + `RecordingEmailSender` swap + `ResetDomainDataAsync` cleanup
  - `RecordingEmailSender` — test-only `IEmailSender` (ConcurrentQueue ile thread-safe)
  - `AuthFlow` — login helper + Bearer token client extension
- [x] **5 integration test** (`PropertyFlowTests` + `ResidentFlowTests`):
  - `Admin_CanBuildFullPropertyHierarchy` — site → block → apartment → occupy → detail readback (full chain)
  - `AddingBlockWithDuplicateName_IsRejectedAsConflict` — case-insensitive duplicate name → 409
  - `RegisterResident_PersistsAggregateAndQueuesWelcomeEmail` — happy path + welcome email captured with non-empty password
  - `RegisterResident_WithDuplicateTcNo_IsRejectedAsConflict` — duplicate TC handler-level check
  - `DifferentEmails_ButSameTcNo_StillRejected` — Application-level check + DB unique index fallback
- [x] **InMemoryRefreshTokenStore.Clear()** — singleton state'in test'ler arası temizliği için
- [x] **CI uyumluluğu** — `Testcontainers` GitHub Actions runner'da Docker daemon erişimi ile çalışır (zaten mevcut Postgres service container yerine container-per-test).
- [x] **CORS extension** — `CorsExtensions.AddSiteManagementCors`:
  - `DevelopmentPolicy` — `http://localhost:4200` (Angular dev) + `Cors:AllowedOrigins` config eklemeleri
  - `ProductionPolicy` — sadece `Cors:AllowedOrigins` config (whitelist)
  - Pipeline `UseCors` Auth'tan **önce** çalışır (preflight 401 olmasın)
  - `appsettings.json`, `docker-compose.yml`, `.env.example` örnek değerlerle güncellendi
- [x] 177/177 test yeşil (Domain 149, Application 5, E2E 8 ← +5 yeni integration, Architecture 15)

**Commit:** `test(e2e): testcontainers infrastructure + 5 integration tests, jwt eager-read fix, cors (W2 Day 6)`

---

### Hafta 2 Çıktısı

- Admin tarayıcıdan tam property + residency CRUD yapabiliyor
- Domain coverage %80+
- E2E test infrastructure yerinde + ilk akış yeşil
- Deploy edilmiş, live URL'de Angular UI çalışıyor

**Hafta 2 sonu self-review:**
- Aggregate root'lar collection'ları read-only mu expose ediyor?
- Setter'lar private mi?
- Domain'de IServiceProvider, DbContext veya ASP.NET reference var mı? (Sıfır olmalı)
- Frontend hardcoded Türkçe string içeriyor mu? (Sıfır olmalı)
- Handler'lar try/catch içermiyor (EntityNotFound dışında)?
- TcNo gerçekten checksum doğrulaması yapıyor mu?

---

## Tatil (Bayram) — Eğer İki Hafta Yetişmezse

Bayramda kapatılabilecek artıklar (öncelik sırasına göre):

1. **Eksik W2 task'larını bitir** (Angular sayfaları, E2E altyapısı)
2. **W3 başla**: Tenancy domain + DuesPeriod (TDD ile en keyifli kısım, sakin ortamda iyi gider)
3. **W4 başla**: PaymentService microservice (yeni teknoloji öğrenme keyfi)

Bayramda en az 1 gün gez, dinlen, hiçbir şeye dokunma. Yorgun kodlama berbat kod üretir, sonra refactor cehennemi olur.
