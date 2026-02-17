# Database Migration Scripts

This folder contains PowerShell scripts to manage database migrations manually.

## Scripts Overview

| Script | Purpose | Usage |
|--------|---------|-------|
| `0-initial-setup.ps1` | **First-time setup** - Creates and applies initial migration | `.\scripts\0-initial-setup.ps1` |
| `1-create-migration.ps1` | Create a new migration | `.\scripts\1-create-migration.ps1 -MigrationName "AddStudentGrades"` |
| `2-apply-migration.ps1` | Apply all pending migrations to database | `.\scripts\2-apply-migration.ps1` |
| `3-rollback-migration.ps1` | Rollback to a specific migration | `.\scripts\3-rollback-migration.ps1 -MigrationName "InitialCreate"` |
| `4-remove-last-migration.ps1` | Remove last migration (if not applied) | `.\scripts\4-remove-last-migration.ps1` |
| `5-list-migrations.ps1` | List all migrations | `.\scripts\5-list-migrations.ps1` |

## Quick Start

### First Time Setup

Run from the project root directory:

```powershell
.\scripts\0-initial-setup.ps1
```

This will:
1. Create the initial migration
2. Apply it to the database
3. Create all required tables

### Creating New Migrations

When you add or modify entities:

```powershell
# 1. Create migration
.\scripts\1-create-migration.ps1 -MigrationName "AddAttendanceFeature"

# 2. Apply to database
.\scripts\2-apply-migration.ps1
```

### Rolling Back Changes

```powershell
# Rollback to specific migration
.\scripts\3-rollback-migration.ps1 -MigrationName "InitialCreate"

# Rollback all migrations
.\scripts\3-rollback-migration.ps1 -MigrationName "0"
```

### Removing Mistakes

If you created a migration but haven't applied it yet:

```powershell
.\scripts\4-remove-last-migration.ps1
```

### Checking Migration Status

```powershell
.\scripts\5-list-migrations.ps1
```

## Prerequisites

1. **EF Core Tools** must be installed:
   ```powershell
   dotnet tool install --global dotnet-ef
   ```

2. **Database Connection String** must be configured in `src/SchoolManagement.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "server=localhost;port=3306;database=SchoolManagementDb;user=root;password=yourpassword"
     }
   }
   ```

3. **MySQL Server** must be running and accessible

## Troubleshooting

### "dotnet ef not found"
Install EF Core tools:
```powershell
dotnet tool install --global dotnet-ef
```

### "Unable to connect to database"
- Check if MySQL server is running
- Verify connection string in `appsettings.json`
- Ensure database user has proper permissions

### Migration already applied
If you need to remove an applied migration:
1. First rollback: `.\scripts\3-rollback-migration.ps1 -MigrationName "PreviousMigration"`
2. Then remove: `.\scripts\4-remove-last-migration.ps1`

## Important Notes

- Always run scripts from the **project root directory** (`school-management-be`)
- Migration files are stored in `src/SchoolManagement.Infrastructure/Migrations/`
- The project uses **MySQL** as the database provider
- All tables use **soft delete** (never hard delete records)
- Audit fields (`CreatedAt`, `UpdatedAt`) are automatically managed
