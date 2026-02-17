# School Management System API Documentation

Version: 1.0  
Last Updated: February 17, 2026

## Table of Contents

1. [Overview](#overview)
2. [Authentication](#authentication)
3. [Error Handling](#error-handling)
4. [API Endpoints](#api-endpoints)
   - [Authentication](#authentication-endpoints)
   - [Students](#students-endpoints)
   - [Staff](#staff-endpoints)
   - [Class Management](#class-management-endpoints)
   - [Subjects](#subjects-endpoints)
   - [Exams](#exams-endpoints)
   - [Timetable](#timetable-endpoints)
   - [Promotions](#promotions-endpoints)
5. [Data Models](#data-models)
6. [Enumerations](#enumerations)

---

## Overview

### Base URL
- **Development**: `https://localhost:{port}/api`
- **Production**: `{your-production-url}/api`

### API Features
- RESTful API architecture
- JWT-based authentication
- Role-based access control (RBAC)
- Centralized error handling
- Soft delete for all entities
- Automatic audit fields (CreatedAt, UpdatedAt)

### Content Type
All API requests and responses use `application/json` unless otherwise specified (e.g., file uploads use `multipart/form-data`).

### Date Format
All dates use ISO 8601 format: `YYYY-MM-DDTHH:mm:ss.sssZ`

---

## Authentication

### JWT Bearer Authentication

All endpoints except `/auth/login` require authentication via JWT Bearer token.

**Include in request headers:**
```http
Authorization: Bearer {your-jwt-token}
```

### User Roles

- **SuperAdmin**: Full system access
- **Admin**: Administrative operations
- **Teacher**: Teaching and exam management
- **Student**: Student-specific access
- **Parent**: Guardian access to student information

---

## Error Handling

### Standard Error Response

```json
{
  "errors": ["Error message 1", "Error message 2"]
}
```

### HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK - Request successful |
| 201 | Created - Resource created successfully |
| 204 | No Content - Successful delete/update |
| 400 | Bad Request - Validation errors |
| 401 | Unauthorized - Invalid/missing token |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource not found |
| 500 | Internal Server Error |

### Validation Errors

```json
{
  "errors": [
    "FirstName is required",
    "Email must be a valid email address"
  ]
}
```

---

## API Endpoints

### Authentication Endpoints

#### 1. Login

**POST** `/auth/login`

Authenticate user and receive JWT token.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh-token-here",
  "expiresAt": "2026-02-17T12:00:00Z",
  "email": "user@example.com",
  "userType": "Staff",
  "roles": ["Admin", "Teacher"]
}
```

**Response (401 Unauthorized):**
```json
{
  "errors": ["Invalid credentials"]
}
```

---

#### 2. Change Password

**POST** `/auth/change-password`

Change current user's password.

**Authentication Required:** Yes

**Request Body:**
```json
{
  "currentPassword": "oldPassword123",
  "newPassword": "newPassword456"
}
```

**Response (200 OK):**
```json
{
  "message": "Password changed successfully"
}
```

---

#### 3. Reset Password

**POST** `/auth/reset-password`

Reset user password (Admin only).

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "newPassword": "resetPassword123"
}
```

**Response (200 OK):**
```json
{
  "message": "Password reset successfully"
}
```

---

### Students Endpoints

#### 1. Admit Student

**POST** `/students`

Create a new student record.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "2010-05-15T00:00:00Z",
  "gender": 0,
  "email": "john.doe@example.com",
  "password": "initialPassword123",
  "phone": "+1234567890",
  "address": "123 Main St, City",
  "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response (201 Created):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "2010-05-15T00:00:00Z",
  "gender": 0,
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "address": "123 Main St, City",
  "rollNumber": "2026001",
  "admissionId": "ADM2026001",
  "admissionDate": "2026-02-17T10:00:00Z",
  "isActive": true,
  "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "classSectionName": "Grade 5-A",
  "guardians": [],
  "documents": []
}
```

---

#### 2. Update Student

**PUT** `/students/{id}`

Update existing student information.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "2010-05-15T00:00:00Z",
  "gender": 0,
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "address": "123 Main St, City",
  "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "isActive": true
}
```

**Response (204 No Content)**

---

#### 3. Delete Student

**DELETE** `/students/{id}`

Soft delete a student (sets IsDeleted flag).

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Response (204 No Content)**

---

#### 4. Get Student by ID

**GET** `/students/{id}`

Retrieve detailed student information.

**Authentication Required:** Yes

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "2010-05-15T00:00:00Z",
  "gender": 0,
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "address": "123 Main St, City",
  "rollNumber": "2026001",
  "admissionId": "ADM2026001",
  "admissionDate": "2026-02-17T10:00:00Z",
  "isActive": true,
  "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "classSectionName": "Grade 5-A",
  "guardians": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Jane Doe",
      "mobile": "+1234567890",
      "email": "jane.doe@example.com",
      "relationship": 1,
      "address": "123 Main St, City",
      "occupation": "Engineer",
      "isPrimaryContact": true
    }
  ],
  "documents": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "documentType": 0,
      "fileName": "birth_certificate.pdf",
      "filePath": "/uploads/documents/birth_certificate.pdf",
      "contentType": "application/pdf",
      "fileSize": 245760,
      "uploadedAt": "2026-02-17T10:00:00Z"
    }
  ]
}
```

---

#### 5. Get Students List

**GET** `/students`

Retrieve paginated list of students.

**Authentication Required:** Yes

**Query Parameters:**
- `pageNumber` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)
- `searchTerm` (string, optional): Search by name, email, or admission ID
- `classSectionId` (guid, optional): Filter by class section
- `isActive` (bool, optional): Filter by active status

**Example Request:**
```
GET /students?pageNumber=1&pageSize=20&searchTerm=John&isActive=true
```

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "rollNumber": "2026001",
    "admissionId": "ADM2026001",
    "classSectionName": "Grade 5-A",
    "isActive": true
  }
]
```

---

#### 6. Link Guardian

**POST** `/students/{studentId}/guardians`

Add a guardian to a student.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Jane Doe",
  "mobile": "+1234567890",
  "email": "jane.doe@example.com",
  "relationship": 1,
  "address": "123 Main St, City",
  "occupation": "Engineer",
  "isPrimaryContact": true
}
```

**Response (200 OK):**
```json
{
  "guardianId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 7. Upload Document

**POST** `/students/{studentId}/documents`

Upload a student document.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Content-Type:** `multipart/form-data`

**Form Data:**
- `studentId` (guid): Student ID
- `documentType` (int): Document type (see Enumerations)
- `file` (file): Document file

**Response (200 OK):**
```json
{
  "documentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

### Staff Endpoints

#### 1. Onboard Staff

**POST** `/staff`

Create a new staff member.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "firstName": "Sarah",
  "lastName": "Johnson",
  "email": "sarah.johnson@school.com",
  "password": "initialPassword123",
  "phone": "+1234567890",
  "qualification": "M.Ed in Mathematics",
  "joiningDate": "2026-02-01T00:00:00Z",
  "staffType": 0
}
```

**Response (201 Created):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "Sarah",
  "lastName": "Johnson",
  "email": "sarah.johnson@school.com",
  "phone": "+1234567890",
  "qualification": "M.Ed in Mathematics",
  "joiningDate": "2026-02-01T00:00:00Z",
  "staffType": 0,
  "isActive": true,
  "roles": []
}
```

---

#### 2. Update Staff

**PUT** `/staff/{id}`

Update staff member information.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "Sarah",
  "lastName": "Johnson",
  "email": "sarah.johnson@school.com",
  "phone": "+1234567890",
  "qualification": "M.Ed in Mathematics",
  "staffType": 0,
  "isActive": true
}
```

**Response (204 No Content)**

---

#### 3. Delete Staff

**DELETE** `/staff/{id}`

Soft delete a staff member.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Response (204 No Content)**

---

#### 4. Get Staff by ID

**GET** `/staff/{id}`

Retrieve staff member details.

**Authentication Required:** Yes

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "Sarah",
  "lastName": "Johnson",
  "email": "sarah.johnson@school.com",
  "phone": "+1234567890",
  "qualification": "M.Ed in Mathematics",
  "joiningDate": "2026-02-01T00:00:00Z",
  "staffType": 0,
  "isActive": true,
  "roles": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "role": 0,
      "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "classSectionName": "Grade 5-A"
    }
  ]
}
```

---

#### 5. Get Staff List

**GET** `/staff`

Retrieve list of staff members.

**Authentication Required:** Yes

**Query Parameters:**
- `pageNumber` (int, optional): Page number
- `pageSize` (int, optional): Items per page
- `searchTerm` (string, optional): Search by name or email
- `staffType` (int, optional): Filter by staff type
- `isActive` (bool, optional): Filter by active status

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "Sarah",
    "lastName": "Johnson",
    "email": "sarah.johnson@school.com",
    "staffType": 0,
    "isActive": true
  }
]
```

---

#### 6. Assign Staff Role

**POST** `/staff/{staffId}/roles`

Assign a role to a staff member.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "staffId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "role": 0,
  "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response (200 OK):**
```json
{
  "roleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

### Class Management Endpoints

#### 1. Create Class

**POST** `/classes`

Create a new class (e.g., Grade 5, Grade 10).

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "name": "Grade 5",
  "sortOrder": 5,
  "description": "Fifth grade curriculum"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 2. Get Classes

**GET** `/classes`

Retrieve all classes.

**Authentication Required:** Yes

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Grade 5",
    "sortOrder": 5,
    "description": "Fifth grade curriculum",
    "sections": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "classId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "className": "Grade 5",
        "sectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "sectionName": "A",
        "displayName": "Grade 5-A",
        "capacity": 40,
        "studentCount": 35
      }
    ]
  }
]
```

---

#### 3. Create Section

**POST** `/classes/sections`

Create a new section (e.g., Section A, Section B).

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "name": "A",
  "sortOrder": 1
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 4. Get Sections

**GET** `/classes/sections`

Retrieve all sections.

**Authentication Required:** Yes

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "A",
    "sortOrder": 1
  },
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "B",
    "sortOrder": 2
  }
]
```

---

#### 5. Create Class-Section

**POST** `/classes/class-sections`

Create a class-section combination (e.g., Grade 5-A).

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "classId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "capacity": 40
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 6. Get Class-Sections

**GET** `/classes/class-sections`

Retrieve class-section combinations.

**Authentication Required:** Yes

**Query Parameters:**
- `classId` (guid, optional): Filter by class

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "classId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "className": "Grade 5",
    "sectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "sectionName": "A",
    "displayName": "Grade 5-A",
    "capacity": 40,
    "studentCount": 35
  }
]
```

