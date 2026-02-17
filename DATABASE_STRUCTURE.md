# School Management System - Database Structure

## Overview

This document describes the complete database schema for the School Management System. The system uses **MySQL** with **Entity Framework Core** and follows **Clean Architecture** principles with soft delete support across all entities.

### Database Provider
- **Provider**: MySQL (via `MySql.EntityFrameworkCore`)
- **Pattern**: Code-First with EF Core Migrations
- **Soft Delete**: All entities support soft delete via `IsDeleted` flag

### Common Fields (BaseEntity)

All entities inherit from `BaseEntity` and include these fields:

| Field | Type | Description |
|-------|------|-------------|
| `Id` | `GUID` | Primary key |
| `CreatedAt` | `DateTime` | Auto-set on creation (UTC) |
| `UpdatedAt` | `DateTime?` | Auto-set on modification (UTC) |
| `CreatedBy` | `string(256)?` | User who created the record |
| `UpdatedBy` | `string(256)?` | User who last updated the record |
| `IsDeleted` | `bool` | Soft delete flag (default: false) |

---

## Core Entities

### 1. AspNetUsers (Identity)

ASP.NET Identity table extended with custom fields.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | `string(450)` | PK | Identity user ID |
| `UserName` | `string(256)` | UNIQUE, NOT NULL | Username |
| `Email` | `string(256)` | UNIQUE, NOT NULL | Email address |
| `EmailConfirmed` | `bool` | NOT NULL | Email verification status |
| `PasswordHash` | `string` | | Hashed password |
| `PhoneNumber` | `string` | | Phone number |
| `UserType` | `int` (enum) | NOT NULL | 0=Student, 1=Staff, 2=Admin, 3=Parent |
| `StudentId` | `GUID?` | FK → Students | Linked student record |
| `StaffId` | `GUID?` | FK → Staff | Linked staff record |
| `RefreshToken` | `string?` | | JWT refresh token |
| `RefreshTokenExpiryTime` | `DateTime?` | | Refresh token expiration |

**Indexes:**
- `IX_AspNetUsers_Email` (UNIQUE)
- `IX_AspNetUsers_UserName` (UNIQUE)

---

### 2. Students

Student records with admission and personal information.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `FirstName` | `string(100)` | NOT NULL | Student first name |
| `LastName` | `string(100)` | NOT NULL | Student last name |
| `DateOfBirth` | `DateTime` | NOT NULL | Date of birth |
| `Gender` | `int` (enum) | NOT NULL | 0=Male, 1=Female, 2=Other |
| `Email` | `string(256)` | UNIQUE, NOT NULL | Student email |
| `Phone` | `string(20)?` | | Contact phone |
| `Address` | `string(500)?` | | Residential address |
| `RollNumber` | `string(50)?` | | Class roll number |
| `AdmissionId` | `string(50)` | UNIQUE, NOT NULL | Unique admission identifier |
| `AdmissionDate` | `DateTime` | NOT NULL | Date of admission |
| `IsActive` | `bool` | NOT NULL | Active status (default: true) |
| `UserId` | `string(450)?` | FK → AspNetUsers | Linked user account |
| `ClassSectionId` | `GUID?` | FK → ClassSections | Current class section |
| `AcademicYearId` | `GUID?` | FK → AcademicYears | Current academic year |

**Indexes:**
- `IX_Students_Email` (UNIQUE)
- `IX_Students_AdmissionId` (UNIQUE)
- `IX_Students_UserId`

**Relationships:**
- One-to-Many: Student → Documents (CASCADE delete)
- One-to-Many: Student → StudentExamResults (CASCADE delete)
- One-to-Many: Student → StudentGuardians (CASCADE delete)
- One-to-Many: Student → PromotionRecords (RESTRICT delete)
- Many-to-One: Student → ClassSection (RESTRICT delete)
- Many-to-One: Student → AcademicYear (RESTRICT delete)

---

### 3. Guardians (Parents)

Guardian/parent information for students.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `Name` | `string(200)` | NOT NULL | Guardian full name |
| `Mobile` | `string(20)` | NOT NULL | Contact mobile |
| `Email` | `string(256)?` | | Guardian email |
| `Relationship` | `int` (enum) | NOT NULL | 0=Father, 1=Mother, 2=Guardian, 3=Other |
| `Address` | `string(500)?` | | Residential address |
| `Occupation` | `string(200)?` | | Guardian's occupation |

