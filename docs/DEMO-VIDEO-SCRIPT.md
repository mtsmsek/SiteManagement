# Demo Video Çekim Senaryosu (2-3 dk, TR)

> 1080p ekran kaydı; ekstra mikrofonla TR seslendirme (tercihen). MP4 olarak çıkar — GIF değil, dosya boyutu daha küçük + kalite daha iyi.
> Çekim öncesi: tarayıcıyı incognito'da temizle, tab'ları sadece gerekenler aç, sistem bildirimlerini kapat.

---

## Hazırlık (kayıt başlamadan önce)

```powershell
# 1) Sıfırdan demo veri
docker compose down -v
Copy-Item .env.example .env -Force  # DEMO_SEED_ON_STARTUP=true zaten açık
docker compose up -d --build
# ~30sn bekle; api healthy olsun
curl http://localhost:8080/health

# 2) Angular dev server
cd web; npm start
# http://localhost:4200 hazır
```

**Açılan tab'lar:**
1. `http://localhost:4200/login` — Angular UI
2. `http://localhost:8025` — MailHog (welcome mailleri için)
3. `http://localhost:8080/scalar/v1` — API docs (kısa flash için)

---

## Sahne planı

### 0:00–0:15 — Açılış kartı

- Project name: **SiteManagement**
- One-liner: "Apartman sitesi yönetim platformu — DDD + Clean Architecture + .NET 10 + Angular 21 + ödeme microservice + SignalR real-time"
- Repo URL küçük altta

### 0:15–0:50 — Resident portal

1. Login sayfası → demo resident mail'i MailHog'dan al → şifreyi kopyala
2. `selin.yilmaz@demo.local` ile giriş → **/resident/dashboard** açılsın
3. Tile'ları göster: **750 ₺ açık borç** + **2 ödenmemiş kalem** + **0 kredi** + **1 okunmamış mesaj**
4. "Borçlarım" → tabloda 2 unpaid + 1 paid kalem
5. Bir unpaid kalemde "Kartla Öde" → CardPaymentDialog açılır
6. Test kartını yapıştır: `4242 4242 4242 4242`, CVV `123`, Ay `12`, Yıl `2030`
7. **Öde** → snackbar "Ödendi" → tabloda kalem **Paid** olur
8. **Kasıtlı decline:** yetersiz bakiye senaryosu için CVV `999` veya yanlış kartla tekrar dene → **kırmızı animasyonlu snackbar** "Ödeme reddedildi" + kalem `Unpaid` kalır

### 0:50–1:20 — Real-time messaging (SignalR vurgusu)

1. Sol panelde **Mesajlar** → "Hoş geldiniz" thread'i (admin'in açtığı, unread badge'i sayfa açılınca temizlenir)
2. Yeni bir tab'da admin login (`admin@sitemanagement.local` / `Str0ng-P@ss-Dev`)
3. Admin → **Mesajlar** sayfası → sol inbox'ta "Selin Yılmaz – Hoş geldiniz" → seç
4. **Resident tab'ını yan yana koy** (split screen veya iki monitör)
5. Admin'den yanıt yaz "Aidat hatırlatması: 500 TL açık borcunuz var" → **Gönder**
6. **Resident ekranında mesaj polling olmadan anında belirsin** — SignalR push'unun vitrini bu an
7. Resident yanıtla "Yarın ödeyeceğim" → admin ekranında anında görünür

### 1:20–1:55 — Admin tarafı

1. Admin → **Yönetim Paneli** → tile'lar: 1 site / 3 sakin / **750 + 500 + 200 tahakkuk** / **750 tahsil** / kalan açık bakiye / 0 kredi / **tahsilat oranı**
2. **Siteler** → "Lavanta Konutları" → blok A → 3 daire (her biri Occupied)
3. **Sakinler** → mat-table 3 satır (Selin, Mert, Ezgi)
4. **Faturalandırma** → 2026-05 Aidat dönemi → "Kalemler" → 3 item (1 paid + 2 unpaid)

### 1:55–2:20 — Tema + dil + (opsiyonel) Scalar

1. Üst toolbar'dan ay/güneş ikon → **dark mode** flip; sayfa tutarlı dark görünür
2. Dil ikonu → **EN** seç → menü "Sites / Residents / Messages" olur (i18n parity vurgusu)
3. (Opsiyonel) Scalar tab → `/scalar/v1` açıp 1-2 endpoint görseli — OpenAPI tabanlı interaktif docs

### 2:20–2:50 — Bitiş kartı

- Repo URL büyük
- "MIT lisanslı, açık kaynak"
- Hashtag chip'leri: `#dotnet #angular #ddd #cleanarchitecture #signalr #microservices`

---

## Tek-cümlelik anahtar mesajlar (seslendirme için)

- Açılışta: "Domain-Driven Design ile yazılmış, ödeme tarafı ayrı microservice'te koşan ve real-time mesajlaşmaya SignalR ekleyen tam stack bir site yönetim platformu."
- Pay-by-card sırasında: "Ödeme, ana API'den ayrı bir microservice'e Refit + Polly ile gidiyor — idempotency key sayesinde retry güvenli."
- Decline'da: "Red karar bir HTTP hatası değil, 200 + Failed payload; consumer-side contract test bunu doğruluyor. Kalem `Unpaid` kalıyor."
- Messaging anında belirme anı: "SignalR push'u — durable mail Outbox'tan gidiyor, ephemeral UI hint SignalR'dan. İkisi farklı garanti seviyeleri."
- Admin dashboard: "Read-side projeksiyon, AsNoTracking; domain entity sızmıyor."
- Tema/dil: "Tema runtime'da değişir; her i18n key build-time parity guardrail testi geçer."

---

## Çekim sonrası

- Video başına 5 sn açılış kartı + sonuna 5 sn kapanış kartı koymak için Davinci Resolve / Shotcut / iMovie yeterli — yazılar simple, kayan animasyon yok
- Final dosya: `docs/demo-video.mp4` veya YouTube'a unlisted yükle, link'i README'ye + LinkedIn postuna ekle
- Screenshot'lar için bir-iki kareyi `docs/screenshots/` altına PNG olarak da kaydet (README pitch bölümüne 2-3 görsel için)
