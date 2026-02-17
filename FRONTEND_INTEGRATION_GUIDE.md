# API Integration Guide for Frontend Developers

Welcome! This guide will help you integrate the School Management System API into your frontend application.

## 📚 Documentation Files

| File | Purpose | Best For |
|------|---------|----------|
| [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) | Complete API reference | Detailed endpoint info, request/response examples |
| [API_QUICK_REFERENCE.md](./API_QUICK_REFERENCE.md) | Quick lookup guide | TypeScript/React examples, common patterns |
| [api-types.ts](./api-types.ts) | TypeScript definitions | Type safety, autocomplete in your IDE |
| [School-Management-API.postman_collection.json](./School-Management-API.postman_collection.json) | Postman collection | API testing, exploring endpoints |

## 🚀 Quick Start (5 Minutes)

### Step 1: Import TypeScript Types

Copy [api-types.ts](./api-types.ts) to your project:

```bash
# If using a shared types folder
cp api-types.ts src/types/

# Or in your API client folder
cp api-types.ts src/api/types.ts
```

### Step 2: Create API Client

```typescript
// src/api/client.ts
const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:5001/api';

export const apiClient = {
  async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const token = localStorage.getItem('authToken');
    
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
  },
};
```

### Step 3: Login Example

```typescript
// src/api/auth.ts
import { apiClient } from './client';
import { LoginRequest, LoginResponse } from './types';

export const authApi = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.request<LoginResponse>(
      '/auth/login',
      {
        method: 'POST',
        body: JSON.stringify(credentials),
      }
    );
    
    // Save token
    localStorage.setItem('authToken', response.token);
    
    return response;
  },
  
  logout() {
    localStorage.removeItem('authToken');
  },
};
```

### Step 4: Test Your Setup

```typescript
// Example usage
import { authApi } from './api/auth';

async function testLogin() {
  try {
    const response = await authApi.login({
      email: 'admin@school.com',
      password: 'Admin@123',
    });
    
    console.log('Login successful!', response);
  } catch (error) {
    console.error('Login failed:', error);
  }
}
```

## 📝 Testing with Postman

1. **Import Collection**
   - Open Postman
   - Click "Import" → Select `School-Management-API.postman_collection.json`

2. **Configure Base URL**
   - Collection → Variables tab
   - Update `base_url` if your API runs on a different port

3. **Login**
   - Run the "Authentication → Login" request
   - Token is automatically saved for subsequent requests

4. **Test Endpoints**
   - All requests now include the authentication token
   - Explore and test any endpoint

**Default Credentials:**
```
Email: admin@school.com
Password: Admin@123
```

## 🔑 Core Concepts

### Authentication Flow

```typescript
1. User logs in → POST /api/auth/login
2. Receive JWT token → Store in localStorage
3. Include token in all subsequent requests:
   Header: Authorization: Bearer {token}
4. On logout → Remove token from localStorage
```

### Error Handling

All error responses follow this format:

```typescript
{
  "errors": ["Error message 1", "Error message 2"]
}
```

Example error handling:

```typescript
try {
  const data = await apiClient.request('/students');
} catch (error) {
  if (error.message.includes('Unauthorized')) {
    // Redirect to login
  } else {
    // Show error to user
    showErrorToast(error.message);
  }
}
```

### Pagination

List endpoints support pagination:

```typescript
// GET /api/students?pageNumber=1&pageSize=10
const students = await apiClient.request<StudentListDto[]>(
  '/students?pageNumber=1&pageSize=10'
);
```

### File Uploads

Use FormData for file uploads:

```typescript
const formData = new FormData();
formData.append('studentId', studentId);
formData.append('documentType', '0'); // BirthCertificate
formData.append('file', file);

const response = await fetch(
  `${API_BASE_URL}/students/${studentId}/documents`,
  {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: formData,
  }
);
```

## 🎯 Common Tasks

### 1. Get List of Students

```typescript
import { StudentListDto, GetStudentsQueryParams } from './types';

async function getStudents(params?: GetStudentsQueryParams) {
  const queryString = new URLSearchParams(
    params as Record<string, string>
  ).toString();
  
  return apiClient.request<StudentListDto[]>(
    `/students?${queryString}`
  );
}

// Usage
const students = await getStudents({
  pageNumber: 1,
  pageSize: 20,
  searchTerm: 'John',
  isActive: true,
});
```

### 2. Create a Student

```typescript
import { AdmitStudentRequest, StudentDto } from './types';

async function createStudent(data: AdmitStudentRequest) {
  return apiClient.request<StudentDto>('/students', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

// Usage
const newStudent = await createStudent({
  firstName: 'John',
  lastName: 'Doe',
  dateOfBirth: '2010-05-15T00:00:00Z',
  gender: Gender.Male,
  email: 'john.doe@example.com',
  password: 'Student@123',
});
```

### 3. Get Student Details

```typescript
async function getStudentById(id: string) {
  return apiClient.request<StudentDto>(`/students/${id}`);
}

// Usage
const student = await getStudentById('3fa85f64-5717-4562-b3fc-2c963f66afa6');
console.log(student.guardians); // Access nested data
console.log(student.documents);
```

### 4. Update Student

```typescript
import { UpdateStudentRequest } from './types';

async function updateStudent(id: string, data: UpdateStudentRequest) {
  return apiClient.request(`/students/${id}`, {
    method: 'PUT',
    body: JSON.stringify({ ...data, id }),
  });
}
```