**Relationships:**
- One-to-Many: Guardian → StudentGuardians (CASCADE delete)

---

### 4. StudentGuardians (Junction Table)

Maps students to their guardians (many-to-many).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `StudentId` | `GUID` | FK → Students, NOT NULL | Student reference |
| `GuardianId` | `GUID` | FK → Guardians, NOT NULL | Guardian reference |
| `IsPrimaryContact` | `bool` | NOT NULL | Primary contact flag (default: false) |

**Indexes:**
- `IX_StudentGuardians_StudentId_GuardianId` (UNIQUE composite)

**Relationships:**
- Many-to-One: StudentGuardian → Student (CASCADE delete)
- Many-to-One: StudentGuardian → Guardian (CASCADE delete)

---

### 5. Documents

Student document attachments.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `StudentId` | `GUID` | FK → Students, NOT NULL | Owning student |
| `DocumentType` | `int` (enum) | NOT NULL | 0=BirthCert, 1=PrevRecords, 2=TransferCert, 3=Photo, 4=Other |
| `FileName` | `string(256)` | NOT NULL | Original file name |
| `FilePath` | `string(500)` | NOT NULL | Server file path |
| `ContentType` | `string(100)` | NOT NULL | MIME type |
| `FileSize` | `long` | NOT NULL | File size in bytes |
| `UploadedAt` | `DateTime` | NOT NULL | Upload timestamp |

**Relationships:**
- Many-to-One: Document → Student (CASCADE delete)

---

### 6. Staff

Staff member records (teachers, admin, support).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `FirstName` | `string(100)` | NOT NULL | Staff first name |
| `LastName` | `string(100)` | NOT NULL | Staff last name |
| `Email` | `string(256)` | UNIQUE, NOT NULL | Staff email |
| `Phone` | `string(20)?` | | Contact phone |
| `Qualification` | `string(200)?` | | Educational qualification |
| `JoiningDate` | `DateTime` | NOT NULL | Date joined institution |
| `StaffType` | `int` (enum) | NOT NULL | 0=Teacher, 1=Admin, 2=Support |
| `IsActive` | `bool` | NOT NULL | Active status (default: true) |
| `UserId` | `string(450)?` | FK → AspNetUsers | Linked user account |

**Indexes:**
- `IX_Staff_Email` (UNIQUE)
- `IX_Staff_UserId`

**Relationships:**
- One-to-Many: Staff → StaffRoles (CASCADE delete)
- One-to-Many: Staff → SubjectTeacherMappings (CASCADE delete)

---

### 7. StaffRoles

Assigns roles to staff members (e.g., Class Teacher, HOD).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `StaffId` | `GUID` | FK → Staff, NOT NULL | Staff member reference |
| `Role` | `int` (enum) | NOT NULL | 0=ClassTeacher, 1=HOD, 2=Admin, 3=SubjectTeacher, 4=Principal |
| `ClassSectionId` | `GUID?` | FK → ClassSections | Assigned class (for ClassTeacher/HOD) |

**Relationships:**
- Many-to-One: StaffRole → Staff (CASCADE delete)
- Many-to-One: StaffRole → ClassSection (optional reference)

---

### 8. Classes

Grade/class levels (e.g., Grade 5, Grade 6).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `Name` | `string(100)` | UNIQUE, NOT NULL | Class name (e.g., "Grade 5") |
| `SortOrder` | `int` | NOT NULL | Display order |
| `Description` | `string(500)?` | | Optional description |

**Indexes:**
- `IX_Classes_Name` (UNIQUE)

**Relationships:**
- One-to-Many: Class → ClassSections (CASCADE delete)

---

### 9. Sections

Section divisions (e.g., A, B, C).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `Name` | `string(50)` | UNIQUE, NOT NULL | Section name (e.g., "A") |
| `SortOrder` | `int` | NOT NULL | Display order |

**Indexes:**
- `IX_Sections_Name` (UNIQUE)

**Relationships:**
- One-to-Many: Section → ClassSections

---

### 10. ClassSections

Combines Class + Section (e.g., "Grade 5-A").

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `ClassId` | `GUID` | FK → Classes, NOT NULL | Class reference |
| `SectionId` | `GUID` | FK → Sections, NOT NULL | Section reference |
| `Capacity` | `int` | NOT NULL | Max student capacity (default: 40) |

**Indexes:**
- `IX_ClassSections_ClassId_SectionId` (UNIQUE composite)

