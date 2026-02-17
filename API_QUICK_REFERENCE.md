# School Management API - Quick Reference

A concise reference guide for frontend integration.

## Base URL
```
Development: https://localhost:{port}/api
```

## Authentication
```http
Authorization: Bearer {jwt-token}
```

---

## Endpoints Summary

### Authentication
| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| POST | `/auth/login` | No | - | User login |
| POST | `/auth/change-password` | Yes | All | Change password |
| POST | `/auth/reset-password` | Yes | Admin | Reset user password |

### Students
| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| POST | `/students` | Yes | Admin | Admit student |
| PUT | `/students/{id}` | Yes | Admin | Update student |
| DELETE | `/students/{id}` | Yes | Admin | Delete student |
| GET | `/students/{id}` | Yes | All | Get student by ID |
| GET | `/students` | Yes | All | Get students list |
| POST | `/students/{studentId}/guardians` | Yes | Admin | Link guardian |
| POST | `/students/{studentId}/documents` | Yes | Admin | Upload document |

### Staff
| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| POST | `/staff` | Yes | Admin | Onboard staff |
| PUT | `/staff/{id}` | Yes | Admin | Update staff |
| DELETE | `/staff/{id}` | Yes | Admin | Delete staff |
| GET | `/staff/{id}` | Yes | All | Get staff by ID |
| GET | `/staff` | Yes | All | Get staff list |
| POST | `/staff/{staffId}/roles` | Yes | Admin | Assign role |

### Classes
| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| POST | `/classes` | Yes | Admin | Create class |
| GET | `/classes` | Yes | All | Get classes |
| POST | `/classes/sections` | Yes | Admin | Create section |
| GET | `/classes/sections` | Yes | All | Get sections |
| POST | `/classes/class-sections` | Yes | Admin | Create class-section |
| GET | `/classes/class-sections` | Yes | All | Get class-sections |

### Subjects
| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| POST | `/subjects` | Yes | Admin | Create subject |
| GET | `/subjects` | Yes | All | Get subjects |
| POST | `/subjects/teacher-mappings` | Yes | Admin | Map subject-teacher |
| GET | `/subjects/teacher-mappings` | Yes | All | Get mappings |

### Exams
| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| POST | `/exams/terms` | Yes | Admin | Create exam term |
| GET | `/exams/terms?academicYearId={id}` | Yes | All | Get exam terms |
| POST | `/exams/grades` | Yes | Admin | Create grade definition |
| GET | `/exams/grades?academicYearId={id}` | Yes | All | Get grade definitions |
| POST | `/exams` | Yes | Admin/Teacher | Create exam |
| GET | `/exams` | Yes | All | Get exams |
| POST | `/exams/results` | Yes | Admin/Teacher | Record result |
| GET | `/exams/results` | Yes | All | Get results |
| POST | `/exams/report-card-templates` | Yes | Admin | Create template |
| GET | `/exams/report-card/{studentId}` | Yes | All | Get report card |

### Timetable
| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| POST | `/timetable/time-slots` | Yes | Admin | Create time slot |
| GET | `/timetable/time-slots` | Yes | All | Get time slots |
| POST | `/timetable/entries` | Yes | Admin | Create entry |
| PUT | `/timetable/entries/{id}` | Yes | Admin | Update entry |
| DELETE | `/timetable/entries/{id}` | Yes | Admin | Delete entry |
| GET | `/timetable/by-class` | Yes | All | Get by class |
| GET | `/timetable/by-teacher` | Yes | All | Get by teacher |

### Promotions
| Method | Endpoint | Auth | Roles | Description |
|--------|----------|------|-------|-------------|
| POST | `/promotions/bulk` | Yes | Admin | Bulk promote |
| GET | `/promotions/preview` | Yes | Admin | Get preview |

---

## Common Request Patterns

### Pagination (Query Parameters)
```
pageNumber=1&pageSize=10
```

### Search (Query Parameters)
```
searchTerm=John
```

### Filter by Active Status
```
isActive=true
```

### Filter by Class Section
```
classSectionId=3fa85f64-5717-4562-b3fc-2c963f66afa6
```

---

## Response Patterns

### Success (200 OK)
```json
{
  "id": "...",
  "field": "value"
}
```

### Created (201 Created)
```json
{
  "id": "newly-created-guid"
}
```

### No Content (204)
No response body

### Error (400/401/404/500)
```json
{
  "errors": ["Error message"]
}
```

---

## Enumerations Quick Reference

### Gender
`0=Male, 1=Female, 2=Other`

### GuardianRelationship
`0=Father, 1=Mother, 2=Guardian, 3=Other`

### DocumentType
`0=BirthCertificate, 1=PreviousRecords, 2=TransferCertificate, 3=Photo, 4=Other`

### StaffType
`0=Teacher, 1=Admin, 2=Support`

### StaffRoleType
`0=ClassTeacher, 1=HOD, 2=Admin, 3=SubjectTeacher, 4=Principal`

### DayOfWeekEnum
`1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday, 7=Sunday`

### ExamTermType
`0=MidTerm, 1=Final, 2=Quarterly, 3=HalfYearly`

---

## TypeScript/JavaScript Example

