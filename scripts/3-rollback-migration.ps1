# Rollback to a specific migration
# Usage: .\scripts\3-rollback-migration.ps1 -MigrationName "TargetMigrationName"
# Usage: .\scripts\3-rollback-migration.ps1 -MigrationName "0" (to rollback all)

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

Write-Host "Rolling back to migration: $MigrationName" -ForegroundColor Cyan

$projectPath = ".\src\SchoolManagement.Infrastructure\"
$startupPath = ".\src\SchoolManagement.API\"

dotnet ef database update $MigrationName `
    --project $projectPath `
    --startup-project $startupPath `
    --verbose

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Rollback completed successfully!" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to rollback migration" -ForegroundColor Red
    exit 1
}
