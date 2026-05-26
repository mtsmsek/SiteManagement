# Hafta 5 — Resident Portal + Messaging + Reports (Detaylı Plan)

> **Hedef:** "Sakin kendi borcunu görüp **kendi** kalemini kartla öder (IDOR
> korumalı); admin ↔ sakin mesajlaşır; admin ve sakin kendi dashboard'larına
> sahip; borç-alacak raporu çıkar." Bu, W4'te admin'e bırakılan PDF yarısını
> tamamlar ve ROADMAP W5'i (Messaging & Reports) kapatır.
>
> **Definition of Done:** Sakin login → "borçlarım" → **kendi** kalemini öder
> (başka sakinin kalemine erişemez — IDOR guardrail testiyle kanıtlı); admin ve
> sakin mesajlaşabilir; admin dashboard (site borç/alacak özeti) + sakin
> dashboard (kendi borcu + okunmamış mesaj) çalışır; her iki solution'ın testleri
> + yeni E2E'ler yeşil.

W1-W4 desenini izler. **Yeni bir solution YOK** — Messaging ana API'nin içinde
yeni bir bounded context (Domain → Application → Infrastructure → Api), resident
portal ise mevcut Identity/Billing/Tenancy altyapısının üstüne **resident-scoped**
endpoint'ler + Angular `/resident/*` alanı.

---

## Başlangıç durumu (W5'e girerken zaten yerinde olanlar)

Self-review + keşif sonucu — bunları **yeniden kurmuyoruz**, üstüne biniyoruz:

| Zaten var | Nerede | W5'e etkisi |
|---|---|---|
| **Resident login hesabı** | `RegisterResidentCommandHandler` → `UserAuthService.RegisterResidentUserAsync` AppUser yaratır, **"Resident" rolü** atar, şifreyi welcome-mail ile yollar (in-transaction) | Sakin zaten login olabiliyor; portal eksik |
| **AppUser ↔ Resident bağı** | `AppUser.ResidentId` (FK → `Resident.Id`), token'a **`resident_id` claim**'i olarak basılıyor | Resident endpoint'leri join'siz "ben kimim" çözer |
| **Roller** | `Domain/Identity/Roles.cs` (`Admin`, `Resident`, stabil GUID'ler), `IdentitySeeder` startup'ta seed eder | Yeni rol gerekmez |
| **Current user portu** | `ICurrentUser` → `UserId`, **`ResidentId`**, `IsInRole(role)` | IDOR korumasının temeli budur |
| **Sakin borç sorgusu** | `IBillingQueries.ListResidentBillsAsync(residentId)` + `ResidentBillDto`; billing item'lar zaten `ResidentId` ile keyli (join gerekmez) | "Borçlarım" read tarafı hazır |
| **Pay-by-card altyapısı** | `IPaymentGateway` + `PayDuesItem`/`PayUtilityItem` handler'ları (idempotency key item bazlı) | Resident ödeme = aynı zincir + ownership guard |
| **FE auth state** | `auth.models.ts` `Roles.resident` + `CurrentUser.residentId`; guard yorumu: "resident portal lands in a later week" | `isResident()` + `residentGuard` eklenecek |

> **Önemli mevcut detay:** `ResidentBillsController` var ama `[Authorize(Roles=Admin)]`
> ve `residentId`'yi **route'tan** alıyor (`/residents/{residentId}/bills`). Bu
> admin içindir. Sakin için **route'tan residentId ALMAYAN**, kimliği yalnızca
> token'dan (`ICurrentUser.ResidentId`) çözen ayrı endpoint'ler kuracağız — IDOR
> bunun üstünde kapanır.

---

## Açık tasarım soruları (Gün 1'de prose ile **birlikte** karar) 

Author tercihi: temel kararları feature yığmadan önce konuşup oturtmak, her
konvansiyonu guardrail test'le kilitlemek. Aşağıda her soru için **önerim** var
ama plan onları kilitlemez.

1. **IDOR koruma deseni.** Sakin yalnızca kendi kaynağına erişsin diye:
   - **(A) Token-scoped endpoint'ler — önerilen.** Resident endpoint'lerinde
     route'ta `residentId` **yok**; handler `ICurrentUser.ResidentId`'yi kullanır.
     IDOR yapısal olarak imkânsız (öğrenilecek id yok). Item ödemede ayrıca
     "bu item gerçekten bu resident'ın mı?" kontrolü.
   - (B) Resource-based authorization handler (`IAuthorizationHandler`) — route'taki
     id ile token'ı karşılaştırır. Daha "enterprise" ama daha çok hareketli parça.
   - **Guardrail:** Resident A, Resident B'nin item'ını okuyamaz/ödeyemez → **403/404**
     E2E testi (her iki yön için).

2. **Resident endpoint yüzeyi.** `/api/me/*` ailesi mi (`GET /api/me/bills`,
   `POST /api/me/bills/{itemId}/pay-by-card`, `GET /api/me/conversations` …),
   `[Authorize(Roles=Resident)]` ve hep token-scoped? → **Öneri: evet, `me`
   controller ailesi.** Admin tarafı mevcut controller'larda kalır.

3. **Ödeme handler'ı paylaşımı.** Resident ödeme = mevcut `PayDuesItem`/
   `PayUtilityItem` zincirini **ownership guard** ile mi tekrar kullanalım, yoksa
   ayrı `PayMyItem` command'ı mı? → **Öneri:** komut aynı kalsın ama "çağıran bu
   item'ın sahibi mi" kontrolü ekleyelim; resident `me` endpoint'i residentId'yi
   token'dan geçirir, admin endpoint'i route'tan.

4. **Messaging aggregate sınırı.** `Conversation` (bir resident'a ait bir konu/
   thread) + içinde `Message`'lar mı? Resident başına tek "destek" thread'i mi,
   yoksa konu başlıklı çok thread mi? Kim başlatır? Okundu bilgisi (unread) nasıl?
   → **Öneri:** `Conversation` aggregate = `{ ResidentId, Subject, Messages[] }`;
   hem admin hem resident `Message` ekler; mesajda `SenderRole` + `ReadAt`;
   resident yalnız kendi conversation'ını görür. Çok-thread (konu başlıklı) basit
   tutulur.

5. **Real-time mi polling mi?** SignalR compose'da **yok** (roadmap). → **Öneri:**
   W5 **polling/refresh** ile gider (unread count endpoint'i); SignalR W6/roadmap.
   Planı SignalR'a bağımlı kurmayalım.

6. **Raporlar read-model.** Borç-alacak raporu + dashboard'lar saf **read-side**
   (query/projection, `AsNoTracking`, DTO). Yeni query servisleri mi, mevcutların
   kompozisyonu mu? → **Öneri:** admin dashboard büyük ölçüde mevcut
   `GetSiteDebtSummaryAsync` + küçük eklemeler; resident dashboard = `ListResidentBills`
   + unread count kompozisyonu. Gereken yerde yeni `IReportQueries`.

---

## Gün 1 — Temel kararlar (prose) + iki hijyen düzeltmesi

**Hedef:** W5'in backbone'unu oturtmak + W5 boyunca E2E'yi kirletecek/eksik
guardrail'leri baştan kapatmak (yeni E2E yığmadan önce).

- [ ] Yukarıdaki **6 açık soruyu** prose ile karara bağla; kararları bu dosyaya
      "Mimari Kararlar (W5)" tablosu olarak işle.
- [x] **Hijyen 1 — E2E ↔ compose DB izolasyonu (CLAUDE.md açık maddesi).** ✅
      Kök sebep: connection string `AddPersistence`'te **eager** okunuyordu (Program
      Build'den önce çağırır) → factory'nin InMemory Testcontainer override'ı çok
      geç geliyor, E2E `localhost:5432` (compose) DB'sine bağlanıp truncate +
      `admin@sitemanagement.local` siliyordu. Fix: connection string'i `AddDbContext`
      lambda'sında **lazy** (`sp.GetRequiredService<IConfiguration>()`) oku. Guardrail:
      `ConnectionStringIsolationTests` — resolved `AppDbContext` fixture container'ını
      kullanıyor. Doğrulandı: E2E sonrası compose admin + demo veri yerinde.
- [x] **Hijyen 2 — PaymentService architecture test.** ✅ `PaymentService.ArchitectureTests`
      (NetArchTest, 4 test): Domain BCL-only (Mongo/EF/ASP yok), Application → Infra/Api'ye
      bağlı değil, Application → Mongo/ASP yok, Infra → Api yok. slnx'e eklendi; yeşil.
      Domain saflığı artık örtük değil, açık guardrail. (Author'ın "konvansiyon ⇒ guardrail" kuralı.)

**Commit:** `test(payment): layer-dependency guardrails` + `test(e2e): isolate from compose db` (+ karar notları)

---

## Gün 2 — Resident Portal Backend (IDOR-safe "me" endpoint'leri)

**Hedef:** Sakin kendi borcunu görür ve **kendi** kalemini öder; başkasınınkine
erişemez.

- [ ] `GET /api/me/bills` — `[Authorize(Roles=Resident)]`; `ICurrentUser.ResidentId`
      → `ListResidentBillsAsync`. Route'ta id **yok**.
- [ ] `GET /api/me/summary` (opsiyonel) — kendi toplam borcu + kredisi.
- [ ] `POST /api/me/bills/{itemId}/pay-by-card` — ownership guard'lı ödeme: item
      gerçekten çağıranın mı? değilse **403/404** (bilgi sızdırma yok). Başarılıysa
      mevcut pay zinciri (charge first → `Paid`), decline → 402.
- [ ] Ownership kontrolü için: item → residentId eşleşmesi (billing item zaten
      `ResidentId` taşıyor) `ICurrentUser.ResidentId` ile karşılaştırılır.
- [ ] **TDD:** handler unit'leri (kendi item'ı / başkasının item'ı / decline);
      `me` controller authz.
- [ ] **IDOR guardrail E2E:** Resident A login → B'nin item'ını oku/öde → 403/404;
      kendi item'ını öde → 204/Paid.

**Commit:** `feat(api): resident self-service bills + pay own item (IDOR-guarded)`

---

## Gün 3 — Messaging Domain (TDD)

**Hedef:** Rich `Conversation` aggregate; admin ↔ sakin mesajlaşma invariant'ları.

- [ ] `Domain/Messaging/Conversation : AggregateRoot` — `{ ResidentId, Subject,
      Messages (iç entity koleksiyonu), CreatedAt }`; `Start(...)`,
      `PostMessage(senderUserId, senderRole, body)`, `MarkRead(byRole, atUtc)`.
- [ ] `Message` iç entity / value: `{ Id, SenderUserId, SenderRole, Body, SentAtUtc,
      ReadAtUtc? }`; body uzunluk limiti `MessagingLimits`.
- [ ] Invariant'lar: boş body yok; conversation bir resident'a aittir; mesaj
      sırası zaman damgalı.
- [ ] Exceptions: `EmptyMessageException` / `MessageTooLongException` (+ MessageKey,
      tr/en resx).
- [ ] **TDD:** `ConversationTests` (start, post, read, invariant'lar), limit'ler.

**Commit:** `feat(messaging): conversation aggregate (TDD)`

---

## Gün 4 — Messaging Application + Infrastructure + API

**Hedef:** Mesajlaşma uçtan uca; hem admin hem resident tarafı.

**Application (CQRS-lite):**
- [ ] Commands: `StartConversationCommand` (admin veya resident başlatabilir —
      karara göre), `PostMessageCommand`, `MarkConversationReadCommand`
      (+ her biri için FluentValidation validator).
- [ ] Queries (`IMessagingQueries`): admin → tüm conversation'lar; resident →
      **yalnız kendi** conversation'ları; mesaj listesi; **unread count**.
- [ ] DTO'lar (projection, domain entity dönmez).

**Infrastructure:**
- [ ] EF Core config + migration (conversation + message tabloları); index'ler
      (ResidentId, CreatedAt).
- [ ] Repository + query implementasyonu (cross-context'e dikkat — W3'teki
      translatable-LINQ dersi).

**API:**
- [ ] Admin: `GET/POST /api/conversations*` (`[Authorize(Roles=Admin)]`).
- [ ] Resident: `GET/POST /api/me/conversations*` (token-scoped, IDOR-safe).
- [ ] **E2E:** admin sakine yazar → sakin görür + cevaplar → admin görür; sakin
      başkasının conversation'ına erişemez (IDOR).

**Commit:** `feat(messaging): app + infra + api (admin & resident, IDOR-safe)`

---

## Gün 5 — Angular Resident Portal

**Hedef:** Sakin kendi alanında borcunu görür/öder ve mesajlaşır.

- [ ] `npm run gen:api` (yeni `me/*` + messaging endpoint'leri).
- [ ] Routing: `/resident/*` + **`residentGuard`** (`auth.isResident()` computed —
      `auth.service.ts`'e ekle); login sonrası rol'e göre yönlendirme
      (admin → `/admin`, resident → `/resident`).
- [ ] Resident layout + sayfalar: **Borçlarım** (liste + kartla öde dialog'unu
      yeniden kullan), **Mesajlar** (thread + cevap), **Dashboard** (Gün 6).
- [ ] Ödeme akışı: mevcut `CardPaymentDialog` + animasyonlu hata snackbar'ı tekrar
      kullan; `me/bills/{id}/pay-by-card`.
- [ ] i18n `resident.*` + `messaging.*` (tr+en, **iki dosyaya da**).
- [ ] Vitest: `residentGuard`, resident store(lar), payment akışı.

**Commit:** `feat(web): resident portal — my bills, pay, messaging`

---

## Gün 6 — Raporlar + Dashboard'lar

**Hedef:** Admin borç-alacak görünürlüğü + iki taraflı dashboard.

**Backend (read-side):**
- [ ] `IReportQueries` (gerekirse) — admin: site(ler) borç/alacak özeti, tahsil
      oranı, açık dönemler; mevcut `GetSiteDebtSummaryAsync` üstüne.
- [ ] Resident dashboard verisi = `ListResidentBills` + toplam açık borç + kredi +
      unread mesaj sayısı kompozisyonu.

**Frontend:**
- [ ] **Admin dashboard:** site özetleri (tahakkuk/tahsil/bakiye/kredi), tahsil
      oranı, kısa "son hareketler".
- [ ] **Resident dashboard:** açık borç toplamı + kalemler kısa görünüm + okunmamış
      mesaj rozeti.
- [ ] (Opsiyonel) basit grafik/progress görselleştirme; aşırıya kaçma.

**Commit:** `feat(reports): admin debt/credit report + admin & resident dashboards`

---

## Gün 7 — Self-Review + Doc + All-Green

- [ ] **Self-review:** IDOR her resident endpoint'inde token-scoped mu (route'tan
      id sızmıyor mu)? Messaging resident yalnız kendininkini mi görüyor? Reports
      saf read-side mi (domain entity dönmüyor, `AsNoTracking`)? Yeni command'ların
      hepsinde validator var mı (architecture test yeşil)? Localize edilmemiş
      string yok mu?
- [ ] Architecture/guardrail testleri (her iki solution) yeşil; IDOR + messaging
      E2E'leri yeşil; web Vitest yeşil.
- [ ] **Manuel uçtan uca:** sakin login → borcunu öder, başkasınınkini göremez;
      admin ↔ sakin mesajlaşır; dashboard'lar dolu.
- [ ] `WEEK-5-DETAIL.md` + `ROADMAP.md` W5 ✅ + `README.md` + `CLAUDE.md` status.

**Commit:** `docs: close out W5` (+ self-review düzeltmeleri)

---

## Hafta 5 Çıktısı (Definition of Done)

- [ ] Sakin login → kendi borcunu görür ve **kendi** kalemini kartla öder
- [ ] **IDOR korumalı:** sakin başka sakinin borcunu/mesajını okuyamaz/ödeyemez
      (her iki yön için guardrail E2E yeşil)
- [ ] Admin ↔ sakin mesajlaşması uçtan uca çalışır (unread dahil)
- [ ] Admin dashboard (borç-alacak/tahsil) + resident dashboard (kendi borcu +
      okunmamış mesaj) çalışır
- [ ] Borç-alacak raporu read-side projeksiyonla; domain entity sızmaz
- [ ] Her iki solution'ın testleri + yeni E2E/Vitest'ler yeşil; CI yeşil
- [ ] **Hijyen kapandı:** E2E↔compose izolasyonu + PaymentService architecture test

### Riskler / kapsam kontrolü
- **IDOR en kritik madde:** "test ettim" demek yetmez; her resident endpoint'i
  için negatif (başkasının kaynağı → 403/404) testi **şart**. Token-scoped desen
  (route'ta id yok) bunu yapısal kılar.
- **Messaging scope creep:** ek/dosya, bildirim, real-time → **W5 dışı.** W5 =
  metin thread + unread + polling. SignalR roadmap/W6.
- **Dashboard scope creep:** ağır BI/grafik kütüphanesi yok; mevcut query'lerin
  kompozisyonu + sade görsel. Vitrin için "temiz ve dolu" yeter.
- **Credit partial settlement** hâlâ **proje sonuna ertelendi** (kredi item'ı tam
  karşılamazsa tüketilmiyor); W5 kapsamında değil — `CLAUDE.md`'de açık madde.
- **Resident self-registration YOK:** sakin hesabı yalnız admin'in kaydıyla doğar
  (mevcut güvenlik duruşu korunur); W5 bunu değiştirmez.
