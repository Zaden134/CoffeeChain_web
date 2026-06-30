param(
    [Parameter(Mandatory = $true)]
    [string]$BackupFile,
    [string]$ConnectionString = "Host=localhost;Port=5432;Database=coffee_chain_management;Username=postgres;Password=postgres",
    [string]$PostgresBin = "",
    [switch]$Clean
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $BackupFile)) {
    throw "Backup file was not found: $BackupFile"
}

$pgRestore = if ($PostgresBin) { Join-Path $PostgresBin "pg_restore.exe" } else { "pg_restore" }
$args = @("--dbname", $ConnectionString)

if ($Clean) {
    $args += "--clean"
    $args += "--if-exists"
}

$args += $BackupFile
& $pgRestore @args

Write-Host "Restore completed from: $BackupFile"