**Relationships:**
- Many-to-One: ClassSection → Class (CASCADE delete)
- Many-to-One: ClassSection → Section (CASCADE delete)
- One-to-Many: ClassSection → Students
- One-to-Many: ClassSection → SubjectTeacherMappings
- One-to-Many: ClassSection → TimetableEntries
- One-to-Many: ClassSection → Exams

---

### 11. Subjects

Academic subjects (e.g., Mathematics, Science).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `Name` | `string(100)` | NOT NULL | Subject name |
| `Code` | `string(20)` | UNIQUE, NOT NULL | Subject code (e.g., "MATH") |
| `Description` | `string(500)?` | | Optional description |

**Indexes:**
- `IX_Subjects_Code` (UNIQUE)

**Relationships:**
- One-to-Many: Subject → SubjectTeacherMappings
- One-to-Many: Subject → Exams

---

### 12. SubjectTeacherMappings

Maps subjects to teachers for specific class sections.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `SubjectId` | `GUID` | FK → Subjects, NOT NULL | Subject being taught |
| `StaffId` | `GUID` | FK → Staff, NOT NULL | Teacher assigned |
| `ClassSectionId` | `GUID` | FK → ClassSections, NOT NULL | Class section |
| `AcademicYearId` | `GUID` | FK → AcademicYears, NOT NULL | Academic year |

**Indexes:**
- `IX_SubjectTeacherMappings_SubjectId_StaffId_ClassSectionId` (UNIQUE composite)

**Relationships:**
- Many-to-One: SubjectTeacherMapping → Subject (RESTRICT delete)
- Many-to-One: SubjectTeacherMapping → Staff (RESTRICT delete)
- Many-to-One: SubjectTeacherMapping → ClassSection (RESTRICT delete)
- Many-to-One: SubjectTeacherMapping → AcademicYear (RESTRICT delete)

---

### 13. AcademicYears

Academic year periods.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `Year` | `string(50)` | UNIQUE, NOT NULL | Year label (e.g., "2025-2026") |
| `StartDate` | `DateTime` | NOT NULL | Year start date |
| `EndDate` | `DateTime` | NOT NULL | Year end date |
| `IsCurrent` | `bool` | NOT NULL | Current active year (default: false) |

**Indexes:**
- `IX_AcademicYears_Year` (UNIQUE)

**Relationships:**
- One-to-Many: AcademicYear → SubjectTeacherMappings
- One-to-Many: AcademicYear → TimetableEntries
- One-to-Many: AcademicYear → ExamTerms
- One-to-Many: AcademicYear → GradeDefinitions
- One-to-Many: AcademicYear → ReportCardTemplates
- One-to-Many: AcademicYear → PromotionRecords

---

### 14. TimeSlots

Daily time periods for timetabling.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `StartTime` | `TimeSpan` | NOT NULL | Period start time |
| `EndTime` | `TimeSpan` | NOT NULL | Period end time |
| `Label` | `string(50)` | NOT NULL | Period label (e.g., "Period 1") |
| `SortOrder` | `int` | NOT NULL | Display order |
| `IsBreak` | `bool` | NOT NULL | Break period flag (default: false) |

**Relationships:**
- One-to-Many: TimeSlot → TimetableEntries

---

### 15. TimetableEntries

Weekly timetable scheduling.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `ClassSectionId` | `GUID` | FK → ClassSections, NOT NULL | Class section |
| `SubjectTeacherMappingId` | `GUID` | FK → SubjectTeacherMappings, NOT NULL | Subject + Teacher |
| `TimeSlotId` | `GUID` | FK → TimeSlots, NOT NULL | Time period |
| `DayOfWeek` | `int` (enum) | NOT NULL | 1=Mon, 2=Tue, 3=Wed, 4=Thu, 5=Fri, 6=Sat, 7=Sun |
| `AcademicYearId` | `GUID` | FK → AcademicYears, NOT NULL | Academic year |

**Indexes:**
- `IX_TimetableEntries_ClassSectionId_DayOfWeek_TimeSlotId_AcademicYearId` (UNIQUE composite)

**Relationships:**
- Many-to-One: TimetableEntry → ClassSection (RESTRICT delete)
- Many-to-One: TimetableEntry → SubjectTeacherMapping (RESTRICT delete)
- Many-to-One: TimetableEntry → TimeSlot (RESTRICT delete)
- Many-to-One: TimetableEntry → AcademicYear (RESTRICT delete)

---

### 16. ExamTerms