---

### Subjects Endpoints

#### 1. Create Subject

**POST** `/subjects`

Create a new subject.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "name": "Mathematics",
  "code": "MATH101",
  "description": "Advanced Mathematics"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 2. Get Subjects

**GET** `/subjects`

Retrieve all subjects.

**Authentication Required:** Yes

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Mathematics",
    "code": "MATH101",
    "description": "Advanced Mathematics"
  }
]
```

---

#### 3. Map Subject-Teacher

**POST** `/subjects/teacher-mappings`

Assign a teacher to a subject for a specific class section.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "subjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "staffId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 4. Get Subject Mappings

**GET** `/subjects/teacher-mappings`

Retrieve subject-teacher mappings.

**Authentication Required:** Yes

**Query Parameters:**
- `classSectionId` (guid, optional): Filter by class section
- `academicYearId` (guid, optional): Filter by academic year

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "subjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "subjectName": "Mathematics",
    "staffId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "staffName": "Sarah Johnson",
    "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "classSectionName": "Grade 5-A",
    "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
]
```

---

### Exams Endpoints

#### 1. Create Exam Term

**POST** `/exams/terms`

Create an exam term (e.g., Mid-term, Final).

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "name": "Mid-term Examination",
  "termType": 0,
  "startDate": "2026-06-01T00:00:00Z",
  "endDate": "2026-06-15T00:00:00Z",
  "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 2. Get Exam Terms

