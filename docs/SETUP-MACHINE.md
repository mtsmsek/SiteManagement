# PC Setup — SiteManagement Geliştirme Ortamı

> Yeni bir Windows makinesinde repo'yu sıfırdan çalıştırmak için izlenecek adımlar. Linux/macOS için tools aynı, kurulum komutları farklı (her bölümde notlandı).

Hedef: Bu rehber bittiğinde
- `dotnet build` çalışıyor
- `docker compose up -d --build` ile tüm stack ayağa kalkıyor
- `curl http://localhost:8080/health` → `{"status":"ok"}`

---

## 1. .NET 10 SDK

İhtiyaç: **.NET 10.0.201** veya üstü (en güncel `dotnet --list-sdks` ile doğrula).

### Windows

```powershell
winget install Microsoft.DotNet.SDK.10 --accept-source-agreements --accept-package-agreements
```

Veya elden: https://dotnet.microsoft.com/download/dotnet/10.0 → "SDK x64 Installer".

Doğrulama:

```powershell
dotnet --version          # 10.0.201 veya üstü olmalı
dotnet --list-sdks        # 10.x.x görünmeli
```

> **macOS:** `brew install --cask dotnet-sdk` ya da Microsoft sayfasından `.pkg`.
> **Linux (Ubuntu/Debian):** Microsoft'un paket reposundan `apt install dotnet-sdk-10.0`.

---

## 2. Docker Desktop + WSL2 (Windows için)

### a) WSL2 kurulumu (Windows 10/11)

PowerShell'i **yönetici** olarak aç:

```powershell
wsl --install
```

Bu komut:
- Virtual Machine Platform ve WSL özelliğini aktive eder
- WSL2 kernel'ini indirir
- Default `Ubuntu` distrosu ekler

Kurulum sonrası **bilgisayarı yeniden başlat**. Tekrar açtığında Ubuntu ilk kez başlatılırsa username/password isteyebilir — istediğini yaz, bu projede kullanılmayacak.

Doğrulama:

```powershell
wsl --status              # Default Version: 2 olmalı
wsl --list --verbose      # En az bir distro `STATE: Running` veya `Stopped` olmalı
```

### b) Docker Desktop

```powershell
winget install Docker.DockerDesktop --accept-source-agreements --accept-package-agreements
```

Veya elden: https://www.docker.com/products/docker-desktop/

Kurulum sonrası Docker Desktop'ı aç:
- Lisans sözleşmesini kabul et (kişisel/eğitim/küçük şirket için **ücretsiz**)
- Settings → "Use WSL 2 based engine" işaretli olmalı (default)
- Sol altta **"Engine running"** yazana kadar bekle (1-2 dakika)

Doğrulama:

```powershell
docker version --format '{{.Server.Version}}'   # 29.x veya üstü olmalı
```

> **macOS:** `brew install --cask docker` veya https://www.docker.com/products/docker-desktop/
> **Linux:** Docker Desktop yerine doğrudan `docker-ce` paketi tercih edilir. Distro'ya göre Docker'ın resmi kurulum kılavuzunu izle: https://docs.docker.com/engine/install/

---

## 3. Git + GitHub auth

### Git

```powershell
winget install Git.Git --accept-source-agreements --accept-package-agreements
```

Identity ayarı (tek seferlik):

```powershell
git config --global user.name "Mehmet Şimşek"
git config --global user.email "mtsmsek@gmail.com"
```

### GitHub authentication

İki seçenek — biri yeterli:

**Seçenek A — GitHub CLI (önerilen):**

```powershell
winget install GitHub.cli --accept-source-agreements --accept-package-agreements
gh auth login          # https → github.com → browser → kodu yapıştır
```

**Seçenek B — Git Credential Manager (Git for Windows ile beraber gelir):**

İlk `git push` çağrısında otomatik browser açılır, GitHub login yapınca kredential cache'lenir.

---

## 4. Repo clone + çalıştırma

