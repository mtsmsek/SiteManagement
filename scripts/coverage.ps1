<#
.SYNOPSIS
    Runs the main solution's tests with Coverlet and renders an HTML report.

.DESCRIPTION
    One-shot local convenience: cleans previous output, runs `dotnet test` with
    the shared `coverlet.runsettings` collector config, then uses ReportGenerator
    (installed on demand as a global tool) to produce coverage/index.html.
    CI builds its own report via the GitHub Actions workflow.

.NOTES
    Requires Docker to be running (E2E tests use Testcontainers).
#>

[CmdletBinding()]
param(
    [string]$Solution = 'SiteManagement.slnx'
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

# Wipe previous output so stale data can't leak into the new report.
Remove-Item -Recurse -Force coverage, TestResults -ErrorAction Ignore | Out-Null

Write-Host "Running tests with coverage..." -ForegroundColor Cyan
dotnet test $Solution `
    --nologo -m:1 `
    --settings coverlet.runsettings `
    --results-directory TestResults

# Idempotent: install the global tool only if it isn't already installed.
$tool = dotnet tool list -g | Select-String 'dotnet-reportgenerator-globaltool'
if (-not $tool) {
    Write-Host "Installing dotnet-reportgenerator-globaltool..." -ForegroundColor Cyan
    dotnet tool install -g dotnet-reportgenerator-globaltool | Out-Null
}

Write-Host "Rendering HTML report..." -ForegroundColor Cyan
reportgenerator `
    "-reports:TestResults/**/coverage.cobertura.xml" `
    "-targetdir:coverage" `
    "-reporttypes:Html;TextSummary"

if (Test-Path coverage/Summary.txt) {
    Write-Host ""
    Get-Content coverage/Summary.txt | Select-Object -First 30
}

Write-Host ""
Write-Host "HTML report ready: coverage/index.html" -ForegroundColor Green
