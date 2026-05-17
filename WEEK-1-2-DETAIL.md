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

### Gün 3 (Çarşamba) — ASP.NET Core Identity + EF Core

**Hedef:** Migration koştu, Identity tabloları Postgres'te oluştu, roller seed'lendi.

- [ ] `Infrastructure/Persistence/AppDbContext.cs` → `IdentityDbContext<AppUser, AppRole, Guid>`
- [ ] `AppUser : IdentityUser<Guid>` (ek alan yoksa şimdilik boş; ileride Resident ile linkleyeceğiz)
- [ ] `AppRole : IdentityRole<Guid>`
- [ ] `Infrastructure/Persistence/SeedData/RoleSeeder.cs`: Admin + Resident rollerini sabit Guid'lerle ekle
- [ ] `Program.cs`'te DI:
  - `services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString))`
  - `services.AddIdentity<AppUser, AppRole>(...).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders()`
- [ ] `dotnet ef migrations add Initial -p src/SiteManagement.Infrastructure -s src/SiteManagement.Api`
- [ ] `dotnet ef database update -p src/SiteManagement.Infrastructure -s src/SiteManagement.Api`
- [ ] Startup'ta migration auto-apply (development için) + role seeding hook
- [ ] Postgres'te AspNet* tablolarının oluştuğunu kontrol et

**Commit:** `feat: aspnet identity with postgres, role seeding`

---

### Gün 4 (Perşembe) — JWT + Auth Endpoints + Serilog + Swagger

**Hedef:** Postman'de register + login + protected endpoint çalışıyor.

- [ ] `Api/Auth/AuthController.cs`:
  - POST `/auth/register` (email, password, fullName → Admin oluştur; sonra resident için ayrı flow olacak)
  - POST `/auth/login` (email, password → access + refresh token)
  - POST `/auth/refresh` (refresh token → yeni access token)
- [ ] `Api/Auth/TokenService.cs`: JWT üret (issuer, audience, key config'den; user'ın role'leri claim olarak)
- [ ] `appsettings.json`'a Jwt section (Key, Issuer, Audience, AccessTokenMinutes, RefreshTokenDays)
- [ ] `Program.cs`'te JWT bearer auth registration
- [ ] Serilog setup: console sink + dosya sink (Serilog.Sinks.Console, Serilog.Sinks.File), structured logging, RequestLoggingMiddleware
- [ ] Swagger UI: JWT auth button (Bearer scheme tanımı), endpoint'lere XML doc
- [ ] Health endpoint: `app.MapHealthChecks("/health")` + DB check (AspNetCore.HealthChecks.NpgSql)
- [ ] Postman testi: register admin → login → token al → /health'e Authorization: Bearer {token} ile çağrı → 200

**Commit:** `feat: jwt auth (register/login/refresh), serilog, swagger, health endpoint`

---

### Gün 5 (Cuma) — Exception Architecture

**Hedef:** Domain ↔ Application ↔ Api exception katmanları doğru çalışıyor, test'lerle doğrulanmış.

- [ ] `Domain/Shared/Exceptions/DomainException.cs` (abstract; ctor: messageKey, args)
- [ ] Örnek alt class: `Domain/Shared/Exceptions/SampleDomainException.cs` (test amaçlı)
- [ ] `Application/Shared/Exceptions/ApplicationException.cs` (abstract)
- [ ] Alt class'lar:
  - `BusinessRuleViolationException` (status 409)
  - `EntityNotFoundException` (status 404)
  - `UnauthorizedActionException` (status 403)
  - `ValidationException` (status 400, FluentValidation'dan)
- [ ] `Application/Behaviors/ExceptionTranslationBehavior.cs` (MediatR `IPipelineBehavior<,>`):
  - try/catch DomainException → switch expression ile uygun ApplicationException
- [ ] `Application/Behaviors/ValidationBehavior.cs` (FluentValidation pipeline)
- [ ] `Application/Behaviors/LoggingBehavior.cs` (request/response log)
- [ ] DI'a behavior'ları register et (MediatR config, `AddOpenBehavior(...)`)
- [ ] `Api/Middleware/GlobalExceptionMiddleware.cs`: ApplicationException → RFC 7807 ProblemDetails JSON + uygun HTTP status
- [ ] Unit test (`Application.Tests`):
  - Fake handler içinde fake DomainException at → behavior'ın yakaladığını ve doğru ApplicationException döndüğünü doğrula
- [ ] E2E test (`E2E.Tests`):
  - Geçersiz request → API 400/409 + ProblemDetails formatında JSON

**Commit:** `feat: layered exception architecture with mediatr translation pipeline`

---

### Gün 6 (Cumartesi) — Localization (Backend)

**Hedef:** Hata mesajları kültüre göre tr-TR / en-US dönüyor.

- [ ] `Application/Resources/ErrorMessages.tr.resx` (key → Türkçe mesaj)
- [ ] `Application/Resources/ErrorMessages.en.resx` (key → İngilizce mesaj)
- [ ] DomainException MessageKey'leri sabit constant'lar olarak: `"Apartment.AlreadyOccupied"`, `"TcNo.Invalid"`, vs.
- [ ] `Program.cs`'te:
  - `services.AddLocalization(opts => opts.ResourcesPath = "Resources")`
  - RequestLocalizationOptions: SupportedCultures = [tr-TR, en-US], DefaultRequestCulture = tr-TR
  - `app.UseRequestLocalization(...)`
- [ ] ExceptionTranslationBehavior'da IStringLocalizer ile MessageKey'i çevir, lokalize mesajla ApplicationException at
- [ ] E2E test:
  - `Accept-Language: en-US` header'ı ile request → response message İngilizce
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
