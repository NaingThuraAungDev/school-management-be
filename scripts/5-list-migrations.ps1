# List all migrations
# Usage: .\scripts\5-list-migrations.ps1

Write-Host "Listing all migrations..." -ForegroundColor Cyan

$projectPath = ".\src\SchoolManagement.Infrastructure\"
$startupPath = ".\src\SchoolManagement.API\"

dotnet ef migrations list `
    --project $projectPath `
    --startup-project $startupPath

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✓ Migration list complete" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to list migrations" -ForegroundColor Red
    exit 1
}
