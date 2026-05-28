<#
.SYNOPSIS
    Records the SiteManagement demo video end-to-end.

.DESCRIPTION
    Drives the showcase recording pipeline:
      1. Wipes Docker volumes + brings the stack up with DemoSeeder on so the UI
         opens against a clean, fully populated database.
      2. Waits for the API and the Angular dev server to be reachable.
      3. Walks the scripted scenario via Playwright (records a WebM + writes a
         scene-timing manifest).
      4. Renders one narration WAV per scene with piper-tts (open-source).
      5. Merges video + audio into docs/demo-video.mp4 with ffmpeg-static.

.NOTES
    Requires: Docker Desktop running, Node + npm + Python in PATH, and the
    Angular dev server (npm start in web/) already serving on :4200. The
    script does NOT start the dev server -- the developer keeps it running so
    rebuild output stays visible.
#>

[CmdletBinding()]
param(
    [switch]$SkipSeed,  # if set, assumes the stack is already seeded; useful for re-takes
    [switch]$RecordOnly # if set, runs only the Playwright recording (no TTS, no merge)
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

function Wait-Http {
    param(
        [Parameter(Mandatory)] [string] $Url,
        [int] $TimeoutSec = 90,
        [string] $Label = $Url
    )
    Write-Host "waiting for $Label..." -ForegroundColor Cyan
    $deadline = (Get-Date).AddSeconds($TimeoutSec)
    while ((Get-Date) -lt $deadline) {
        try {
            $r = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
            if ($r.StatusCode -lt 500) {
                Write-Host "  ok ($($r.StatusCode))" -ForegroundColor Green
                return
            }
        } catch {
            Start-Sleep -Seconds 1
        }
    }
    throw "Timed out waiting for $Label after $TimeoutSec s."
}

# --- prep: piper model (gitignored; pulled on first run) -------------------
$modelDir = Join-Path $repoRoot 'demo\models'
$modelPath = Join-Path $modelDir 'en_US-lessac-medium.onnx'
$configPath = "$modelPath.json"
$modelUrl = 'https://huggingface.co/rhasspy/piper-voices/resolve/main/en/en_US/lessac/medium/en_US-lessac-medium.onnx'
$configUrl = "$modelUrl.json"

if (-not (Test-Path $modelPath)) {
    Write-Host "=== downloading piper EN model (~60 MB, one-off) ===" -ForegroundColor Yellow
    New-Item -ItemType Directory -Force -Path $modelDir | Out-Null
    Invoke-WebRequest -Uri $modelUrl -OutFile $modelPath -UseBasicParsing
    Invoke-WebRequest -Uri $configUrl -OutFile $configPath -UseBasicParsing
}

if (-not $SkipSeed) {
    Write-Host "=== resetting demo stack ===" -ForegroundColor Yellow
    if (-not (Test-Path .env)) {
        Copy-Item .env.example .env
    }
    # Docker writes informational lines (Stopping/Started/...) to stderr;
    # under PowerShell 5.1 that becomes a NativeCommandError record even with
    # 2>$null. Wrapping in cmd /c hides stderr at the OS pipe level so the
    # parent PowerShell never sees it.
    cmd /c "docker compose down -v >nul 2>nul"
    if ($LASTEXITCODE -ne 0) { throw "docker compose down failed ($LASTEXITCODE)." }

    # BuildKit's "changes out of order" bug bites this repo intermittently;
    # fall back to the legacy builder for the demo recording (slower but
    # reliable).
    $env:DOCKER_BUILDKIT = '0'
    $env:COMPOSE_DOCKER_CLI_BUILD = '0'
    cmd /c "docker compose up -d --build api >nul 2>nul"
    if ($LASTEXITCODE -ne 0) { throw "docker compose up failed ($LASTEXITCODE)." }

    Wait-Http -Url 'http://localhost:8080/health' -TimeoutSec 120 -Label 'main API /health'
    Wait-Http -Url 'http://localhost:8025/api/v2/messages' -TimeoutSec 30 -Label 'MailHog API'
}

Wait-Http -Url 'http://localhost:4200' -TimeoutSec 90 -Label 'Angular dev server (npm start in web/)'

Write-Host "=== recording with Playwright ===" -ForegroundColor Yellow
$demoDir = Join-Path $repoRoot 'demo'
Set-Location $demoDir
try {
    npm run record
    if ($LASTEXITCODE -ne 0) {
        throw "Playwright run failed with exit code $LASTEXITCODE."
    }
} finally {
    Set-Location $repoRoot
}

if ($RecordOnly) {
    Write-Host "RecordOnly mode: skipping TTS + merge." -ForegroundColor DarkGray
    return
}

Write-Host "=== narrating scenes with piper-tts ===" -ForegroundColor Yellow
Set-Location $demoDir
try {
    npm run narrate
    if ($LASTEXITCODE -ne 0) {
        throw "Narration failed with exit code $LASTEXITCODE."
    }
} finally {
    Set-Location $repoRoot
}

Write-Host "=== merging with ffmpeg ===" -ForegroundColor Yellow
Set-Location $demoDir
try {
    # ffmpeg routes its progress log to stderr by design; under PowerShell 5.1
    # that's wrapped in NativeCommandError records even when the process
    # returns 0. cmd /c hides stderr at the OS level so only the real exit
    # code reaches us.
    cmd /c "npm run merge >nul 2>nul"
    if ($LASTEXITCODE -ne 0) {
        throw "Merge failed with exit code $LASTEXITCODE."
    }
} finally {
    Set-Location $repoRoot
}

$out = Join-Path $repoRoot 'docs\demo-video.mp4'
if (Test-Path $out) {
    Write-Host ""
    Write-Host "[OK] demo video ready: $out" -ForegroundColor Green
} else {
    throw "Merge reported success but $out does not exist."
}
