# School Management System - Backend

A comprehensive school management system built with ASP.NET Core, following Clean Architecture principles with CQRS pattern.

## Quick Links

� **[Frontend Integration Guide](./FRONTEND_INTEGRATION_GUIDE.md)** - **START HERE** for frontend developers  
📚 **[Complete API Documentation](./API_DOCUMENTATION.md)** - Detailed documentation for all API endpoints  
⚡ **[Quick Reference](./API_QUICK_REFERENCE.md)** - Concise API reference with code examples  
💾 **[TypeScript Types](./api-types.ts)** - Type definitions for frontend integration  
📮 **[Postman Collection](./School-Management-API.postman_collection.json)** - Import into Postman for testing

## Features

- **Student Management**: Admission, profile management, guardians, documents
- **Staff Management**: Onboarding, roles, teacher assignments
- **Class Management**: Classes, sections, class-sections combinations
- **Subject Management**: Subject creation and teacher-subject mapping
- **Exam Management**: Exam terms, grade definitions, exams, results, report cards
- **Timetable Management**: Time slots, timetable entries for classes and teachers
- **Promotions**: Bulk student promotion with preview

## Architecture

- **Clean Architecture** with strict dependency flow
- **CQRS** pattern via MediatR
- **Record-based** commands and queries
- **Automatic validation** using FluentValidation
- **JWT** authentication with role-based access control
- **Soft delete** for all entities
- **Automatic audit** fields (CreatedAt, UpdatedAt)

## Technology Stack

- ASP.NET Core 8.0
- Entity Framework Core
- MySQL Database
- MediatR (CQRS)
- FluentValidation
- ASP.NET Identity
- JWT Authentication
- Serilog
- Swagger/OpenAPI

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- MySQL Server
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd school-management-be
   ```

2. **Configure Database**
   
   Update `appsettings.json` with your MySQL connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=SchoolManagement;User=root;Password=yourpassword;"
   }
   ```

3. **Run Initial Setup**
   ```powershell
   .\scripts\0-initial-setup.ps1
   ```

4. **Run the Application**
   ```bash
   dotnet run --project .\src\SchoolManagement.API\
   ```

5. **Access Swagger UI**
   
   Navigate to: `https://localhost:{port}/swagger`

### Database Migrations

```powershell
# Create migration
.\scripts\1-create-migration.ps1 MigrationName

# Apply migrations
.\scripts\2-apply-migration.ps1

# Rollback migration
.\scripts\3-rollback-migration.ps1

# Remove last migration
.\scripts\4-remove-last-migration.ps1

# List migrations
.\scripts\5-list-migrations.ps1
```

## API Documentation

### For Frontend Developers

- **[API_DOCUMENTATION.md](./API_DOCUMENTATION.md)** - Complete API reference with all endpoints, request/response examples, and data models
- **[API_QUICK_REFERENCE.md](./API_QUICK_REFERENCE.md)** - Quick reference with TypeScript/React examples
- **[Postman Collection](./School-Management-API.postman_collection.json)** - Pre-configured API requests for testing

### Base URL

```
Development: https://localhost:{port}/api
```

### Authentication

All endpoints (except `/auth/login`) require JWT authentication:

```http
Authorization: Bearer {your-jwt-token}
```

### Default Credentials

After running the initial setup, use these credentials:

```
SuperAdmin: admin@school.com / Admin@123
```

## Project Structure

```
src/
├── SchoolManagement.API/          # Web API layer
│   ├── Controllers/               # API controllers
│   ├── Middleware/                # Custom middleware
│   └── Services/                  # API-specific services
│
├── SchoolManagement.Application/  # Application layer (CQRS)
│   ├── Features/                  # Vertical slices (commands, queries, handlers)
│   ├── DTOs/                      # Data transfer objects
│   ├── Common/                    # Shared interfaces, behaviors
│   └── DependencyInjection.cs
│
├── SchoolManagement.Domain/       # Domain layer
│   ├── Entities/                  # Domain entities
│   ├── Enums/                     # Enumerations
│   └── Common/                    # Base entity
│
└── SchoolManagement.Infrastructure/ # Infrastructure layer
    ├── Persistence/               # EF Core, DbContext
    ├── Identity/                  # Authentication, JWT
    ├── Services/                  # File storage, etc.
    └── Seed/                      # Data seeding
```

## API Endpoints Summary

| Module | Endpoint Base | Description |
|--------|---------------|-------------|
| Authentication | `/api/auth` | Login, password management |
| Students | `/api/students` | Student CRUD, guardians, documents |
| Staff | `/api/staff` | Staff CRUD, role assignments |
| Classes | `/api/classes` | Classes, sections, class-sections |
| Subjects | `/api/subjects` | Subjects, teacher mappings |
| Exams | `/api/exams` | Terms, grades, exams, results, report cards |
| Timetable | `/api/timetable` | Time slots, entries, schedules |
| Promotions | `/api/promotions` | Student promotions |

## Development Guidelines

See [.github/copilot-instructions.md](./.github/copilot-instructions.md) for detailed coding conventions and patterns.

### Key Conventions

- All commands/queries are **records**
- Use `IRequest<Result<T>>` for all MediatR requests
- No repositories - use `IApplicationDbContext` directly
- FluentValidation runs automatically via pipeline behavior
- Soft delete only - never hard delete
- Set `DeleteBehavior.Restrict` on foreign keys

## Testing with Postman

1. Import `School-Management-API.postman_collection.json` into Postman
2. Run the **Login** request to get a JWT token (auto-saved to collection variable)
3. All subsequent requests will automatically use the token
4. Update the `base_url` variable if needed

## Logging

Logs are written to:
- Console (Development)
- `Logs/log-{date}.txt` files
- Serilog configuration in `appsettings.json`

## CORS

Currently set to `AllowAll` for development. **Review before production deployment.**

## License

[Your License Here]

## Support

For API integration support:
- Review API documentation
- Check Swagger UI at `/swagger`
- Contact backend team

---

**Version**: 1.0  
**Last Updated**: February 2026