### 5. Delete Student

```typescript
async function deleteStudent(id: string) {
  return apiClient.request(`/students/${id}`, {
    method: 'DELETE',
  });
}
```

## 🎨 React Hooks Examples

### useAuth Hook

```typescript
import { useState, useEffect, useContext, createContext } from 'react';
import { LoginResponse } from './types';

interface AuthContextType {
  user: LoginResponse | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<LoginResponse | null>(null);

  const login = async (email: string, password: string) => {
    const response = await authApi.login({ email, password });
    setUser(response);
  };

  const logout = () => {
    authApi.logout();
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{
      user,
      login,
      logout,
      isAuthenticated: !!user,
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
}
```

### useStudents Hook

```typescript
import { useState, useEffect } from 'react';
import { StudentListDto } from './types';

export function useStudents() {
  const [students, setStudents] = useState<StudentListDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchStudents = async (searchTerm = '', page = 1) => {
    setLoading(true);
    setError(null);
    
    try {
      const data = await getStudents({
        searchTerm,
        pageNumber: page,
        pageSize: 20,
      });
      setStudents(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStudents();
  }, []);

  return { students, loading, error, refetch: fetchStudents };
}
```

## 📊 Complete API Modules Example

Create separate API modules for each feature:

```
src/api/
├── client.ts          # Base API client
├── types.ts           # TypeScript types (copied from api-types.ts)
├── auth.ts            # Authentication endpoints
├── students.ts        # Student endpoints
├── staff.ts           # Staff endpoints
├── classes.ts         # Class management endpoints
├── subjects.ts        # Subject endpoints
├── exams.ts           # Exam endpoints
├── timetable.ts       # Timetable endpoints
└── promotions.ts      # Promotion endpoints
```

### Example: students.ts

```typescript
import { apiClient } from './client';
import {
  StudentDto,
  StudentListDto,
  AdmitStudentRequest,
  UpdateStudentRequest,
  LinkGuardianRequest,
  GetStudentsQueryParams,
} from './types';

export const studentsApi = {
  getAll: (params?: GetStudentsQueryParams) => {
    const query = new URLSearchParams(params as any).toString();
    return apiClient.request<StudentListDto[]>(`/students?${query}`);
  },

  getById: (id: string) =>
    apiClient.request<StudentDto>(`/students/${id}`),

  create: (data: AdmitStudentRequest) =>
    apiClient.request<StudentDto>('/students', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  update: (id: string, data: UpdateStudentRequest) =>
    apiClient.request(`/students/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ ...data, id }),
    }),

  delete: (id: string) =>
    apiClient.request(`/students/${id}`, { method: 'DELETE' }),

  linkGuardian: (studentId: string, data: LinkGuardianRequest) =>
    apiClient.request(`/students/${studentId}/guardians`, {
      method: 'POST',
      body: JSON.stringify({ ...data, studentId }),
    }),

  uploadDocument: async (
    studentId: string,
    documentType: number,
    file: File
  ) => {
    const formData = new FormData();
    formData.append('studentId', studentId);
    formData.append('documentType', documentType.toString());
    formData.append('file', file);

    const token = localStorage.getItem('authToken');
    const response = await fetch(
      `${process.env.REACT_APP_API_URL}/students/${studentId}/documents`,
      {
        method: 'POST',
        headers: {
          ...(token && { Authorization: `Bearer ${token}` }),
        },
        body: formData,
      }
    );

    if (!response.ok) throw new Error('Upload failed');
    return response.json();
  },
};
```

## 🔍 Debugging Tips

1. **Check Network Tab**
   - Open browser DevTools → Network
   - Look for failed requests
   - Check request/response headers and body

2. **Verify Token**
   ```typescript
   console.log('Token:', localStorage.getItem('authToken'));
   ```

3. **Test in Postman First**
   - Ensure endpoint works in Postman
   - Compare request headers/body with your code

4. **Enable CORS**
   - The API has CORS enabled for development
   - If issues persist, check your request origin

5. **Check API Status**
   - Ensure the backend is running
   - Visit `https://localhost:{port}/swagger` to verify

## ⚠️ Common Pitfalls

1. **Date Format**
   - Always use ISO 8601: `2010-05-15T00:00:00Z`
   - Use `new Date().toISOString()` in JavaScript

2. **GUID Format**
   - All IDs are UUIDs
   - Format: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`

3. **Enum Values**
   - Send numeric values, not strings
   - Example: `gender: 0` not `gender: "Male"`

4. **File Uploads**
   - Don't set `Content-Type` header for FormData
   - Browser sets it automatically with boundary

5. **Authentication**
   - Check token expiration
   - Implement auto-logout on 401 responses

## 📞 Need Help?

1. **Check Documentation**
   - [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) - Full details
   - [API_QUICK_REFERENCE.md](./API_QUICK_REFERENCE.md) - Quick lookup

2. **Test in Postman**
   - Import the collection
   - Verify the endpoint works

3. **Check Swagger**
   - Visit `/swagger` in development
   - Interactive API documentation

4. **Contact Backend Team**
   - Provide request/response details
   - Share error messages

## 🎉 You're Ready!

You now have everything you need to integrate the School Management System API:

- ✅ TypeScript types for type safety
- ✅ Postman collection for testing
- ✅ Complete API documentation
- ✅ Code examples and best practices
- ✅ Common patterns and hooks

Start building your frontend with confidence! 🚀
