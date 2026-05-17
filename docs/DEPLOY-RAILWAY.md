# Railway Deploy — SiteManagement

Adım adım talimat. Railway hesabı ücretsiz açılır ve ayda **$5 kredi** verir (kart bilgisi vermeden). Bu vitrin projesi için fazlasıyla yeterli, kredi biterse servis durur — otomatik para çekilmez.

> Tahmini süre: **15–20 dakika** (ilk seferde).

---

## 1. Railway hesabı + GitHub bağlantısı

1. https://railway.app aç → **Login with GitHub**.
2. Verification e-mail'ini onayla.
3. **New Project** → **Deploy from GitHub repo** → `mtsmsek/SiteManagement` seç.
4. Railway repo'yu okur, `Dockerfile`'ı bulur ve `Deploy` der. İlk build 3-5 dakika sürer.

---

## 2. PostgreSQL servisi ekle

Railway projesi içinde:

1. **+ New** → **Database** → **PostgreSQL** (PostgreSQL 16 default gelir).
2. Provision tamamlandığında Railway otomatik olarak `DATABASE_URL` env değişkenini API servisine bağlamaz — manuel yapacağız.

### `DATABASE_URL`'yi API servisine bağla

1. API servisine tıkla → **Variables** sekmesi.
2. **+ New Variable** → **Add Reference** → `Postgres.DATABASE_URL` seç.
3. Variable adı `DATABASE_URL` olarak kalsın.

API kodu (`DatabaseUrlExtensions.UsePlatformDatabaseUrl`) bu env'i otomatik tanır ve Npgsql formatına çevirir.

---

## 3. JWT secret + diğer env değişkenleri

API servisinin **Variables** sekmesinde aşağıdakileri ekle:

| Variable | Değer (öneri) |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `Jwt__Key` | **En az 32 karakter, rastgele üretilmiş** (örn. PowerShell: `[Convert]::ToBase64String((1..32 \| ForEach-Object { Get-Random -Maximum 256 }))`) |
| `Jwt__Issuer` | `SiteManagement` |
| `Jwt__Audience` | `SiteManagement.Clients` |
| `Jwt__AccessTokenMinutes` | `60` |
| `Jwt__RefreshTokenDays` | `14` |

> **Üst seviye not:** `__` (double underscore) ASP.NET Core'da config section separator olarak kullanılır — yani `Jwt__Key` = `Jwt:Key` config path'ine bağlanır.

---

## 4. Public domain ayarla

1. API servisinin **Settings** sekmesi → **Networking** → **Generate Domain**.
2. Railway sana `xxxxx.up.railway.app` formatında bir URL verir.

API otomatik olarak `$PORT` env değişkenine bind eder (`PortBindingExtensions.UsePlatformPort`). Sen ekstra port ayarı yapmana gerek yok.

---

## 5. İlk deploy'un doğrulanması

Deploy tamamlanınca:

```powershell
# Sağlık probe'u (Postgres dahil)
curl https://<your-app>.up.railway.app/health
# -> Healthy

# Register
$body = @{ email = "admin@yourdomain.com"; password = "Str0ng-P@ss-123"; fullName = "Admin" } | ConvertTo-Json
Invoke-RestMethod -Uri "https://<your-app>.up.railway.app/api/auth/register" `
    -Method Post -Body $body -ContentType "application/json"

# Login
$loginBody = @{ email = "admin@yourdomain.com"; password = "Str0ng-P@ss-123" } | ConvertTo-Json
Invoke-RestMethod -Uri "https://<your-app>.up.railway.app/api/auth/login" `
    -Method Post -Body $loginBody -ContentType "application/json"
```

Production'da Scalar UI (`/scalar/v1`) **kapalı** (sadece `IsDevelopment()`'ta açık). İhtiyaç olursa W6 polish'inde Production'da da açacağız.

---

## 6. Sürekli deploy

Railway her `main` push'una otomatik deploy eder. CI workflow'u (`.github/workflows/ci.yml`) build + test'i yeşile çıkardıktan sonra push edersin, Railway yeni image'ı kendisi build edip dağıtır.

Eğer "main'i CI yeşil olunca otomatik deploy etme" istiyorsan: Railway → Service → Settings → **Source** → **Deploy on push** opsiyonu zaten varsayılan olarak açık.

---

## 7. Sorun giderme

### Build başarısız
- Railway build loglarında ".NET 10 SDK" image'ı bulunamadıysa: Dockerfile zaten `mcr.microsoft.com/dotnet/sdk:10.0` kullanıyor, Railway image'a erişiyor. Genelde geçici GHCR/Docker Hub rate-limit hatası, **Redeploy** butonu çözer.

### `/health` 500 dönüyor
- API başladı ama Postgres'e bağlanamıyor. **Variables**'ta `DATABASE_URL` referansının doğru servise bağlı olduğunu kontrol et.
- API loglarında "connection refused" varsa: Railway internal networking'i bekle (PostgreSQL servisi başlangıçta 30-60 sn sürebilir).

### Migration çalışmadı, tablolar yok
- API loglarında `DatabaseInitializer` log'larına bak. `Applying EF Core migrations...` → `Database initialization complete.` olmalı.
- Yoksa: `Jwt__Key` veya `DATABASE_URL` eksik olabilir; uygulama startup'ta crash etmiş olabilir.

### `Jwt__Key` çok kısa hatası
- En az 32 karakter olmalı (HS256 SHA-256 için). Yukarıdaki PowerShell snippet'i base64-encoded 32 random byte üretir.

---

## 8. Maliyet uyarısı

Railway'in **hobby plan**'ı ayda **$5 kredi** verir (kart vermeden). Bu vitrin projesi için yeterli ama:

- API uyumadığı için sürekli kredi yer
- Eğer uzun süre dokunmayacaksan **Service → Settings → Pause** ile dondur
- Resource limits'i Railway dashboard'unda izle

Daha agresif ücretsiz tier istiyorsan alternatif: **Render.com** — kredi kartı vermeden serbest, ama API 15 dk inaktivitede uyur (ilk request'i 30-60 sn sürer).
