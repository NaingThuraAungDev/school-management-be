# Apply all pending migrations to the database
# Usage: .\scripts\2-apply-migration.ps1

Write-Host "Applying database migrations..." -ForegroundColor Cyan

$projectPath = ".\src\SchoolManagement.Infrastructure\"
$startupPath = ".\src\SchoolManagement.API\"

dotnet ef database update `
    --project $projectPath `
    --startup-project $startupPath `
    --verbose

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Migrations applied successfully!" -ForegroundColor Green
    Write-Host "Database is now up to date" -ForegroundColor Yellow
} else {
    Write-Host "✗ Failed to apply migrations" -ForegroundColor Red
    Write-Host "Check your connection string in appsettings.json" -ForegroundColor Yellow
    exit 1
}
