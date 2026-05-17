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

- [ ] `.github/workflows/ci.yml`:
  - Trigger: pull_request, push to main
  - Steps: checkout, setup-dotnet@v4 (10.0.x), restore, build, test
  - Postgres service container (test'lerin DB'ye erişimi için) veya TestContainers (Docker-in-Docker)
  - Test sonuç raporlamayı GitHub'a publish et
- [ ] Main branch protection: PR'da review zorunlu (eğer tek başına çalışıyorsan kapalı tut), status check zorunlu
- [ ] Railway hesabı, GitHub repo bağla
- [ ] Railway'de:
  - PostgreSQL add-on
  - Environment variables: ConnectionStrings__DefaultConnection, Jwt__Key, vs.
  - Dockerfile-based deploy
- [ ] İlk deploy → public URL al
- [ ] Postman'le live URL'de register + login test, /health 200
- [ ] README v1:
  - Proje özeti (Patika bitirme + DDD ile modernize)
  - Tech stack listesi
  - Local çalıştırma (`docker compose up`)
  - Live URL
  - Mimari özeti placeholder (W6'da detay)

**Commit:** `chore: github actions ci, railway deploy, readme v1`

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
- [ ] `Domain/Property/Exceptions/`:
  - `InvalidApartmentTypeException : DomainException`
  - `DuplicateApartmentNumberException : DomainException`
  - `ApartmentAlreadyOccupiedException : DomainException`
  - `ApartmentNotOccupiedException : DomainException`
  - `EmptyBlockException : DomainException` (boş blok silinemez gibi kural varsa)
- [ ] **TDD — önce test yaz** (`Domain.Tests/Property/ApartmentTests.cs`):
  - `Create_WithInvalidType_Throws`
  - `Create_DefaultStatus_IsEmpty`
  - `MarkAsOccupied_WhenEmpty_ChangesStatusToOccupied`
  - `MarkAsOccupied_WhenAlreadyOccupied_Throws`
  - `MarkAsEmpty_WhenOccupied_ChangesStatusToEmpty`
- [ ] `Domain/Property/Apartment.cs`:
  - private ctor, public factory `Apartment.Create(ApartmentNumber, Floor, ApartmentType)`
  - Status property (private set), enum (Empty, Occupied)
  - Behaviors: `MarkAsOccupied()`, `MarkAsEmpty()`, `ChangeType(ApartmentType)`
- [ ] **Test yaz** (`BlockTests.cs`):
  - `AddApartment_WithDuplicateNumber_Throws`
  - `Block_Apartments_IsReadOnly` (encapsulation)
- [ ] `Domain/Property/Block.cs`:
  - private List<Apartment> _apartments, public IReadOnlyCollection<Apartment> Apartments
  - `AddApartment(...)`, `RemoveApartment(apartmentId)`
- [ ] **Test yaz** (`SiteTests.cs`):
  - `AddBlock_WithDuplicateName_Throws`
  - `Site_IsAggregateRoot_ManagesBlocks`
- [ ] `Domain/Property/Site.cs` (aggregate root):
  - private List<Block> _blocks, public IReadOnlyCollection<Block> Blocks
  - `Site.Create(name, address)` factory
  - `AddBlock(...)`, `RemoveBlock(...)`
- [ ] `coverlet` ile coverage report → Domain.Property %85+ doğrula

**Commit:** `feat(domain): property bounded context with rich aggregates and tdd invariants`

---

### Gün 3 (Çarşamba) — Residency Domain (TDD)

**Hedef:** Resident aggregate ve value object'ler tamam, TC validation çalışıyor.

- [ ] `Domain/Residency/ValueObjects/`:
  - `TcNo` (string, 11 hane + checksum validation — gerçek TC algoritması)
  - `FullName` (firstName, lastName, max length)
  - `Email` (regex validation)
  - `PhoneNumber` (Türkiye formatı: +90 veya 0 ile başlar, 10 hane)
  - `PlateNumber` (Türkiye plaka format'ı: il kodu + harf + sayı)
  - `VehicleInfo` (PlateNumber, opsiyonel ek bilgi)
- [ ] `Domain/Residency/Exceptions/`:
  - `InvalidTcNoException`
  - `InvalidEmailException`
  - `InvalidPhoneNumberException`
  - `InvalidPlateNumberException`
- [ ] **TDD** (`Domain.Tests/Residency/TcNoTests.cs`):
  - Geçerli TC ile create → success
  - Checksum'ı yanlış TC → exception
  - 11 haneden az/fazla → exception
  - Harf içeriyor → exception
- [ ] `Domain/Residency/Resident.cs` (aggregate root):
  - Id, TcNo, FullName, Email, PhoneNumber, Vehicles (List<VehicleInfo>)
  - Factory `Resident.Create(...)`
  - Behaviors: `UpdateContactInfo(email, phone)`, `AddVehicle(VehicleInfo)`, `RemoveVehicle(plateNumber)`, `ChangeFullName(...)`
- [ ] **Test yaz** (`ResidentTests.cs`):
  - `Create_WithValidData_Success`
  - `AddVehicle_WithDuplicatePlate_Throws`
  - `RemoveVehicle_WhenNotExists_Throws`

**Commit:** `feat(domain): residency bounded context with value objects and tc validation`

---

### Gün 4 (Perşembe) — EF Mapping + Migration + Repositories

**Hedef:** Property + Residency tabloları Postgres'te oluştu, repository'ler hazır.

- [ ] `Infrastructure/Persistence/Configurations/`:
  - `SiteConfiguration : IEntityTypeConfiguration<Site>`
  - `BlockConfiguration : IEntityTypeConfiguration<Block>` (FK to Site)
  - `ApartmentConfiguration : IEntityTypeConfiguration<Apartment>` (FK to Block)
  - `ResidentConfiguration : IEntityTypeConfiguration<Resident>`
  - `VehicleInfoConfiguration` (owned entity collection)
- [ ] Value object converter'lar:
  - `TcNoConverter : ValueConverter<TcNo, string>`
  - `EmailConverter`, `PhoneNumberConverter`, vs.
- [ ] `AppDbContext.OnModelCreating`'de configuration'ları apply et
- [ ] `dotnet ef migrations add PropertyAndResidency -p src/SiteManagement.Infrastructure -s src/SiteManagement.Api`
- [ ] `dotnet ef database update ...`
- [ ] Postgres'te tabloları gör (Sites, Blocks, Apartments, Residents, ResidentVehicles)
- [ ] Repository interface'leri Domain'de:
  - `Domain/Property/ISiteRepository.cs` (GetByIdAsync, AddAsync, ListAsync, ...)
  - `Domain/Residency/IResidentRepository.cs`
- [ ] Implementation'lar:
  - `Infrastructure/Repositories/SiteRepository.cs`
  - `Infrastructure/Repositories/ResidentRepository.cs`
- [ ] DI registration

**Commit:** `feat(infra): ef mappings, migrations and repositories for property and residency`

---

### Gün 5 (Cuma) — Application Handlers + API Endpoints

**Hedef:** API endpoint'leri Swagger'da görünüyor, Postman'le çalışıyor.

- [ ] `Application/Property/Commands/`:
  - `CreateSiteCommand`, `CreateSiteCommandHandler`, `CreateSiteCommandValidator`
  - `AddBlockCommand`, ...
  - `AddApartmentCommand`, ...
  - `UpdateApartmentCommand`, ...
  - `DeleteApartmentCommand`, ...
- [ ] `Application/Property/Queries/`:
  - `GetSiteByIdQuery`, `ListSitesQuery`, `ListApartmentsByBlockQuery`, ...
- [ ] `Application/Residency/Commands/`:
  - `CreateResidentCommand` (otomatik password generation: random 12 char, BCrypt hash, AppUser oluştur, ResidentId-AppUserId link)
  - `UpdateResidentCommand`, `DeleteResidentCommand`
- [ ] `Application/Residency/Queries/`:
  - `GetResidentByIdQuery`, `ListResidentsQuery`
- [ ] FluentValidation validator'lar her command için
- [ ] Handler'lar: temiz, try/catch yok, sadece domain çağrısı + EntityNotFound throw
- [ ] `Api/Controllers/PropertyController.cs` ([Authorize(Roles="Admin")])
- [ ] `Api/Controllers/ResidentController.cs` ([Authorize(Roles="Admin")])
- [ ] Swagger annotations (ProducesResponseType ile)
- [ ] Postman koleksiyonu hazırla: auth → property CRUD → resident CRUD

**Commit:** `feat(app): property and residency command/query handlers, validators, api endpoints`

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

### E2E Test Altyapısı (Gün 7 sonu veya pazartesiye sarkabilir)

**Hedef:** TestContainers + WebApplicationFactory yerinde, ilk E2E test yeşil.

- [ ] `tests/SiteManagement.E2E.Tests/`:
  - `CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime`
  - InitializeAsync: PostgreSqlContainer kaldır, connection string override
  - DisposeAsync: container kapat
- [ ] `Fixtures/DatabaseFixture.cs` (xUnit collection fixture)
- [ ] İlk E2E test (`PropertyFlowTests.cs`):
  - Register admin (DB'ye seed veya endpoint çağrısı)
  - Login → token al
  - POST /sites → 201
  - POST /sites/{id}/blocks → 201
  - POST /blocks/{id}/apartments → 201
  - GET /sites/{id} → tüm yapı görünür
  - Assertions FluentAssertions ile
- [ ] CI'a E2E test koşumunu ekle (Docker-in-Docker veya GitHub Actions PostgreSQL service container)
- [ ] README'de "How to run E2E tests" bölümü

**Commit:** `test(e2e): testcontainers infrastructure with first property flow`

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
