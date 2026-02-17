# School Management System - AI Agent Guidelines

## Architecture

**Clean Architecture** with strict dependency flow: `API → Infrastructure → Application → Domain`

- **Domain**: Core entities inheriting [BaseEntity](src/SchoolManagement.Domain/Common/BaseEntity.cs) with automatic audit fields
- **Application**: CQRS pattern via MediatR - all requests are **records** implementing `IRequest<Result<T>>`
- **Infrastructure**: EF Core with MySQL, ASP.NET Identity + JWT
- **API**: Thin controllers delegating to MediatR

**Key Pattern**: No repositories—use `IApplicationDbContext` directly in handlers ([example](src/SchoolManagement.Application/Features/Students/Commands/StudentCommandHandlers.cs))

## Code Style

### Feature Organization (Vertical Slices)

Each feature in `Application/Features/{FeatureName}/`:
```
Students/
├── Commands/
│   ├── StudentCommands.cs          # Command records
│   └── StudentCommandHandlers.cs   # IRequestHandler implementations
├── Queries/
│   ├── StudentQueries.cs
│   └── StudentQueryHandlers.cs
└── Validators/
    └── StudentValidators.cs        # FluentValidation
```

Corresponding DTOs in `Application/DTOs/{FeatureName}/`

### Naming Conventions

- **Commands**: `{Verb}{Entity}Command` (e.g., `AdmitStudentCommand`, `UpdateStudentCommand`)
- **Queries**: `Get{Entity}{Context}Query` (e.g., `GetStudentByIdQuery`, `GetStudentsListQuery`)
- **Handlers**: `{CommandName}Handler` implementing `IRequestHandler<TRequest, Result<TResponse>>`
- **Validators**: `{CommandName}Validator` extending `AbstractValidator<T>`

### Record-Based Requests

All commands and queries are **records**:
```csharp
public record AdmitStudentCommand(
    string FirstName,
    string LastName,
    // ... parameters
) : IRequest<Result<StudentDto>>;
```

## Validation

**FluentValidation** runs automatically via [ValidationBehavior](src/SchoolManagement.Application/Common/Behaviors/ValidationBehavior.cs) MediatR pipeline.

- Create validators in `Features/{Feature}/Validators/`
- Auto-registered via Assembly scanning in [DependencyInjection.cs](src/SchoolManagement.Application/DependencyInjection.cs#L16)
- Validation failures throw `ValidationException` caught by [ExceptionHandlingMiddleware](src/SchoolManagement.API/Middleware/ExceptionHandlingMiddleware.cs)

## Data Access

### Entity Configuration

- Use Fluent API in [EntityConfigurations.cs](src/SchoolManagement.Infrastructure/Persistence/Configurations/EntityConfigurations.cs)
- **Critical**: Set `DeleteBehavior.Restrict` on foreign keys to prevent cascade deletes
- All entities have global query filters for soft delete: `HasQueryFilter(e => !e.IsDeleted)`

### Audit Fields

Automatic via [ApplicationDbContext.SaveChangesAsync](src/SchoolManagement.Infrastructure/Persistence/ApplicationDbContext.cs#L65-L77):
- `CreatedAt` set on insert
- `UpdatedAt` set on modification
- Never set these manually

## Authentication & Authorization

### JWT Configuration

- Tokens generated via [JwtTokenService](src/SchoolManagement.Infrastructure/Identity/JwtTokenService.cs)
- User context accessed via `ICurrentUserService` (registered in API layer)
- **Roles**: `SuperAdmin`, `Admin`, `Teacher`, `Student`, `Parent`

### Controller Authorization

```csharp
[Authorize]  // Global on controller
[Authorize(Roles = "SuperAdmin,Admin")]  // Specific on actions
```

### Identity Requirements

Password rules defined in [Infrastructure DependencyInjection](src/SchoolManagement.Infrastructure/DependencyInjection.cs#L25-L36):
- Min length: 8 characters
- Requires: digit, lowercase, uppercase
- Unique emails enforced

## Error Handling

**Two-tier handling** in [ExceptionHandlingMiddleware](src/SchoolManagement.API/Middleware/ExceptionHandlingMiddleware.cs):

1. **ValidationException**: Returns 400 with structured field errors
2. **All other exceptions**: Returns 500 (detailed in dev, generic in prod)

### Result Pattern

All handler responses use `Result<T>` from [Common/Models](src/SchoolManagement.Application/Common/Models/Result.cs):
```csharp
return Result<StudentDto>.Success(studentDto, "Student admitted successfully");
return Result<StudentDto>.Failure("Student not found");
```

## Dependency Injection

Register services via extension methods in each layer's `DependencyInjection.cs`:

- **Application**: MediatR + FluentValidation auto-scanning
- **Infrastructure**: DbContext, Identity, JWT, file storage
- **API**: HttpContextAccessor, CurrentUserService

Called in [Program.cs](src/SchoolManagement.API/Program.cs#L16-L17).

## Build and Test

```powershell
# Build solution
dotnet build

# Run API (seeds database automatically)
dotnet run --project .\src\SchoolManagement.API\

# Database migrations
dotnet ef migrations add <Name> --project .\src\SchoolManagement.Infrastructure\ --startup-project .\src\SchoolManagement.API\
dotnet ef database update --project .\src\SchoolManagement.Infrastructure\ --startup-project .\src\SchoolManagement.API\
```

**Database**: MySQL via `MySql.EntityFrameworkCore` provider

## Project Conventions

1. **No direct DbContext mutations in controllers**—always use MediatR commands/queries
2. **Soft delete only**—set `IsDeleted = true`, never hard delete
3. **Unique indexes** on `Email` and admission/employee IDs (see entity configurations)
4. **File uploads** handled via `IFileStorageService` returning file paths
5. **Seeding** runs automatically on app start ([DataSeeder.cs](src/SchoolManagement.Infrastructure/Seed/DataSeeder.cs))
6. **Logging** via Serilog—configuration in `appsettings.json`, logs to `Logs/` directory
7. **CORS** set to `AllowAll` for development (review before production)

## Integration Points

- **Database**: MySQL connection string in `appsettings.json` → `ConnectionStrings:DefaultConnection`
- **JWT Settings**: `appsettings.json` → `JwtSettings` (Issuer, Audience, SecretKey)
- **Static Files**: Served from `wwwroot` for uploaded documents
- **Swagger**: Available at `/swagger` in development mode

## Security Notes

- **Password hashing** managed by ASP.NET Identity (PBKDF2)
- **Refresh tokens** stored in `ApplicationUser` table
- **JWT expiry**: Configurable in `JwtSettings:ExpiryMinutes`
- **File uploads**: Validate size/type in validators before storage
- **Sensitive data**: Never log passwords, tokens, or connection strings—Serilog configured to scrub
