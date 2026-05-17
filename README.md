# SiteManagement

> Patika bitirme projesinin DDD + TDD ile modern bir yorumu. Site (apartman kompleksi) yönetim sistemi: admin daire/blok yönetimi, sakin kaydı, aidat & fatura dağıtımı; resident kendi borçlarını görüp kredi kartıyla öder. Ödeme tarafı ayrı bir microservice.

🚧 **Work in progress** — şu an Hafta 1 (Foundation & Deploy).

## Tech Stack

- **Backend:** .NET 10, ASP.NET Core, EF Core 10, PostgreSQL 16, MediatR, FluentValidation, Serilog, ASP.NET Identity + JWT
- **Payment Service:** ayrı .NET Web API + MongoDB 7
- **Frontend:** Angular (W2'de eklenecek)
- **Mimari:** DDD (rich domain) + Clean Architecture + Modüler Monolit + CQRS-lite
- **Test:** xUnit + FluentAssertions + NSubstitute + TestContainers (TDD Domain'de zorunlu)
- **Infra:** Docker Compose, GitHub Actions, Railway

Detaylı plan: [ROADMAP.md](ROADMAP.md) · Gün gün ilk iki hafta: [WEEK-1-2-DETAIL.md](WEEK-1-2-DETAIL.md)

## Local Setup

> Sıfırdan kurulum (yeni bir makinede): [docs/SETUP-MACHINE.md](docs/SETUP-MACHINE.md) — .NET SDK, Docker Desktop, WSL2, git/gh, opsiyonel araçlar dahil tüm adımlar.

### Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Compose v2) + WSL2 (Windows)
- _Önerilen:_ JetBrains Rider / Visual Studio 2022 17.13+ / VS Code + C# Dev Kit

### Hızlı başlangıç (tek komut, sıfırdan)

```powershell
# 1) Environment dosyasını oluştur (varsayılan dev credential'larıyla yeterli)
cp .env.example .env

# 2) Tüm stack'i ayağa kaldır (postgres + mongo + mailhog + api)
docker compose up -d --build

# 3) Hazır olmalarını bekle ve health endpoint'ini doğrula
curl http://localhost:8080/health
# -> { "status": "ok" }
```

Servisler:

| Servis | URL / Port | Not |
|---|---|---|
| API | http://localhost:8080 | `/health` smoke endpoint |
| Scalar (API docs UI) | http://localhost:8080/scalar/v1 | OpenAPI tabanlı interaktif dokümantasyon (dev) |
| OpenAPI JSON | http://localhost:8080/openapi/v1.json | Postman/Insomnia/Bruno import için |
| MailHog UI | http://localhost:8025 | Yakalanan dev mailleri burada |
| PostgreSQL | `localhost:5432` | DBeaver/pgAdmin ile bağlan |
| MongoDB | `localhost:27017` | MongoDB Compass ile bağlan (W4'te aktif) |

### Sadece dependency'leri çalıştır, API'yi local'den koş

```powershell
docker compose up -d postgres mongo mailhog
dotnet run --project src/SiteManagement.Api
# API artık http://localhost:5200 üzerinde
```

### Test'leri koş

```powershell
dotnet test
```

### Stack'i kapat

```powershell
docker compose down            # container'ları kapat, volume'ler durur
docker compose down --volumes  # volume'leri de sil (DB sıfırlanır)
```

## Lisans

MIT — bkz. [LICENSE](LICENSE).
