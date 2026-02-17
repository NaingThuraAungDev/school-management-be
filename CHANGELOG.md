# API Changelog

All notable changes to the School Management System API will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-02-17

### Added

#### Authentication
- `POST /api/auth/login` - User authentication with JWT token generation
- `POST /api/auth/change-password` - Change current user's password
- `POST /api/auth/reset-password` - Admin reset user password
- JWT Bearer token authentication for all protected endpoints
- Role-based access control (SuperAdmin, Admin, Teacher, Student, Parent)

#### Students Management
- `POST /api/students` - Admit new student (Admin only)
- `GET /api/students` - Get paginated list of students with search and filters
- `GET /api/students/{id}` - Get student details including guardians and documents
- `PUT /api/students/{id}` - Update student information (Admin only)
- `DELETE /api/students/{id}` - Soft delete student (Admin only)
- `POST /api/students/{studentId}/guardians` - Link guardian to student (Admin only)
- `POST /api/students/{studentId}/documents` - Upload student document (Admin only)
- Support for multiple guardians per student
- Document management (birth certificates, photos, etc.)
- Automatic roll number and admission ID generation

#### Staff Management
- `POST /api/staff` - Onboard new staff member (Admin only)
- `GET /api/staff` - Get paginated list of staff with filters
- `GET /api/staff/{id}` - Get staff details including roles
- `PUT /api/staff/{id}` - Update staff information (Admin only)
- `DELETE /api/staff/{id}` - Soft delete staff (Admin only)
- `POST /api/staff/{staffId}/roles` - Assign role to staff member (Admin only)
- Support for multiple roles per staff member
- Staff types: Teacher, Admin, Support
- Role types: ClassTeacher, HOD, Admin, SubjectTeacher, Principal

#### Class Management
- `POST /api/classes` - Create new class (Admin only)
- `GET /api/classes` - Get all classes with sections
- `POST /api/classes/sections` - Create new section (Admin only)
- `GET /api/classes/sections` - Get all sections
- `POST /api/classes/class-sections` - Create class-section combination (Admin only)
- `GET /api/classes/class-sections` - Get class-sections with optional class filter
- Student capacity tracking per class-section
- Automatic student count calculation

#### Subjects
- `POST /api/subjects` - Create new subject (Admin only)
- `GET /api/subjects` - Get all subjects
- `POST /api/subjects/teacher-mappings` - Map teacher to subject for class-section (Admin only)
- `GET /api/subjects/teacher-mappings` - Get subject-teacher mappings with filters
- Subject code and description support

#### Exams Management
- `POST /api/exams/terms` - Create exam term (Admin only)
- `GET /api/exams/terms` - Get exam terms for academic year
- `POST /api/exams/grades` - Create grade definition (Admin only)
- `GET /api/exams/grades` - Get grade definitions for academic year
- `POST /api/exams` - Create exam (Admin/Teacher)
- `GET /api/exams` - Get exams with optional filters
- `POST /api/exams/results` - Record student exam result (Admin/Teacher)
- `GET /api/exams/results` - Get exam results with filters
- `POST /api/exams/report-card-templates` - Create report card template (Admin only)
- `GET /api/exams/report-card/{studentId}` - Generate student report card
- Exam term types: MidTerm, Final, Quarterly, HalfYearly
- Automatic grade calculation based on percentage
- Overall percentage and grade calculation for report cards
- Configurable grading system per academic year

#### Timetable Management
- `POST /api/timetable/time-slots` - Create time slot (Admin only)
- `GET /api/timetable/time-slots` - Get all time slots
- `POST /api/timetable/entries` - Create timetable entry (Admin only)
- `PUT /api/timetable/entries/{id}` - Update timetable entry (Admin only)
- `DELETE /api/timetable/entries/{id}` - Delete timetable entry (Admin only)
- `GET /api/timetable/by-class` - Get timetable for class-section
- `GET /api/timetable/by-teacher` - Get timetable for teacher
- Day-wise schedule management (Monday-Sunday)
- Room allocation support

#### Promotions
- `POST /api/promotions/bulk` - Bulk promote students (Admin only)
- `GET /api/promotions/preview` - Preview students eligible for promotion (Admin only)
- Eligibility calculation based on academic performance
- Batch promotion support

### Features

