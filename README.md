# SiteManagement

> Patika bitirme projesinin **DDD + TDD + Clean Architecture** ile modern bir yorumu. Site (apartman kompleksi) yönetim sistemi: admin daire/blok yönetimi, sakin kaydı, aidat & fatura dağıtımı; resident kendi borçlarını görüp kredi kartıyla öder. Ödeme tarafı ayrı bir microservice.

[![CI](https://github.com/mtsmsek/SiteManagement/actions/workflows/ci.yml/badge.svg)](https://github.com/mtsmsek/SiteManagement/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com/)

🚧 **Work in progress** — şu an **Hafta 1 tamam** (Foundation & Deploy). [ROADMAP.md](ROADMAP.md) toplam 6 haftalık planı, [WEEK-1-2-DETAIL.md](WEEK-1-2-DETAIL.md) gün gün ilerlemeyi içerir.

---

## ✅ Bugün ne çalışıyor

- ✅ `docker compose up -d --build` ile postgres + mongo + mailhog + api tek komutta ayağa kalkar
- ✅ ASP.NET Core Identity + PostgreSQL (EF Core 10 migration'ları startup'ta otomatik uygulanır)
- ✅ JWT bearer auth — **register / login / refresh** endpoint'leri, refresh token rotation
- ✅ CQRS-lite (MediatR command/query ayrımı) + FluentValidation + 3 pipeline behavior:
  - `LoggingBehavior` — her command için structured request/elapsed log
  - `ValidationBehavior` — handler çalışmadan önce FluentValidation çalıştırır
  - `ExceptionTranslationBehavior` — Domain → Application exception translation tek noktada
- ✅ **Üç katmanlı exception architecture** + RFC 7807 ProblemDetails responses
- ✅ **Backend localization** (tr-TR default + en-US) — `Accept-Language` header'a göre lokalize mesajlar; FluentValidation placeholder'ları doğru substitue olur
- ✅ Serilog + structured request logging
- ✅ Scalar API docs UI (`/scalar/v1`) — Bearer auth panel ile
- ✅ Health checks (Postgres probe dahil)
- ✅ GitHub Actions CI (build + test, Postgres service container)
- ✅ Railway deploy hazır (PORT + DATABASE_URL platform env'lerini otomatik handle eder)

🔜 **Hafta 2:** Property + Residency bounded context'leri (rich domain, TDD), Angular admin UI.

---

## Tech Stack

| Katman | Seçim |
|---|---|
| Runtime | .NET 10 |
| Web framework | ASP.NET Core 10 (controllers, OpenAPI v2) |
| Main DB | PostgreSQL 16 |
| Payment DB | MongoDB 7 _(W4)_ |
| ORM | EF Core 10 |
| Auth | ASP.NET Core Identity + JWT bearer |
| CQRS bus | MediatR 14 |
| Validation | FluentValidation 12 |
| Logging | Serilog (structured, request logging) |
| API docs | Scalar UI (Swashbuckle yerine — .NET 10 / OpenApi 2.0 uyumlu) |
| HTTP client | Refit + Polly _(W4)_ |
| Localization | `IStringLocalizer` + .resx (tr-TR, en-US) |
| Frontend | Angular (standalone components) _(W2)_ |
| UI lib | Material veya PrimeNG _(W6)_ |
| i18n (frontend) | ngx-translate _(W2)_ |
| Test | xUnit, FluentAssertions, NSubstitute, Testcontainers |
| Container | Docker + Compose v2 |
| CI | GitHub Actions |
| Deploy | Railway |

---

## Mimari özeti

```
SiteManagement.Domain        ← framework-free; aggregate'lar, value object'ler, domain event'ler
       ▲
SiteManagement.Application   ← MediatR command/query handler'ları, FluentValidation, port'lar (interface'ler)
       ▲
SiteManagement.Infrastructure ← EF Core, Identity, JWT token service, repository implementations
       ▲
SiteManagement.Api           ← Controller'lar, middleware, OpenAPI/Scalar, Serilog wireup, Program.cs
```

- **Domain** sıfır external bağımlılık. Sadece BCL. Aggregate root'lar invariant'larını kendi içlerinde tutar; setter'lar private.
- **Application** sadece port'ları (`ITokenService`, `IUserAuthService`, `IRefreshTokenStore`) bilir — Identity / JWT / EF Core referansı yok. CQRS-lite ile her use case bir command/query + handler + validator.
- **Infrastructure** port'ların concrete implementation'larını + EF Core mapping + Identity setup'ını barındırır.
- **Api** thin — controller'lar `ISender.Send(command)` çağırır, business logic yoktur.

Detaylı kararlar (rich domain prensipleri, exception translation kuralı, test stratejisi): [ROADMAP.md](ROADMAP.md).

---

## Local Setup

> Sıfırdan kurulum (yeni makinede): **[docs/SETUP-MACHINE.md](docs/SETUP-MACHINE.md)** — .NET SDK, Docker Desktop, WSL2, git/gh, opsiyonel araçlar dahil.

### Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Compose v2) + WSL2 (Windows)
- _Önerilen:_ JetBrains Rider / Visual Studio 2022 17.13+ / VS Code + C# Dev Kit

### Hızlı başlangıç

```powershell
git clone https://github.com/mtsmsek/SiteManagement.git
cd SiteManagement

# 1) Env dosyasını oluştur (varsayılan dev credential'larıyla yeterli)
Copy-Item .env.example .env

# 2) Tüm stack'i ayağa kaldır (postgres + mongo + mailhog + api)
docker compose up -d --build

# 3) Health endpoint
curl http://localhost:8080/health
# -> Healthy
```

### Auth smoke

```powershell
$base = "http://localhost:8080"

# Register
$body = @{ email = "admin@local"; password = "Str0ng-P@ss"; fullName = "Admin" } | ConvertTo-Json
Invoke-RestMethod -Uri "$base/api/auth/register" -Method Post -Body $body -ContentType "application/json"

# Login (returns access + refresh tokens)
$body = @{ email = "admin@local"; password = "Str0ng-P@ss" } | ConvertTo-Json
Invoke-RestMethod -Uri "$base/api/auth/login" -Method Post -Body $body -ContentType "application/json"

# Test localization: en-US
Invoke-WebRequest -Uri "$base/api/auth/login" -Method Post `
    -Body (@{ email = ""; password = "" } | ConvertTo-Json) -ContentType "application/json" `
    -Headers @{ "Accept-Language" = "en-US" } -SkipHttpErrorCheck | Select-Object -ExpandProperty Content
# -> {"errors":{"Email":["Email is required."], ...}}
```

### Çalışan servisler

| Servis | URL / Port | Not |
|---|---|---|
| API | http://localhost:8080 | `/health` (Postgres probe dahil) |
| Scalar API docs | http://localhost:8080/scalar/v1 | OpenAPI tabanlı interaktif UI (dev only) |
| OpenAPI JSON | http://localhost:8080/openapi/v1.json | Postman/Insomnia/Bruno import için |
| MailHog UI | http://localhost:8025 | Dev SMTP catcher |
| PostgreSQL | `localhost:5432` | DBeaver/pgAdmin ile bağlan |
| MongoDB | `localhost:27017` | _(W4'te aktif olacak)_ |

### Sadece DB'leri çalıştır, API'yi local'den koş

```powershell
docker compose up -d postgres mongo mailhog
dotnet run --project src/SiteManagement.Api
# -> http://localhost:5200
```

### Test

```powershell
dotnet test
```

### Stack'i kapat

```powershell
docker compose down              # container'ları kapat, volume'ler durur
docker compose down --volumes    # volume'leri de sil (DB sıfırlanır)
```

---

## Production Deploy

> _Live URL **W6**'da eklenecek_ — kod ve dokümantasyon hazır, deploy tarafı vitrin polish'iyle (W6) birlikte yapılacak.

Adım adım Railway talimatı: **[docs/DEPLOY-RAILWAY.md](docs/DEPLOY-RAILWAY.md)**.

API kodunda iki platform helper var (Railway / Render / Heroku / Fly.io ile uyumlu):
- `PortBindingExtensions.UsePlatformPort()` — host'un inject ettiği `$PORT`'a otomatik bind
- `DatabaseUrlExtensions.UsePlatformDatabaseUrl()` — `postgresql://user:pass@host:port/db` formatlı `DATABASE_URL`'yi Npgsql syntax'ına otomatik çevirir

Local docker-compose'da bu env'ler set olmadığı için bu helper'lar no-op.

---

## CI & Test stratejisi

GitHub Actions ([`.github/workflows/ci.yml`](.github/workflows/ci.yml)):

- Her `main` push'unda ve PR'da koşar
- .NET 10 SDK kurar, Postgres 16 service container'ı ayağa kaldırır
- `dotnet restore` → `dotnet build --configuration Release` (warnings = errors, csproj'da aktif) → `dotnet test`
- Test sonuçları artifact olarak yüklenir

Test projeleri:

| Proje | Amaç |
|---|---|
| `SiteManagement.Domain.Tests` | Domain unit testleri (TDD ile yazılacak — W2'den itibaren aggregate invariant'ları) |
| `SiteManagement.Application.Tests` | Handler / pipeline behavior unit testleri (repo mock'lı, NSubstitute) |
| `SiteManagement.E2E.Tests` | TestContainers + WebApplicationFactory (W2 sonu altyapı, şimdilik pure helper testler) |
| **`SiteManagement.ArchitectureTests`** | NetArchTest ile katman bağımlılık koruması + CQRS naming + resource key bütünlüğü |

Architecture testleri proje sağlığının uzun vadeli garantörü:
- **Layer dependency:** Domain BCL-only, Application no-EF / no-ASP.NET, Infrastructure → Api referans yok
- **CQRS naming:** Her `IRequest<>` ya `Command` ya `Query` ile bitiyor; her command için handler **ve** validator var; handler'lar `sealed`
- **Resource integrity:** Her `ErrorMessageKeys` / `ValidationMessages` const'unun **hem tr hem en resx**'te karşılığı var; iki resx **drift yok**

---

## Lisans

MIT — bkz. [LICENSE](LICENSE).
