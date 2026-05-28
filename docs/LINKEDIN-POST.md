# LinkedIn Post Taslağı (TR)

> Üç versiyon hazırladım — kısa (300 kelime altı, akış için), orta (yarım sayfa, en uzun okuyucuyu kaybetmeden detay), uzun (mülakat öncesi paylaşım için). Birini seç, ekran görüntülerini ve repo URL'ini yerleştir.

---

## Versiyon A — Kısa (~150 kelime)

🚀 Patika bitirme projemi vitrin haline getirdiğim **SiteManagement** açık kaynak oldu.

Apartman sitesi yönetim platformu: admin daire/blok/sakin yönetimi + aidat ve fatura dağıtımı, sakin login → kendi borçlarını kartla öder. Mesajlaşma SignalR ile real-time.

**Öne çıkan tasarım kararları:**
🔹 **DDD + Clean Architecture** — rich domain model (anti-anemic); aggregate'lar invariant'larını kendi tutar
🔹 **Modular monolith + ayrı Payment microservice** (PostgreSQL + MongoDB polyglot persistence; Refit + Polly resilience)
🔹 **Authz pipeline + token-scoped /api/me/*** — IDOR yapısal olarak imkânsız (her iki yön E2E ile kanıtlı)
🔹 **Outbox + SignalR ikili** — durable async delivery için Outbox, ephemeral real-time için SignalR
🔹 **10 ADR** + 90% line coverage + 222 domain + 89 application + 37 E2E + 32 web test

Tech: .NET 10 · ASP.NET Core · Angular 21 (signals) · PostgreSQL 16 · MongoDB 7 · MediatR · Refit + Polly · Serilog · Docker · GitHub Actions

🔗 GitHub: https://github.com/mtsmsek/SiteManagement
🎥 Demo: [video linki]

#dotnet #angular #ddd #cleanarchitecture #signalr #microservices

---

## Versiyon B — Orta (~300 kelime)

🚀 Patika bitirme projemi 6 haftalık disiplinli bir vitrin haline getirdim: **SiteManagement** artık açık kaynak.

**Ne yapıyor?** İki rollü site (apartman kompleksi) yönetim platformu. Admin daire/blok yönetimi, sakin kaydı, aidat ve fatura dağıtımı yapıyor; sakin login olup kendi borçlarını görüyor ve kredi kartıyla ödüyor. Mesajlaşma real-time.

**Mimari nasıl?**
🔹 **Modüler monolith** (modular = içeride bounded context'ler birbirine ref vermez; monolith = tek deployment) + **ayrı Payment microservice** (MongoDB; polyglot persistence demosu)
🔹 **Rich Domain Model** — `apartment.MarkAsOccupied()`, `duesPeriod.Close()`, `conversation.MarkRead()`. Setter'lar private; invariant'lar aggregate'in içinde
🔹 **CQRS-lite + MediatR pipeline** — Validation → Authorization → ResourceOwnership → ExceptionTranslation → Transaction → handler. Cross-cutting yok handler'larda
🔹 **Authorization yapısal:** her request tam bir `IAdminRequest`/`IResidentRequest`/`IPublicRequest` marker'ı; arch test "authz unutmak = build hatası" yapar. **IDOR yapısal olarak imkânsız** — token-scoped `/api/me/*`, route'ta id yok
🔹 **Outbox pattern** — `IDomainEvent` in-transaction, `IIntegrationEvent` after-commit. Welcome mail outbox'tan; pay-by-card senkron HTTP (Refit + Polly + idempotency key)
🔹 **SignalR real-time messaging** — durable kanal Outbox, ephemeral UI hint SignalR; farklı garanti seviyeleri
🔹 **10 ADR** (`docs/adr/`) — her karar MADR formatında: bağlam / karar / alternatifler / sonuçlar

**Test stratejisi:** Honeycomb / Testing Trophy — 222 domain (TDD) + 89 application + 18 architecture guardrail + 37 E2E (Testcontainers + WireMock) + 32 web (Vitest). Coverage: **Line 90.2% / Branch 82.3% / Method 84.9%**.

**Stack:** .NET 10 · ASP.NET Core (controllers + MediatR) · Angular 21 (standalone, signals) · PostgreSQL 16 · MongoDB 7 · Refit + Polly · SignalR · Docker Compose · GitHub Actions CI

Repo: https://github.com/mtsmsek/SiteManagement
Demo (2 dk): [video linki]
Mimari kararlar: https://github.com/mtsmsek/SiteManagement/tree/main/docs/adr

#dotnet #aspnetcore #angular #ddd #cleanarchitecture #cqrs #signalr #microservices #openSource

---

## Versiyon C — Uzun (mülakat öncesi paylaşım, ~500 kelime)

> Versiyon B'yi al, başlangıçta + her "Mimari" maddesi altında 1-2 cümlelik **niye seçildi** ile zenginleştir:
>
> - "Senior bir geliştirici yerine geçecek bir vitrin için, sadece 'çalışan kod' yetmez — 'bu kararı niye verdi' okunaklı olmalı. Onun için her mimari karar ayrı bir ADR olarak yazıldı."
> - "DDD'yi tercih etmek bir lüks değildi: domain yeterince zengin (aidat dağıtımı, kredi balance, soft delete root-only, audit, IDOR). Anemic model burada hızla rotaya çıkardı."
> - "Modular monolith — 'her şey tek yerde' DEĞİL. Tek deployment + içeride bağımsız modüller. Bounded context'ler birbirine using vermez, sadece event üzerinden konuşabilir. Yarın bir context'i ayırmak refactor değil, packaging."
> - "Authz'u pipeline'a almak ve marker'ları arch test'le kilitlemek — IDOR'un 'unutuldu' olmasını yapısal olarak imkânsız kılıyor. Mülakat sorusu: 'IDOR'a karşı nasıl koruyorsun?' Cevap: 'Route'ta id taşımam.'"

---

## Görsel hazırlık

LinkedIn postuna 4 görsel ekle (slayt formatı):

1. **Bounded context map** (README'deki mermaid'i screenshot'la)
2. **Resident dashboard** — borç tile'ları + tahsilat
3. **Pay-by-card dialog + animasyonlu hata snackbar** (decline anı)
4. **Admin messaging** — iki panel inbox/thread, unread badge

Veya: 1080×1080 tek bir kolaj.

---

## Yayınlama notu

- Linkedi cuma sabah 09:00-10:00 arası önerilen — TR auditörüne dev/eng toplulukları aktif
- Yorumlara erken cevap ver: ilk saatte gelen etkileşim algoritmik olarak büyütür
- "Bir özelliği nasıl yaptığını anlatır mısın?" sorusu gelirse direkt ilgili ADR'ı linkle