#### Core Features
- Clean Architecture with CQRS pattern
- MediatR for command/query handling
- Automatic validation using FluentValidation
- Centralized exception handling middleware
- Soft delete for all entities (no hard deletes)
- Automatic audit fields (CreatedAt, UpdatedAt)
- Global query filters for soft-deleted records

#### Security
- JWT Bearer token authentication
- Password hashing using ASP.NET Identity
- Role-based authorization
- Secure password requirements (min 8 chars, digit, uppercase, lowercase)
- Token refresh support

#### Data Management
- MySQL database with Entity Framework Core
- Database seeding for initial data
- Automatic migration support
- Foreign key constraints with DeleteBehavior.Restrict

#### Logging & Monitoring
- Serilog for structured logging
- Request/response logging
- File-based logging in Logs/ directory
- Console logging in development

#### API Documentation
- Swagger/OpenAPI documentation at `/swagger` (Development)
- Interactive API testing via Swagger UI
- JWT authentication support in Swagger
- Comprehensive documentation files
- Postman collection for API testing
- TypeScript type definitions

#### Developer Experience
- PowerShell scripts for database migrations
- Automatic database seeding
- CORS enabled for development
- Static file serving for uploads
- Detailed error messages in development

### Technical Details

#### Response Formats
- Success responses with data payload
- Consistent error response format with errors array
- HTTP 200 for successful GET/POST with data
- HTTP 201 for successful resource creation
- HTTP 204 for successful DELETE/UPDATE without data
- HTTP 400 for validation errors
- HTTP 401 for authentication errors
- HTTP 404 for resource not found
- HTTP 500 for server errors

#### Pagination
- Query parameters: `pageNumber`, `pageSize`
- Default: pageNumber=1, pageSize=10
- Available on list endpoints (students, staff)

#### Search & Filtering
- Text search via `searchTerm` query parameter
- Entity-specific filters (classSectionId, isActive, staffType, etc.)
- Combine multiple filters in single request

#### File Uploads
- Multipart/form-data support
- Document management for students
- File path storage in database
- Static file serving from wwwroot

#### Date Handling
- ISO 8601 format for all dates
- UTC timezone support
- Automatic date conversion

#### GUID Usage
- All entity IDs are GUIDs (UUID v4)
- Client-side ID generation supported
- Consistent across all entities

### Dependencies

#### Backend Framework
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- MySQL.EntityFrameworkCore 8.0

#### Libraries
- MediatR 12.x (CQRS)
- FluentValidation 11.x
- Serilog 3.x
- Swashbuckle.AspNetCore 6.x (Swagger)

#### Database
- MySQL 8.0+

---

## Version History

### [1.0.0] - 2026-02-17
- Initial release
- Complete API for school management system
- Full CRUD operations for all entities
- Authentication and authorization
- Comprehensive documentation

---

## Migration Notes

### From Development to Production

1. **Update CORS Policy**
   - Change from `AllowAll` to specific origins
   - Update in `Program.cs`

2. **Update Connection String**
   - Set production database connection
   - Update `appsettings.Production.json`

3. **JWT Settings**
   - Generate new secret key for production
   - Update token expiry settings as needed

4. **Disable Swagger**
   - Swagger only enabled in Development by default
   - Remove if not needed in production

5. **Logging Configuration**
   - Review log levels for production
   - Configure log retention policies
   - Consider centralized logging

6. **Static Files**
   - Configure proper file storage (cloud storage recommended)
   - Update `IFileStorageService` implementation

---

## Breaking Changes

None in this version (initial release).

---

## Deprecations

None in this version (initial release).

---

## Security Notes

### Password Requirements
- Minimum 8 characters
- At least one digit
- At least one lowercase letter
- At least one uppercase letter
- Unique email addresses enforced

### Token Security
- JWT tokens expire based on configuration
- Refresh tokens stored securely
- Token validation on every protected endpoint

### Data Security
- Soft delete prevents accidental data loss
- Audit fields track changes
- Role-based access prevents unauthorized operations

---

## Known Issues

None at this time.

---

## Coming Soon (Planned Features)

- Attendance management
- Fee management
- Library management
- Transport management
- Messaging system
- Parent portal
- Mobile app support
- Advanced reporting
- Dashboard analytics
- Notification system
- Calendar integration

---

## Support

For questions or issues:
- Check API documentation
- Review Swagger documentation at `/swagger`
- Contact backend development team

---

## Contributors

Backend Development Team - Initial API implementation

---

**Last Updated**: February 17, 2026
