param(
    [string]$ConnectionString = "Host=localhost;Port=5432;Database=coffee_chain_management;Username=postgres;Password=postgres",
    [string]$OutputDirectory = "backups",
    [string]$PostgresBin = ""
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $OutputDirectory)) {
    New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
}

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$outputPath = Join-Path $OutputDirectory "coffee-chain-$timestamp.dump"
$pgDump = if ($PostgresBin) { Join-Path $PostgresBin "pg_dump.exe" } else { "pg_dump" }

& $pgDump --dbname $ConnectionString --format custom --file $outputPath

Write-Host "Backup created: $outputPath"
