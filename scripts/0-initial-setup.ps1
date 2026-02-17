# Initial database setup - Creates initial migration and applies it
# Usage: .\scripts\0-initial-setup.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "School Management Database Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = ".\src\SchoolManagement.Infrastructure\"
$startupPath = ".\src\SchoolManagement.API\"

# Step 1: Check if migrations already exist
Write-Host "[1/3] Checking for existing migrations..." -ForegroundColor Yellow
$migrationsPath = Join-Path $projectPath "Migrations"
if (Test-Path $migrationsPath) {
    $migrationFiles = Get-ChildItem -Path $migrationsPath -Filter "*.cs"
    if ($migrationFiles.Count -gt 0) {
        Write-Host "⚠ Migrations already exist. Use other scripts to manage them." -ForegroundColor Yellow
        Write-Host "Run .\scripts\5-list-migrations.ps1 to see existing migrations" -ForegroundColor Yellow
        exit 0
    }
}

# Step 2: Create initial migration
Write-Host "[2/3] Creating initial migration..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate `
    --project $projectPath `
    --startup-project $startupPath `
    --verbose

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to create initial migration" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Initial migration created" -ForegroundColor Green
Write-Host ""

# Step 3: Apply migration to database
Write-Host "[3/3] Applying migration to database..." -ForegroundColor Yellow
dotnet ef database update `
    --project $projectPath `
    --startup-project $startupPath `
    --verbose

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to apply migration" -ForegroundColor Red
    Write-Host "Please check your database connection string in appsettings.json" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "✓ Database setup completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "All tables have been created in your database." -ForegroundColor Yellow
Write-Host "You can now run the application with: dotnet run --project .\src\SchoolManagement.API\" -ForegroundColor Yellow
