<#
.SYNOPSIS
    Generates a self-signed RSA certificate and exports it as a Base64-encoded PFX.

.DESCRIPTION
    This script is intended for use in Development and Staging environments ONLY.
    Do NOT use generated keys in Production.

    Produces:
      - <Kid>.pfx              — raw PFX file (gitignored via *.pfx)
      - <Kid>.pfx.base64.txt   — Base64-encoded PFX (for AppHost parameter jwt-signing-pfx)
      - <Kid>.public.pem       — RSA public key in PEM format (for Jwt:PublicKey)
      - <Kid>.jwt-settings.json — ready-to-use appsettings fragment

    All output goes to artifacts/jwt/ which is excluded by .gitignore (artifacts/).

.PARAMETER Kid
    Key ID (kid). Used as the filename prefix and as Jwt:Signing:ActiveKid.
    Defaults to "dev-<yyyyMMdd>".

.PARAMETER Password
    PFX password as SecureString. If omitted, a random password is generated automatically.

.PARAMETER Subject
    Certificate subject. Defaults to "CN=InterStyle JWT Signing".

.PARAMETER KeySize
    RSA key size in bits. Defaults to 2048.

.PARAMETER ValidYears
    Certificate validity period in years. Defaults to 3.

.PARAMETER OutputDirectory
    Output directory for generated files. Defaults to ".\artifacts\jwt".

.EXAMPLE
    # Auto-generate password
    pwsh .\scripts\security\New-JwtSigningPfx.ps1

.EXAMPLE
    # Custom kid and explicit password
    $pwd = Read-Host -Prompt "PFX password" -AsSecureString
    pwsh .\scripts\security\New-JwtSigningPfx.ps1 -Kid "staging-20260319" -Password $pwd
#>

[CmdletBinding()]
param(
    [string]$Kid = "dev-$(Get-Date -Format 'yyyyMMdd')",

    [SecureString]$Password,

    [string]$Subject = "CN=InterStyle JWT Signing",

    [int]$KeySize = 2048,

    [int]$ValidYears = 3,

    [string]$OutputDirectory = ".\artifacts\jwt"
)

$ErrorActionPreference = "Stop"

# ── Resolve password ──────────────────────────────────────────────────────────

$passwordGenerated = $false
if ($null -eq $Password -or $Password.Length -eq 0) {
    $rawPassword = [Guid]::NewGuid().ToString("N")
    $Password = ConvertTo-SecureString -String $rawPassword -AsPlainText -Force
    $passwordGenerated = $true
}

$plainPassword = [System.Net.NetworkCredential]::new("", $Password).Password

# ── Prepare output directory ──────────────────────────────────────────────────

if (-not (Test-Path $OutputDirectory)) {
    New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
}

# ── Generate RSA certificate ──────────────────────────────────────────────────

$rsa = [System.Security.Cryptography.RSA]::Create($KeySize)

$hashAlgorithm = [System.Security.Cryptography.HashAlgorithmName]::SHA256
$padding       = [System.Security.Cryptography.RSASignaturePadding]::Pkcs1

$dn      = [System.Security.Cryptography.X509Certificates.X500DistinguishedName]::new($Subject)
$request = [System.Security.Cryptography.X509Certificates.CertificateRequest]::new(
    $dn, $rsa, $hashAlgorithm, $padding
)

