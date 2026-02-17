# Remove the last migration (only if not applied to database)
# Usage: .\scripts\4-remove-last-migration.ps1

Write-Host "Removing last migration..." -ForegroundColor Cyan
Write-Host "WARNING: Only works if migration hasn't been applied to database!" -ForegroundColor Yellow

$projectPath = ".\src\SchoolManagement.Infrastructure\"
$startupPath = ".\src\SchoolManagement.API\"

$confirmation = Read-Host "Are you sure you want to remove the last migration? (yes/no)"
if ($confirmation -ne "yes") {
    Write-Host "Operation cancelled" -ForegroundColor Yellow
    exit 0
}

dotnet ef migrations remove `
    --project $projectPath `
    --startup-project $startupPath `
    --verbose

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Last migration removed successfully!" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to remove migration" -ForegroundColor Red
    Write-Host "If the migration was already applied, use rollback instead" -ForegroundColor Yellow
    exit 1
}