Exam periods within academic years.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `Name` | `string(100)` | NOT NULL | Term name (e.g., "Mid-Term Exam") |
| `TermType` | `int` (enum) | NOT NULL | 0=MidTerm, 1=Final, 2=Quarterly, 3=HalfYearly |
| `StartDate` | `DateTime` | NOT NULL | Term start date |
| `EndDate` | `DateTime` | NOT NULL | Term end date |
| `AcademicYearId` | `GUID` | FK → AcademicYears, NOT NULL | Associated academic year |

**Relationships:**
- Many-to-One: ExamTerm → AcademicYear (RESTRICT delete)
- One-to-Many: ExamTerm → Exams

---

### 17. Exams

Individual exam schedules (subject + class + term).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `ExamTermId` | `GUID` | FK → ExamTerms, NOT NULL | Exam term |
| `SubjectId` | `GUID` | FK → Subjects, NOT NULL | Subject |
| `ClassSectionId` | `GUID` | FK → ClassSections, NOT NULL | Class section |
| `ExamDate` | `DateTime` | NOT NULL | Scheduled exam date |
| `MaxMarks` | `decimal(5,2)` | NOT NULL | Maximum marks |
| `PassingMarks` | `decimal(5,2)` | NOT NULL | Passing threshold |

**Indexes:**
- `IX_Exams_ExamTermId_SubjectId_ClassSectionId` (UNIQUE composite)

**Relationships:**
- Many-to-One: Exam → ExamTerm (RESTRICT delete)
- Many-to-One: Exam → Subject (RESTRICT delete)
- Many-to-One: Exam → ClassSection (RESTRICT delete)
- One-to-Many: Exam → StudentExamResults

---

### 18. StudentExamResults

Individual student exam scores.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `ExamId` | `GUID` | FK → Exams, NOT NULL | Exam reference |
| `StudentId` | `GUID` | FK → Students, NOT NULL | Student reference |
| `MarksObtained` | `decimal(5,2)` | NOT NULL | Marks scored |
| `Percentage` | `decimal(5,2)` | NOT NULL | Calculated percentage |
| `GradeDefinitionId` | `GUID?` | FK → GradeDefinitions | Assigned grade |
| `Remarks` | `string(500)?` | | Optional remarks |

**Indexes:**
- `IX_StudentExamResults_ExamId_StudentId` (UNIQUE composite)

**Relationships:**
- Many-to-One: StudentExamResult → Exam (RESTRICT delete)
- Many-to-One: StudentExamResult → Student (RESTRICT delete)
- Many-to-One: StudentExamResult → GradeDefinition (RESTRICT delete)

---

### 19. GradeDefinitions

Grading scale definitions.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `Label` | `string(10)` | NOT NULL | Grade label (e.g., "A+") |
| `MinPercentage` | `decimal(5,2)` | NOT NULL | Minimum percentage |
| `MaxPercentage` | `decimal(5,2)` | NOT NULL | Maximum percentage |
| `GradePoint` | `int` | NOT NULL | Grade point value |
| `Description` | `string(200)?` | | Description (e.g., "Excellent") |
| `AcademicYearId` | `GUID` | FK → AcademicYears, NOT NULL | Academic year |

**Relationships:**
- Many-to-One: GradeDefinition → AcademicYear (RESTRICT delete)

---

### 20. ReportCardTemplates

Report card layout templates.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `Name` | `string(100)` | NOT NULL | Template name |
| `TemplateConfig` | `string(4000)` | NOT NULL | JSON configuration (default: "{}") |
| `IsActive` | `bool` | NOT NULL | Active status (default: true) |
| `AcademicYearId` | `GUID` | FK → AcademicYears, NOT NULL | Academic year |

**Relationships:**
- Many-to-One: ReportCardTemplate → AcademicYear (RESTRICT delete)

---

### 21. PromotionRecords

Student class promotion history.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| *(BaseEntity fields)* | | | See Common Fields |
| `StudentId` | `GUID` | FK → Students, NOT NULL | Promoted student |
| `FromClassSectionId` | `GUID` | FK → ClassSections, NOT NULL | Source class section |
| `ToClassSectionId` | `GUID` | FK → ClassSections, NOT NULL | Destination class section |
| `AcademicYearId` | `GUID` | FK → AcademicYears, NOT NULL | Academic year |
| `PromotedAt` | `DateTime` | NOT NULL | Promotion timestamp |
| `Remarks` | `string(500)?` | | Optional remarks |