**GET** `/exams/terms`

Retrieve exam terms for an academic year.

**Authentication Required:** Yes

**Query Parameters:**
- `academicYearId` (guid, required): Academic year ID

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Mid-term Examination",
    "termType": 0,
    "startDate": "2026-06-01T00:00:00Z",
    "endDate": "2026-06-15T00:00:00Z",
    "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
]
```

---

#### 3. Create Grade Definition

**POST** `/exams/grades`

Define grading criteria.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "label": "A+",
  "minPercentage": 90,
  "maxPercentage": 100,
  "gradePoint": 10,
  "description": "Outstanding",
  "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 4. Get Grade Definitions

**GET** `/exams/grades`

Retrieve grade definitions.

**Authentication Required:** Yes

**Query Parameters:**
- `academicYearId` (guid, required): Academic year ID

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "label": "A+",
    "minPercentage": 90,
    "maxPercentage": 100,
    "gradePoint": 10,
    "description": "Outstanding",
    "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
]
```

---

#### 5. Create Exam

**POST** `/exams`

Schedule an exam.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin, Teacher

**Request Body:**
```json
{
  "examTermId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "subjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "examDate": "2026-06-05T09:00:00Z",
  "maxMarks": 100,
  "passingMarks": 40
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 6. Get Exams

**GET** `/exams`

Retrieve exams.

**Authentication Required:** Yes

**Query Parameters:**
- `examTermId` (guid, optional): Filter by exam term
- `classSectionId` (guid, optional): Filter by class section
- `subjectId` (guid, optional): Filter by subject

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "examTermId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "examTermName": "Mid-term Examination",
    "subjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "subjectName": "Mathematics",
    "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "classSectionName": "Grade 5-A",
    "examDate": "2026-06-05T09:00:00Z",
    "maxMarks": 100,
    "passingMarks": 40
  }
]
```

