<#
.SYNOPSIS
    Compilacion y firmado reproducibles de QuitSmoke (APK + AAB).
    Cumple la Constitucion del proyecto (§6, §7): genera artefactos firmados de
    forma reproducible a partir del codigo fuente.

.DESCRIPTION
    Limpia, restaura y publica el proyecto Android en configuracion Release,
    firmando con el keystore local. La contrasena se lee del fichero local
    (gitignored) para no exponer secretos en el repositorio (§5).

.PARAMETER Keystore
    Ruta al keystore. Por defecto: quitsmoke.keystore en la raiz.

.PARAMETER KeyAlias
    Alias de la clave dentro del keystore.

.PARAMETER PasswordFile
    Fichero que contiene la contrasena del keystore (gitignored).

.EXAMPLE
    ./scripts/build.ps1 -KeyAlias quitsmoke
#>
[CmdletBinding()]
param(
    [string]$Keystore     = "$PSScriptRoot/../quitsmoke.keystore",
    [string]$KeyAlias      = "quitsmoke",
    [string]$PasswordFile  = "$PSScriptRoot/../keystore.password.txt",
    [string]$Framework     = "net9.0-android",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root    = Resolve-Path "$PSScriptRoot/.."
$csproj  = Join-Path $root "QuitSmoke.csproj"

if (-not (Test-Path $Keystore))    { throw "No se encuentra el keystore: $Keystore" }
if (-not (Test-Path $PasswordFile)){ throw "No se encuentra el fichero de contrasena: $PasswordFile (ver Constitucion §5)" }

$password = (Get-Content $PasswordFile -Raw).Trim()

Write-Host "==> Limpiando bin/obj (Constitucion §11)..." -ForegroundColor Cyan
Get-ChildItem $root -Include bin,obj -Recurse -Directory -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -notmatch 'constitution' } |
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "==> dotnet restore..." -ForegroundColor Cyan
dotnet restore $csproj

# Propiedades comunes de firma
$signArgs = @(
    "-c", $Configuration,
    "-f", $Framework,
    "/p:AndroidKeyStore=true",
    "/p:AndroidSigningKeyStore=$Keystore",
    "/p:AndroidSigningKeyAlias=$KeyAlias",
    "/p:AndroidSigningKeyPass=$password",
    "/p:AndroidSigningStorePass=$password"
)

Write-Host "==> Generando AAB firmado..." -ForegroundColor Cyan
dotnet publish $csproj @signArgs /p:AndroidPackageFormat=aab

Write-Host "==> Generando APK firmado..." -ForegroundColor Cyan
dotnet publish $csproj @signArgs /p:AndroidPackageFormat=apk

$outDir = Join-Path $root "bin/$Configuration/$Framework/publish"
Write-Host ""
Write-Host "==> Artefactos generados en: $outDir" -ForegroundColor Green
Get-ChildItem $outDir -Include *.aab,*.apk -Recurse -ErrorAction SilentlyContinue |
    ForEach-Object { Write-Host "   - $($_.FullName)" }
