<#
.SYNOPSIS
    Publicacion de QuitSmoke en Google Play (Constitucion §6, §7, §8).

.DESCRIPTION
    Sube el AAB firmado a Google Play Console usando fastlane `supply`.
    Por defecto publica en el canal de PRUEBAS INTERNAS, conforme a la regla
    obligatoria de la Constitucion §8 ("publicar siempre primero en pruebas
    internas"). El paso a produccion requiere completar la lista de verificacion
    del Criterio de Entrega (§12).

.PARAMETER Aab
    Ruta al AAB firmado generado por build.ps1.

.PARAMETER Track
    Canal de Play: internal | alpha | beta | production. Por defecto: internal.

.PARAMETER ServiceAccountJson
    Ruta al JSON de la cuenta de servicio de Play (gitignored, §5).

.PARAMETER PackageName
    Application Id. Por defecto: com.socratic.quitsmoke.

.EXAMPLE
    ./scripts/publish.ps1 -Aab bin/Release/net9.0-android/publish/com.socratic.quitsmoke-Signed.aab
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Aab,
    [ValidateSet("internal","alpha","beta","production")]
    [string]$Track = "internal",
    [string]$ServiceAccountJson = "$PSScriptRoot/../play-service-account.json",
    [string]$PackageName = "com.socratic.quitsmoke"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $Aab)) { throw "No se encuentra el AAB: $Aab. Ejecuta antes scripts/build.ps1" }
if (-not (Test-Path $ServiceAccountJson)) {
    throw "No se encuentra la cuenta de servicio: $ServiceAccountJson (Constitucion §5: nunca versionar; ver Plan de Contingencia §11)"
}
if (-not (Get-Command fastlane -ErrorAction SilentlyContinue)) {
    throw "fastlane no esta instalado. Instala con: gem install fastlane"
}

if ($Track -eq "production") {
    Write-Warning "Vas a publicar en PRODUCCION. Confirma que se cumple el Criterio de Entrega (Constitucion §12):"
    Write-Warning "  - Metadata en todos los idiomas soportados, permisos justificados, CHANGELOG y versionCode incrementado."
    $confirm = Read-Host "Escribe 'PRODUCCION' para continuar"
    if ($confirm -ne "PRODUCCION") { throw "Publicacion cancelada por el usuario." }
}

Write-Host "==> Subiendo $Aab al canal '$Track' de $PackageName..." -ForegroundColor Cyan
fastlane supply `
    --aab $Aab `
    --track $Track `
    --package_name $PackageName `
    --json_key $ServiceAccountJson `
    --release_status "completed"

Write-Host "==> Subida completada. Verifica en Play Console (Constitucion §7, paso 6)." -ForegroundColor Green
