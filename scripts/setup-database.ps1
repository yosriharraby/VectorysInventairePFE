# Applique les migrations EF sur SQL Express et affiche l'instruction SSMS.
$ErrorActionPreference = "Stop"
$env:ASPNETCORE_ENVIRONMENT = "Development"

$apiDir = Join-Path $PSScriptRoot "..\src\VectorysInventaire.API"
$infraDir = Join-Path $PSScriptRoot "..\src\VectorysInventaire.Infrastructure"
$server = "YOSRI\SQLEXPRESS"

Write-Host "=== VectorysInventaire — configuration base de donnees ===" -ForegroundColor Cyan
Write-Host ""

Push-Location $apiDir
try {
    Write-Host "Application des migrations EF Core sur $server ..." -ForegroundColor Yellow
    dotnet ef database update --project $infraDir
    if ($LASTEXITCODE -ne 0) { throw "dotnet ef database update a echoue." }

    Write-Host ""
    Write-Host "Demarrage API pour seed des utilisateurs (quelques secondes)..." -ForegroundColor Yellow
    $job = Start-Job -ScriptBlock {
        param($dir)
        Set-Location $dir
        $env:ASPNETCORE_ENVIRONMENT = "Development"
        dotnet run --launch-profile http --no-build 2>&1
    } -ArgumentList $apiDir

    Start-Sleep -Seconds 12
    Stop-Job $job -ErrorAction SilentlyContinue
    Remove-Job $job -Force -ErrorAction SilentlyContinue
    Get-Process -Name "VectorysInventaire.API","dotnet" -ErrorAction SilentlyContinue |
        Where-Object { $_.Path -like "*VectorysInventaire*" } |
        Stop-Process -Force -ErrorAction SilentlyContinue
}
finally {
    Pop-Location
}

Write-Host ""
Write-Host "Verification sur $server ..." -ForegroundColor Yellow

$query = @"
USE VectorysDB;
SELECT @@SERVERNAME AS Serveur, DB_NAME() AS Base;
SELECT name AS TableName FROM sys.tables ORDER BY name;
SELECT COUNT(*) AS Utilisateurs FROM Utilisateurs;
SELECT COUNT(*) AS Inventaires FROM Inventaires;
"@

sqlcmd -S $server -Q $query -W

Write-Host ""
Write-Host "=== SSMS : connexion ===" -ForegroundColor Green
Write-Host "  Nom du serveur : YOSRI\SQLEXPRESS"
Write-Host "  Authentification : Windows"
Write-Host "  Base             : VectorysDB"
Write-Host ""