---

#### 7. Record Exam Result

**POST** `/exams/results`

Record student exam results.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin, Teacher

**Request Body:**
```json
{
  "examId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "marksObtained": 85,
  "remarks": "Good performance"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "examId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "studentName": "John Doe",
  "subjectName": "Mathematics",
  "marksObtained": 85,
  "maxMarks": 100,
  "percentage": 85,
  "gradeLabel": "A",
  "remarks": "Good performance"
}
```

---

#### 8. Get Exam Results

**GET** `/exams/results`

Retrieve exam results.

**Authentication Required:** Yes

**Query Parameters:**
- `examId` (guid, optional): Filter by exam
- `studentId` (guid, optional): Filter by student
- `examTermId` (guid, optional): Filter by exam term

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "examId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "studentName": "John Doe",
    "subjectName": "Mathematics",
    "marksObtained": 85,
    "maxMarks": 100,
    "percentage": 85,
    "gradeLabel": "A",
    "remarks": "Good performance"
  }
]
```

---

#### 9. Create Report Card Template

**POST** `/exams/report-card-templates`

Create a report card template.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "name": "Standard Report Card",
  "templateConfig": "{\"header\":\"School Name\",\"footer\":\"Principal Signature\"}",
  "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 10. Get Report Card

**GET** `/exams/report-card/{studentId}`

Generate student report card.

**Authentication Required:** Yes

**Query Parameters:**
- `examTermId` (guid, required): Exam term ID

**Response (200 OK):**
```json
{
  "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "studentName": "John Doe",
  "rollNumber": "2026001",
  "classSectionName": "Grade 5-A",
  "examTermName": "Mid-term Examination",
  "academicYear": "2026",
  "subjectResults": [
    {
      "subjectName": "Mathematics",
      "marksObtained": 85,
      "maxMarks": 100,
      "percentage": 85,
      "grade": "A",
      "remarks": "Good performance"
    },
    {
      "subjectName": "Science",
      "marksObtained": 78,
      "maxMarks": 100,
      "percentage": 78,
      "grade": "B+",
      "remarks": "Satisfactory"
    }
  ],
  "totalMarksObtained": 163,
  "totalMaxMarks": 200,
  "overallPercentage": 81.5,
  "overallGrade": "A"
}
```

---

### Timetable Endpoints

#### 1. Create Time Slot

**POST** `/timetable/time-slots`

Define a time slot for the timetable.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "name": "Period 1",
  "startTime": "08:00:00",
  "endTime": "08:45:00",
  "sortOrder": 1
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 2. Get Time Slots

**GET** `/timetable/time-slots`

Retrieve all time slots.

**Authentication Required:** Yes

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Period 1",
    "startTime": "08:00:00",
    "endTime": "08:45:00",
    "sortOrder": 1
  }
]
```