```powershell
# Tercihen C:\Users\<you>\source\repos altına
git clone https://github.com/mtsmsek/SiteManagement.git
cd SiteManagement

# .env dosyasını oluştur (varsayılan dev credential'ları yeterli)
Copy-Item .env.example .env

# Tüm stack'i ayağa kaldır
docker compose up -d --build

# Smoke test
curl http://localhost:8080/health
# -> {"status":"ok"}
```

Açılan portlar:

| Servis | URL |
|---|---|
| API | http://localhost:8080 |
| Scalar (API docs UI) | http://localhost:8080/scalar/v1 |
| OpenAPI JSON | http://localhost:8080/openapi/v1.json |
| MailHog UI | http://localhost:8025 |
| PostgreSQL | `localhost:5432` |
| MongoDB | `localhost:27017` |

---

## 5. Opsiyonel araçlar (geliştirme rahatlığı)

| Araç | Amaç | Kurulum |
|---|---|---|
| **JetBrains Rider** | .NET IDE (önerilen) | https://www.jetbrains.com/rider/ — eğitim/açık kaynak lisansı ücretsiz |
| **Visual Studio 2022 17.13+** | Alternatif .NET IDE | https://visualstudio.microsoft.com/ — Community sürümü ücretsiz |
| **VS Code + C# Dev Kit** | Hafif alternatif | `winget install Microsoft.VisualStudioCode` + extension marketplace'ten C# Dev Kit |
| **DBeaver Community** | Postgres GUI | `winget install dbeaver.dbeaver` veya https://dbeaver.io/ |
| **MongoDB Compass** | Mongo GUI | https://www.mongodb.com/products/compass |
| **Bruno** veya **Insomnia** | HTTP istemcisi (Postman alternatifi) | `winget install Bruno.Bruno` |
| **Node.js LTS** | W2'den itibaren Angular için | `winget install OpenJS.NodeJS.LTS` |

---

## 6. Sorun giderme

### "Docker Desktop is unable to start" / "WSL needs updating"
- WSL kurulu mu kontrol et: `wsl --status`. `wsl --install` çalıştır + restart.
- Restart sonrası Docker Desktop'ı tekrar aç.

### `dotnet restore` private feed'e takılıyor
- Repo köküne `NuGet.config` koyduk; global feed'ler bypass ediliyor. Hala takılıyorsa: `dotnet nuget locals all --clear` koş, sonra `dotnet restore` tekrar dene.

### `docker compose up` Postgres healthcheck'te takılıyor
- Volume'ü temizle: `docker compose down --volumes` sonra `docker compose up -d --build`.
- 5432 portu zaten kullanımdaysa (local Postgres servisi varsa): `.env`'de `POSTGRES_PORT=5433` yap.

### Port çakışmaları
- `.env`'de istediğin portları değiştir: `API_PORT`, `POSTGRES_PORT`, `MONGO_PORT`, `MAILHOG_UI_PORT`.

### Test'leri koşmak istiyorsun
```powershell
dotnet test
```

---

## 7. Hızlı komut özeti

```powershell
# Stack'i ayağa kaldır
docker compose up -d --build

# Logları izle
docker compose logs -f api
docker compose logs -f postgres

# Stack'i kapat
docker compose down              # container'ları kapat, volume'leri sakla
docker compose down --volumes    # volume'leri de sil (DB sıfırla)

# Sadece API'yi rebuild + restart
docker compose up -d --build api

# Local'de API koş (Docker yerine), DB container'da
docker compose up -d postgres mongo mailhog
dotnet run --project src/SiteManagement.Api
# -> http://localhost:5200

# Test koş
dotnet test

# Yeni migration ekle (W1 Gün 3'ten itibaren)
dotnet ef migrations add <MigrationName> -p src/SiteManagement.Infrastructure -s src/SiteManagement.Api
dotnet ef database update -p src/SiteManagement.Infrastructure -s src/SiteManagement.Api
```