**Relationships:**
- Many-to-One: PromotionRecord → Student (RESTRICT delete)
- Many-to-One: PromotionRecord → ClassSection (FromClassSection) (RESTRICT delete)
- Many-to-One: PromotionRecord → ClassSection (ToClassSection) (RESTRICT delete)
- Many-to-One: PromotionRecord → AcademicYear (RESTRICT delete)

---

## Enumerations Reference

### Gender
```csharp
Male = 0
Female = 1
Other = 2
```

### GuardianRelationship
```csharp
Father = 0
Mother = 1
Guardian = 2
Other = 3
```

### DocumentType
```csharp
BirthCertificate = 0
PreviousRecords = 1
TransferCertificate = 2
Photo = 3
Other = 4
```

### StaffType
```csharp
Teacher = 0
Admin = 1
Support = 2
```

### StaffRoleType
```csharp
ClassTeacher = 0
HOD = 1
Admin = 2
SubjectTeacher = 3
Principal = 4
```

### UserType
```csharp
Student = 0
Staff = 1
Admin = 2
Parent = 3
```

### DayOfWeekEnum
```csharp
Monday = 1
Tuesday = 2
Wednesday = 3
Thursday = 4
Friday = 5
Saturday = 6
Sunday = 7
```

### ExamTermType
```csharp
MidTerm = 0
Final = 1
Quarterly = 2
HalfYearly = 3
```

---

## Entity Relationship Summary

### Key Relationships

#### Student-Centric
- **Student** ↔ **Guardian** (Many-to-Many via StudentGuardians)
- **Student** → **Documents** (One-to-Many, CASCADE)
- **Student** → **StudentExamResults** (One-to-Many, CASCADE)
- **Student** → **ClassSection** (Many-to-One, RESTRICT)
- **Student** → **PromotionRecords** (One-to-Many, RESTRICT)
- **Student** ↔ **AspNetUsers** (One-to-One optional)

#### Staff-Centric
- **Staff** → **StaffRoles** (One-to-Many, CASCADE)
- **Staff** → **SubjectTeacherMappings** (One-to-Many, CASCADE)
- **Staff** ↔ **AspNetUsers** (One-to-One optional)

#### Academic Structure
- **Class** → **ClassSections** (One-to-Many, CASCADE)
- **Section** → **ClassSections** (One-to-Many, CASCADE)
- **ClassSection** → **Students** (One-to-Many)
- **Subject** → **SubjectTeacherMappings** (One-to-Many)

#### Timetabling
- **ClassSection** + **SubjectTeacherMapping** + **TimeSlot** + **DayOfWeek** → **TimetableEntry**
- **AcademicYear** → **TimetableEntries** (One-to-Many)

#### Examinations
- **AcademicYear** → **ExamTerms** (One-to-Many)
- **ExamTerm** + **Subject** + **ClassSection** → **Exam** (unique composite)
- **Exam** + **Student** → **StudentExamResult** (unique composite)
- **GradeDefinition** ← **StudentExamResult** (optional grading)

#### Promotions
- **Student** + **FromClassSection** + **ToClassSection** + **AcademicYear** → **PromotionRecord**

---

## Delete Behavior Patterns

### CASCADE Delete
Applied to child entities that should be removed when parent is deleted:
- Student → Documents, StudentExamResults
- Guardian → StudentGuardians
- Staff → StaffRoles, SubjectTeacherMappings
- Class/Section → ClassSections

### RESTRICT Delete
Applied to protect referential integrity on critical relationships:
- All foreign keys to: ClassSection, AcademicYear, Subject
- Student → PromotionRecords
- Exam → StudentExamResults

### Soft Delete
All entities use `IsDeleted` flag with global query filters to hide deleted records from normal queries.

---

## Unique Constraints Summary

| Table | Unique Constraint | Columns |
|-------|------------------|---------|
| **AspNetUsers** | Email | `Email` |
| **AspNetUsers** | UserName | `UserName` |
| **Students** | Email | `Email` |
| **Students** | AdmissionId | `AdmissionId` |
| **Staff** | Email | `Email` |
| **Classes** | Name | `Name` |
| **Sections** | Name | `Name` |
| **Subjects** | Code | `Code` |
| **AcademicYears** | Year | `Year` |
| **ClassSections** | Class + Section | `ClassId, SectionId` |
| **StudentGuardians** | Student + Guardian | `StudentId, GuardianId` |
| **SubjectTeacherMappings** | Subject + Staff + ClassSection | `SubjectId, StaffId, ClassSectionId` |
| **TimetableEntries** | ClassSection + Day + TimeSlot + Year | `ClassSectionId, DayOfWeek, TimeSlotId, AcademicYearId` |
| **Exams** | ExamTerm + Subject + ClassSection | `ExamTermId, SubjectId, ClassSectionId` |
| **StudentExamResults** | Exam + Student | `ExamId, StudentId` |

