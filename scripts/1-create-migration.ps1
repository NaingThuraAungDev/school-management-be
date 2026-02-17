# Create a new database migration
# Usage: .\scripts\1-create-migration.ps1 -MigrationName "YourMigrationName"

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

Write-Host "Creating migration: $MigrationName" -ForegroundColor Cyan

$projectPath = ".\src\SchoolManagement.Infrastructure\"
$startupPath = ".\src\SchoolManagement.API\"

dotnet ef migrations add $MigrationName `
    --project $projectPath `
    --startup-project $startupPath `
    --verbose

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Migration '$MigrationName' created successfully!" -ForegroundColor Green
    Write-Host "Location: src\SchoolManagement.Infrastructure\Migrations\" -ForegroundColor Yellow
    Write-Host "Next step: Run .\scripts\2-apply-migration.ps1 to apply to database" -ForegroundColor Yellow
} else {
    Write-Host "✗ Failed to create migration" -ForegroundColor Red
    exit 1
}