---

#### 3. Create Timetable Entry

**POST** `/timetable/entries`

Create a timetable entry.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "classSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "subjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "staffId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "dayOfWeek": 1,
  "timeSlotId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "room": "Room 101"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

#### 4. Update Timetable Entry

**PUT** `/timetable/entries/{id}`

Update a timetable entry.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "subjectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "staffId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "dayOfWeek": 1,
  "timeSlotId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "room": "Room 102"
}
```

**Response (204 No Content)**

---

#### 5. Delete Timetable Entry

**DELETE** `/timetable/entries/{id}`

Delete a timetable entry.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Response (204 No Content)**

---

#### 6. Get Timetable by Class

**GET** `/timetable/by-class`

Retrieve timetable for a class section.

**Authentication Required:** Yes

**Query Parameters:**
- `classSectionId` (guid, required): Class section ID
- `academicYearId` (guid, required): Academic year ID

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "dayOfWeek": 1,
    "timeSlotName": "Period 1",
    "startTime": "08:00:00",
    "endTime": "08:45:00",
    "subjectName": "Mathematics",
    "staffName": "Sarah Johnson",
    "room": "Room 101"
  }
]
```

---

#### 7. Get Teacher Timetable

**GET** `/timetable/by-teacher`

Retrieve timetable for a teacher.

**Authentication Required:** Yes