$keyUsage = [System.Security.Cryptography.X509Certificates.X509KeyUsageExtension]::new(
    [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::DigitalSignature,
    $true
)
$request.CertificateExtensions.Add($keyUsage)

$oids = [System.Security.Cryptography.OidCollection]::new()
$oids.Add([System.Security.Cryptography.Oid]::new("1.3.6.1.5.5.7.3.3")) | Out-Null
$eku = [System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension]::new($oids, $false)
$request.CertificateExtensions.Add($eku)

$bc = [System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension]::new($false, $false, 0, $true)
$request.CertificateExtensions.Add($bc)

$notBefore = [DateTimeOffset]::UtcNow.AddMinutes(-5)
$notAfter  = $notBefore.AddYears($ValidYears)

$cert = $request.CreateSelfSigned($notBefore, $notAfter)

try {
    # ── Export PFX ────────────────────────────────────────────────────────────

    $pfxBytes  = $cert.Export(
        [System.Security.Cryptography.X509Certificates.X509ContentType]::Pfx,
        $plainPassword
    )
    $pfxBase64 = [Convert]::ToBase64String($pfxBytes)

    # ── Export public key PEM ─────────────────────────────────────────────────
    # Build SubjectPublicKeyInfo DER from certificate public key data
    # (compatible with .NET Framework / PowerShell 5.1)

    $pubKeyDer = $cert.PublicKey.EncodedKeyValue.RawData

    # DER length encoder
    $derLength = {
        param([int]$n)
        if ($n -lt 128)  { return [byte[]]@($n) }
        if ($n -lt 256)  { return [byte[]]@(0x81, $n) }
        return [byte[]]@(0x82, [byte](($n -shr 8) -band 0xFF), [byte]($n -band 0xFF))
    }

    # BIT STRING: 0x00 (no unused bits) + PKCS#1 public key DER
    $bsPayload = [byte[]](,0x00) + $pubKeyDer
    $bitString = [byte[]](,0x03) + (& $derLength $bsPayload.Length) + $bsPayload

    # AlgorithmIdentifier for rsaEncryption (OID 1.2.840.113549.1.1.1 + NULL)
    $algId = [byte[]](0x30,0x0D, 0x06,0x09,0x2A,0x86,0x48,0x86,0xF7,0x0D,0x01,0x01,0x01, 0x05,0x00)

    # Outer SEQUENCE
    $spkiPayload = $algId + $bitString
    $spki = [byte[]](,0x30) + (& $derLength $spkiPayload.Length) + $spkiPayload

    $spkiBase64 = [Convert]::ToBase64String($spki, [Base64FormattingOptions]::InsertLineBreaks)
    $publicKeyPem = "-----BEGIN PUBLIC KEY-----`r`n$spkiBase64`r`n-----END PUBLIC KEY-----"

    # ── Write files ───────────────────────────────────────────────────────────

    $safeKid      = $Kid -replace '[^a-zA-Z0-9\-_\.]', '_'
    $pfxPath      = Join-Path $OutputDirectory "$safeKid.pfx"
    $base64Path   = Join-Path $OutputDirectory "$safeKid.pfx.base64.txt"
    $publicKeyPath= Join-Path $OutputDirectory "$safeKid.public.pem"
    $jsonPath     = Join-Path $OutputDirectory "$safeKid.jwt-settings.json"

    [System.IO.File]::WriteAllBytes($pfxPath, $pfxBytes)
    [System.IO.File]::WriteAllText($base64Path, $pfxBase64)
    [System.IO.File]::WriteAllText($publicKeyPath, $publicKeyPem)

    $json = [ordered]@{
        Jwt = [ordered]@{
            Signing = [ordered]@{
                ActiveKid = $Kid
                Keys = @(
                    [ordered]@{
                        Kid         = $Kid
                        PfxBase64   = $pfxBase64
                        PfxPassword = $plainPassword
                    }
                )
            }
            PublicKey = $publicKeyPem
        }
    } | ConvertTo-Json -Depth 8

    [System.IO.File]::WriteAllText($jsonPath, $json)

    # ── Summary ───────────────────────────────────────────────────────────────

    Write-Host ""
    Write-Host "[!] FOR DEVELOPMENT / STAGING USE ONLY -- do not use in Production." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Generated JWT signing certificate" -ForegroundColor Green
    Write-Host "---------------------------------"
    Write-Host "Kid:            $Kid"
    Write-Host "Valid until:    $($notAfter.ToString('yyyy-MM-dd'))"
    Write-Host "Key size:       $KeySize bits"
    Write-Host ""
    Write-Host "Output files:" -ForegroundColor Cyan
    Write-Host "  PFX:          $pfxPath"
    Write-Host "  Base64:       $base64Path"
    Write-Host "  Public PEM:   $publicKeyPath"
    Write-Host "  JSON snippet: $jsonPath"
    Write-Host ""

    if ($passwordGenerated) {
        Write-Host "Generated PFX password (save it now, it is not stored anywhere else):" -ForegroundColor Yellow
        Write-Host "  $plainPassword" -ForegroundColor Yellow
        Write-Host ""
    }

    Write-Host "AppHost parameter names:" -ForegroundColor Cyan
    Write-Host "  jwt-active-kid       = $Kid"
    Write-Host "  jwt-signing-pfx      = `<see $base64Path`>"
    Write-Host "  jwt-signing-password = `<see above or your SecureString`>"
    Write-Host ""
    Write-Host "Use 'dotnet user-secrets set' or AppHost Parameters to inject these values." -ForegroundColor DarkGray
}
catch {
    Write-Host "Error generating JWT certificate: $_" -ForegroundColor Red
    exit 1
}
finally {
    $cert.Dispose()
    $rsa.Dispose()

    # Clear plain-text password from memory
    $plainPassword = $null
}