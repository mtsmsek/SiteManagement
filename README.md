# SiteManagement

> Patika bitirme projesinin DDD + TDD ile modern bir yorumu. Site (apartman kompleksi) yönetim sistemi: admin daire/blok yönetimi, sakin kaydı, aidat & fatura dağıtımı; resident kendi borçlarını görüp kredi kartıyla öder. Ödeme tarafı ayrı bir microservice.

🚧 **Work in progress** — şu an Hafta 1 (Foundation & Deploy).

## Tech Stack

- **Backend:** .NET 10, ASP.NET Core, EF Core, PostgreSQL 16, MediatR, FluentValidation, Serilog, ASP.NET Identity + JWT
- **Payment Service:** ayrı .NET Web API + MongoDB 7
- **Frontend:** Angular (W2'de eklenecek)
- **Mimari:** DDD (rich domain) + Clean Architecture + Modüler Monolit + CQRS-lite
- **Test:** xUnit + FluentAssertions + NSubstitute + TestContainers (TDD Domain'de zorunlu)
- **Infra:** Docker Compose, GitHub Actions, Railway

Detaylı plan: [ROADMAP.md](ROADMAP.md) · Gün gün ilk iki hafta: [WEEK-1-2-DETAIL.md](WEEK-1-2-DETAIL.md)

## Local Setup

> _W1 Gün 2'de docker-compose eklenecek, talimatlar buraya gelecek._

```powershell
dotnet build
dotnet test
```

## Lisans

MIT — bkz. [LICENSE](LICENSE).