---

## Database Diagram (Textual ERD)

```
┌─────────────────┐         ┌──────────────────┐
│  AspNetUsers    │◄────────┤    Students      │
│  (Identity)     │         │                  │
└─────────────────┘         └──────────────────┘
                                    │
                                    ├──► Documents (1:N)
                                    ├──► StudentExamResults (1:N)
                                    ├──► PromotionRecords (1:N)
                                    └──► StudentGuardians (M:N)
                                            │
                                            ▼
                                    ┌──────────────────┐
                                    │    Guardians     │
                                    └──────────────────┘

┌─────────────────┐
│  AspNetUsers    │◄────────┐
│  (Identity)     │         │
└─────────────────┘         │
                    ┌──────────────────┐
                    │      Staff       │
                    └──────────────────┘
                            │
                            ├──► StaffRoles (1:N)
                            └──► SubjectTeacherMappings (1:N)

┌──────────────────┐        ┌──────────────────┐        ┌──────────────────┐
│     Classes      │───────►│  ClassSections   │◄───────│    Sections      │
└──────────────────┘        └──────────────────┘        └──────────────────┘
                                    │
                                    ├──► Students (1:N)
                                    ├──► SubjectTeacherMappings (1:N)
                                    ├──► TimetableEntries (1:N)
                                    └──► Exams (1:N)

┌──────────────────┐
│    Subjects      │───────►  SubjectTeacherMappings (1:N)
└──────────────────┘              │
                                  │
                          ┌───────┴────────┐
                          ▼                ▼
                  TimetableEntries     Exams (1:N)
                          ▲
                          │
                  ┌───────┴────────┐
                  │                │
              TimeSlots      AcademicYears
                                  │
                                  ├──► ExamTerms (1:N)
                                  ├──► GradeDefinitions (1:N)
                                  ├──► ReportCardTemplates (1:N)
                                  └──► PromotionRecords (1:N)

┌──────────────────┐
│   ExamTerms      │───────► Exams (1:N)
└──────────────────┘              │
                                  └──► StudentExamResults (1:N)
                                            │
                                  ┌─────────┴────────┐
                                  ▼                  ▼
                          GradeDefinitions       Students
```

---

## Migration Commands

```powershell
# Create new migration
dotnet ef migrations add <MigrationName> `
  --project .\src\SchoolManagement.Infrastructure\ `
  --startup-project .\src\SchoolManagement.API\

# Apply migrations to database
dotnet ef database update `
  --project .\src\SchoolManagement.Infrastructure\ `
  --startup-project .\src\SchoolManagement.API\

# Remove last migration (if not applied)
dotnet ef migrations remove `
  --project .\src\SchoolManagement.Infrastructure\ `
  --startup-project .\src\SchoolManagement.API\

# Generate SQL script
dotnet ef migrations script `
  --project .\src\SchoolManagement.Infrastructure\ `
  --startup-project .\src\SchoolManagement.API\ `
  --output migration.sql
```

---

## Notes

1. **Audit Trail**: All entities automatically track `CreatedAt`, `UpdatedAt`, `CreatedBy`, and `UpdatedBy` via `ApplicationDbContext.SaveChangesAsync()` override.

2. **Soft Delete**: Global query filters (`HasQueryFilter(e => !e.IsDeleted)`) hide soft-deleted records from all queries.

3. **Identity Integration**: ASP.NET Identity tables (AspNetUsers, AspNetRoles, etc.) are managed separately and extended with custom fields.

4. **Connection String**: Located in `appsettings.json` → `ConnectionStrings:DefaultConnection`

5. **Database Seeding**: Automatic seeding runs on application start via `DataSeeder.cs` (creates default users, roles, academic year, etc.)

6. **Precision**: Decimal fields for marks/percentages use `decimal(5,2)` precision.

7. **Max Lengths**: All string fields have explicit max lengths to prevent unbounded data growth.

---

**Last Updated**: February 17, 2026  
**Database Version**: Initial Schema (Migration: `InitialCreate`)