**Query Parameters:**
- `staffId` (guid, required): Staff ID
- `academicYearId` (guid, required): Academic year ID

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "dayOfWeek": 1,
    "timeSlotName": "Period 1",
    "startTime": "08:00:00",
    "endTime": "08:45:00",
    "subjectName": "Mathematics",
    "classSectionName": "Grade 5-A",
    "room": "Room 101"
  }
]
```

---

### Promotions Endpoints

#### 1. Bulk Promote Students

**POST** `/promotions/bulk`

Promote students from one class section to another.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Request Body:**
```json
{
  "from ClassSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "toClassSectionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "academicYearId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "studentIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "3fa85f64-5717-4562-b3fc-2c963f66afa7"
  ]
}
```

**Response (200 OK):**
```json
{
  "promotedCount": 2
}
```

---

#### 2. Get Promotion Preview

**GET** `/promotions/preview`

Preview students eligible for promotion.

**Authentication Required:** Yes  
**Roles:** SuperAdmin, Admin

**Query Parameters:**
- `fromClassSectionId` (guid, required): Current class section
- `academicYearId` (guid, required): Current academic year

**Response (200 OK):**
```json
[
  {
    "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "studentName": "John Doe",
    "rollNumber": "2026001",
    "currentClassSection": "Grade 5-A",
    "overallPercentage": 85.5,
    "isEligible": true
  }
]
```

---

## Data Models

### Student Related

#### StudentDto
```json
{
  "id": "guid",
  "firstName": "string",
  "lastName": "string",
  "dateOfBirth": "datetime",
  "gender": "int (0=Male, 1=Female, 2=Other)",
  "email": "string",
  "phone": "string (nullable)",
  "address": "string (nullable)",
  "rollNumber": "string",
  "admissionId": "string",
  "admissionDate": "datetime",
  "isActive": "bool",
  "classSectionId": "guid (nullable)",
  "classSectionName": "string (nullable)",
  "guardians": ["GuardianDto"],
  "documents": ["DocumentDto"]
}
```

#### GuardianDto
```json
{
  "id": "guid",
  "name": "string",
  "mobile": "string",
  "email": "string (nullable)",
  "relationship": "int (see GuardianRelationship enum)",
  "address": "string (nullable)",
  "occupation": "string (nullable)",
  "isPrimaryContact": "bool"
}
```

#### DocumentDto
```json
{
  "id": "guid",
  "documentType": "int (see DocumentType enum)",
  "fileName": "string",
  "filePath": "string",
  "contentType": "string",
  "fileSize": "long",
  "uploadedAt": "datetime"
}
```

### Staff Related

#### StaffDto
```json
{
  "id": "guid",
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phone": "string (nullable)",
  "qualification": "string (nullable)",
  "joiningDate": "datetime",
  "staffType": "int (see StaffType enum)",
  "isActive": "bool",
  "roles": ["StaffRoleDto"]
}
```

#### StaffRoleDto
```json
{
  "id": "guid",
  "role": "int (see StaffRoleType enum)",
  "classSectionId": "guid (nullable)",
  "classSectionName": "string (nullable)"
}
```

### Class Management Related

#### ClassDto
```json
{
  "id": "guid",
  "name": "string",
  "sortOrder": "int",
  "description": "string (nullable)",
  "sections": ["ClassSectionDto"]
}
```

#### ClassSectionDto
```json
{
  "id": "guid",
  "classId": "guid",
  "className": "string",
  "sectionId": "guid",
  "sectionName": "string",
  "displayName": "string",
  "capacity": "int",
  "studentCount": "int"
}
```

#### SubjectDto
```json
{
  "id": "guid",
  "name": "string",
  "code": "string",
  "description": "string (nullable)"
}
```

---

## Enumerations

### Gender
```
0 = Male
1 = Female
2 = Other
```

### GuardianRelationship
```
0 = Father
1 = Mother
2 = Guardian
3 = Other
```

### DocumentType
```
0 = BirthCertificate
1 = PreviousRecords
2 = TransferCertificate
3 = Photo
4 = Other
```

### StaffType
```
0 = Teacher
1 = Admin
2 = Support
```

### StaffRoleType
```
0 = ClassTeacher
1 = HOD (Head of Department)
2 = Admin
3 = SubjectTeacher
4 = Principal
```

### UserType
```
0 = Student
1 = Staff
2 = Admin
3 = Parent
```

### DayOfWeekEnum
```
1 = Monday
2 = Tuesday
3 = Wednesday
4 = Thursday
5 = Friday
6 = Saturday
7 = Sunday
```

### ExamTermType
```
0 = MidTerm
1 = Final
2 = Quarterly
3 = HalfYearly
```

---

## Best Practices

### 1. Authentication
- Always include the JWT token in the Authorization header
- Handle 401 (Unauthorized) responses by redirecting to login
- Refresh tokens when close to expiration

### 2. Error Handling
- Check the `errors` array in error responses
- Display validation errors to users
- Log 500 errors for debugging

### 3. Pagination
- Use `pageNumber` and `pageSize` query parameters for list endpoints
- Default values: pageNumber=1, pageSize=10
- Store pagination state in your frontend application

### 4. Search and Filters
- Use `searchTerm` for text-based searching
- Combine multiple filters (e.g., classSectionId + isActive)
- Debounce search inputs to reduce API calls

### 5. File Uploads
- Use `multipart/form-data` content type
- Validate file size and type on frontend before upload
- Handle upload progress for better UX

### 6. Dates
- Always send dates in ISO 8601 format
- Convert to local timezone for display
- Use date pickers with proper validation

### 7. GUIDs
- All IDs are GUIDs (UUID v4)
- Format: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
- Validate GUID format before API calls

---

## Sample Workflow: Student Admission

1. **Get available class sections**
   ```
   GET /classes/class-sections
   ```

2. **Admit new student**
   ```
   POST /students
   {
     "firstName": "John",
     "lastName": "Doe",
     ...
   }
   ```

3. **Add guardian**
   ```
   POST /students/{studentId}/guardians
   {
     "name": "Jane Doe",
     ...
   }
   ```

4. **Upload documents**
   ```
   POST /students/{studentId}/documents
   Content-Type: multipart/form-data
   ```

---

## Support

For API issues or questions:
- Check Swagger documentation at `/swagger` (Development only)
- Review error messages in response
- Contact backend team with request/response details

---

**Version History:**
- v1.0 (Feb 2026): Initial API documentation
