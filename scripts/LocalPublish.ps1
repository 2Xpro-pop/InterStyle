<#
.SYNOPSIS
    Loads secrets into environment variables and runs aspire publish.

.DESCRIPTION
    Reads the AppHost user-secrets (secrets.json) and sets each
    "Parameters:<name>" entry as $env:Parameters__<name> so that
    'dotnet run --publisher manifest' / 'aspire publish' can resolve them.

.PARAMETER UserSecretsId
    The UserSecretsId of InterStyle.AppHost.csproj.

.PARAMETER AspireArgs
    Additional arguments forwarded to 'aspire publish'.

.EXAMPLE
    pwsh .\scripts\LocalPublish.ps1
#>

[CmdletBinding()]
param(
    [string]$UserSecretsId = "6987ee0c-2889-4135-8cde-df0330f3804d",

    [bool]$UseContainerRegistry = $false,

    [Parameter(ValueFromRemainingArguments)]
    [string[]]$AspireArgs
)

$ErrorActionPreference = "Stop"

$secretsFile = Join-Path $env:APPDATA "Microsoft\UserSecrets\$UserSecretsId\secrets.json"

if (-not (Test-Path $secretsFile)) {
    Write-Host "secrets.json not found at $secretsFile" -ForegroundColor Red
    Write-Host "Run JwtSigningPfx.ps1 first, then populate secrets via 'dotnet user-secrets set'." -ForegroundColor Yellow
    exit 1
}

# ── Load secrets into environment ─────────────────────────────────────────────

$secrets = Get-Content $secretsFile -Raw -Encoding UTF8 | ConvertFrom-Json
$count = 0

foreach ($prop in $secrets.PSObject.Properties) {
    # Parameters:admin-password  ->  Parameters__admin-password
    $envName = $prop.Name -replace ':', '__'
    [System.Environment]::SetEnvironmentVariable($envName, $prop.Value, 'Process')
    $count++

    # Show what was set (mask long values)
    $display = $prop.Value
    if ($display.Length -gt 40) { $display = $display.Substring(0, 37) + "..." }
    Write-Host "  $envName = $display" -ForegroundColor DarkGray
}

[System.Environment]::SetEnvironmentVariable("use-container-registry", $UseContainerRegistry, 'Process')

Write-Host ""
Write-Host "Loaded $count secret(s) into environment." -ForegroundColor Green
Write-Host ""

# ── Run aspire deploy ────────────────────────────────────────────────────────

$appHostDir = Join-Path $PSScriptRoot "..\src\InterStyle.AppHost"

Write-Host "Running: aspire deploy $AspireArgs" -ForegroundColor Cyan
Write-Host "  Working directory: $appHostDir" -ForegroundColor DarkGray
Write-Host ""

Push-Location $appHostDir
try {
    aspire deploy @AspireArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Host "aspire deploy failed with exit code $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}
finally {
    Pop-Location
}