### API Client Setup
```typescript
const API_BASE_URL = 'https://localhost:5001/api';

// Store token after login
const setAuthToken = (token: string) => {
  localStorage.setItem('authToken', token);
};

// Get token for requests
const getAuthToken = (): string | null => {
  return localStorage.getItem('authToken');
};

// Generic fetch wrapper
async function apiRequest<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getAuthToken();
  
  const config: RequestInit = {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
      ...options.headers,
    },
  };

  const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
  
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.errors?.join(', ') || 'Request failed');
  }

  if (response.status === 204) {
    return {} as T;
  }

  return response.json();
}
```

### Usage Examples

#### Login
```typescript
interface LoginRequest {
  email: string;
  password: string;
}

interface LoginResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  email: string;
  userType: string;
  roles: string[];
}

async function login(credentials: LoginRequest): Promise<LoginResponse> {
  const response = await apiRequest<LoginResponse>('/auth/login', {
    method: 'POST',
    body: JSON.stringify(credentials),
  });
  
  setAuthToken(response.token);
  return response;
}
```

#### Get Students List
```typescript
interface Student {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  rollNumber: string;
  admissionId: string;
  classSectionName: string | null;
  isActive: boolean;
}

async function getStudents(
  pageNumber = 1,
  pageSize = 10,
  searchTerm = ''
): Promise<Student[]> {
  const params = new URLSearchParams({
    pageNumber: pageNumber.toString(),
    pageSize: pageSize.toString(),
    ...(searchTerm && { searchTerm }),
  });

  return apiRequest<Student[]>(`/students?${params}`);
}
```

#### Create Student
```typescript
interface CreateStudentRequest {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  gender: number;
  email: string;
  password: string;
  phone?: string;
  address?: string;
  classSectionId?: string;
  academicYearId?: string;
}

async function createStudent(
  data: CreateStudentRequest
): Promise<Student> {
  return apiRequest<Student>('/students', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}
```

#### Upload Document
```typescript
async function uploadDocument(
  studentId: string,
  documentType: number,
  file: File
): Promise<{ documentId: string }> {
  const formData = new FormData();
  formData.append('studentId', studentId);
  formData.append('documentType', documentType.toString());
  formData.append('file', file);

  const token = getAuthToken();
  
  const response = await fetch(
    `${API_BASE_URL}/students/${studentId}/documents`,
    {
      method: 'POST',
      headers: {
        ...(token && { Authorization: `Bearer ${token}` }),
      },
      body: formData,
    }
  );

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.errors?.join(', ') || 'Upload failed');
  }

  return response.json();
}
```

---

## React Hooks Example

### useAuth Hook
```typescript
import { useState, useEffect } from 'react';

interface User {
  email: string;
  userType: string;
  roles: string[];
}

export function useAuth() {
  const [user, setUser] = useState<User | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const token = getAuthToken();
    if (token) {
      // Decode JWT to get user info (or fetch from /auth/me endpoint)
      // For now, just set authenticated
      setIsAuthenticated(true);
    }
  }, []);

  const login = async (email: string, password: string) => {
    const response = await apiRequest<LoginResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
    
    setAuthToken(response.token);
    setUser({
      email: response.email,
      userType: response.userType,
      roles: response.roles,
    });
    setIsAuthenticated(true);
    
    return response;
  };

  const logout = () => {
    localStorage.removeItem('authToken');
    setUser(null);
    setIsAuthenticated(false);
  };

  return { user, isAuthenticated, login, logout };
}
```

### useStudents Hook
```typescript
import { useState, useEffect } from 'react';

export function useStudents() {
  const [students, setStudents] = useState<Student[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchStudents = async (
    pageNumber = 1,
    pageSize = 10,
    searchTerm = ''
  ) => {
    setLoading(true);
    setError(null);
    
    try {
      const data = await getStudents(pageNumber, pageSize, searchTerm);
      setStudents(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch students');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStudents();
  }, []);

  return { students, loading, error, fetchStudents };
}
```

---

## Common Errors & Solutions

### 401 Unauthorized
- **Cause**: Missing or invalid token
- **Solution**: Redirect to login page, refresh token

### 400 Bad Request
- **Cause**: Validation errors
- **Solution**: Display `errors` array to user, fix form fields

### 403 Forbidden
- **Cause**: Insufficient permissions
- **Solution**: Check user roles, show permission denied message

### 404 Not Found
- **Cause**: Resource doesn't exist
- **Solution**: Handle gracefully, redirect to listing page

### 500 Internal Server Error
- **Cause**: Server-side error
- **Solution**: Log error, show generic error message, retry

---

## Tips for Frontend Integration

1. **Use TypeScript interfaces** for all API models
2. **Create reusable API service** functions
3. **Implement error boundaries** for React apps
4. **Show loading states** during API calls
5. **Debounce search inputs** (300-500ms)
6. **Cache frequently accessed data** (classes, sections, subjects)
7. **Handle file uploads** with progress indicators
8. **Validate forms** on frontend before API calls
9. **Store JWT securely** (HttpOnly cookies in production)
10. **Implement auto-logout** on token expiration

---

For detailed documentation, see [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)
