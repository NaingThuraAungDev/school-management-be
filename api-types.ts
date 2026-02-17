/**
 * School Management System - TypeScript Type Definitions
 * Auto-generated type definitions for API integration
 * Version: 1.0
 * Last Updated: February 2026
 */

// ============================================================================
// ENUMERATIONS
// ============================================================================

export enum Gender {
    Male = 0,
    Female = 1,
    Other = 2,
}

export enum GuardianRelationship {
    Father = 0,
    Mother = 1,
    Guardian = 2,
    Other = 3,
}

export enum DocumentType {
    BirthCertificate = 0,
    PreviousRecords = 1,
    TransferCertificate = 2,
    Photo = 3,
    Other = 4,
}

export enum StaffType {
    Teacher = 0,
    Admin = 1,
    Support = 2,
}

export enum StaffRoleType {
    ClassTeacher = 0,
    HOD = 1,
    Admin = 2,
    SubjectTeacher = 3,
    Principal = 4,
}

export enum UserType {
    Student = 0,
    Staff = 1,
    Admin = 2,
    Parent = 3,
}

export enum DayOfWeekEnum {
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7,
}

export enum ExamTermType {
    MidTerm = 0,
    Final = 1,
    Quarterly = 2,
    HalfYearly = 3,
}

// ============================================================================
// AUTHENTICATION
// ============================================================================

export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    token: string;
    refreshToken: string;
    expiresAt: string;
    email: string;
    userType: string;
    roles: string[];
}

export interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
}

export interface ResetPasswordRequest {
    userId: string;
    newPassword: string;
}

export interface AcademicYearDto {
    id: string;
    year: string;
    startDate: string;
    endDate: string;
    isCurrent: boolean;
}

// ============================================================================
// STUDENTS
// ============================================================================

export interface StudentDto {
    id: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    gender: Gender;
    email: string;
    phone?: string;
    address?: string;
    rollNumber: string;
    admissionId: string;
    admissionDate: string;
    isActive: boolean;
    classSectionId?: string;
    classSectionName?: string;
    guardians: GuardianDto[];
    documents: DocumentDto[];
}

export interface StudentListDto {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    rollNumber: string;
    admissionId: string;
    classSectionName?: string;
    isActive: boolean;
}

export interface GuardianDto {
    id: string;
    name: string;
    mobile: string;
    email?: string;
    relationship: GuardianRelationship;
    address?: string;
    occupation?: string;
    isPrimaryContact: boolean;
}

export interface DocumentDto {
    id: string;
    documentType: DocumentType;
    fileName: string;
    filePath: string;
    contentType: string;
    fileSize: number;
    uploadedAt: string;
}

export interface AdmitStudentRequest {
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    gender: Gender;
    email: string;
    password: string;
    phone?: string;
    address?: string;
    classSectionId?: string;
    academicYearId?: string;
}

export interface UpdateStudentRequest {
    id: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    gender: Gender;
    email: string;
    phone?: string;
    address?: string;
    classSectionId?: string;
    isActive: boolean;
}

export interface LinkGuardianRequest {
    studentId: string;
    name: string;
    mobile: string;
    email?: string;
    relationship: GuardianRelationship;
    address?: string;
    occupation?: string;
    isPrimaryContact: boolean;
}

export interface UploadDocumentRequest {
    studentId: string;
    documentType: DocumentType;
    file: File;
}

// ============================================================================
// STAFF
// ============================================================================

export interface StaffDto {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    qualification?: string;
    joiningDate: string;
    staffType: StaffType;
    isActive: boolean;
    roles: StaffRoleDto[];
}

export interface StaffListDto {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    staffType: StaffType;
    isActive: boolean;
}

export interface StaffRoleDto {
    id: string;
    role: StaffRoleType;
    classSectionId?: string;
    classSectionName?: string;
}

export interface OnboardStaffRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    phone?: string;
    qualification?: string;
    joiningDate: string;
    staffType: StaffType;
}

export interface UpdateStaffRequest {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    qualification?: string;
    staffType: StaffType;
    isActive: boolean;
}

export interface AssignStaffRoleRequest {
    staffId: string;
    role: StaffRoleType;
    classSectionId?: string;
    academicYearId: string;
}

// ============================================================================
// CLASS MANAGEMENT
// ============================================================================

export interface ClassDto {
    id: string;
    name: string;
    sortOrder: number;
    description?: string;
    sections: ClassSectionDto[];
}

export interface SectionDto {
    id: string;
    name: string;
    sortOrder: number;
}

export interface ClassSectionDto {
    id: string;
    classId: string;
    className: string;
    sectionId: string;
    sectionName: string;
    displayName: string;
    capacity: number;
    studentCount: number;
}

export interface SubjectDto {
    id: string;
    name: string;
    code: string;
    description?: string;
}

export interface SubjectTeacherMappingDto {
    id: string;
    subjectId: string;
    subjectName: string;
    staffId: string;
    staffName: string;
    classSectionId: string;
    classSectionName: string;
    academicYearId: string;
}

export interface CreateClassRequest {
    name: string;
    sortOrder: number;
    description?: string;
}

export interface CreateSectionRequest {
    name: string;
    sortOrder: number;
}

export interface CreateClassSectionRequest {
    classId: string;
    sectionId: string;
    academicYearId: string;
    capacity: number;
}

export interface CreateSubjectRequest {
    name: string;
    code: string;
    description?: string;
}

export interface MapSubjectTeacherRequest {
    subjectId: string;
    staffId: string;
    classSectionId: string;
    academicYearId: string;
}

// ============================================================================
// EXAMS
// ============================================================================

export interface ExamTermDto {
    id: string;
    name: string;
    termType: ExamTermType;
    startDate: string;
    endDate: string;
    academicYearId: string;
}

export interface GradeDefinitionDto {
    id: string;
    label: string;
    minPercentage: number;
    maxPercentage: number;
    gradePoint: number;
    description?: string;
    academicYearId: string;
}

export interface ExamDto {
    id: string;
    examTermId: string;
    examTermName: string;
    subjectId: string;
    subjectName: string;
    classSectionId: string;
    classSectionName: string;
    examDate: string;
    maxMarks: number;
    passingMarks: number;
}

export interface StudentExamResultDto {
    id: string;
    examId: string;
    studentId: string;
    studentName: string;
    subjectName: string;
    marksObtained: number;
    maxMarks: number;
    percentage: number;
    gradeLabel?: string;
    remarks?: string;
}

export interface SubjectResultDto {
    subjectName: string;
    marksObtained: number;
    maxMarks: number;
    percentage: number;
    grade?: string;
    remarks?: string;
}

export interface ReportCardDto {
    studentId: string;
    studentName: string;
    rollNumber: string;
    classSectionName: string;
    examTermName: string;
    academicYear: string;
    subjectResults: SubjectResultDto[];
    totalMarksObtained: number;
    totalMaxMarks: number;
    overallPercentage: number;
    overallGrade?: string;
}

export interface ReportCardTemplateDto {
    id: string;
    name: string;
    templateConfig: string;
    isActive: boolean;
    academicYearId: string;
}

export interface CreateExamTermRequest {
    name: string;
    termType: ExamTermType;
    startDate: string;
    endDate: string;
    academicYearId: string;
}

export interface CreateGradeDefinitionRequest {
    label: string;
    minPercentage: number;
    maxPercentage: number;
    gradePoint: number;
    description?: string;
    academicYearId: string;
}

export interface CreateExamRequest {
    examTermId: string;
    subjectId: string;
    classSectionId: string;
    examDate: string;
    maxMarks: number;
    passingMarks: number;
}

export interface RecordExamResultRequest {
    examId: string;
    studentId: string;
    marksObtained: number;
    remarks?: string;
}

export interface CreateReportCardTemplateRequest {
    name: string;
    templateConfig: string;
    academicYearId: string;
}

// ============================================================================
// TIMETABLE
// ============================================================================

export interface TimeSlotDto {
    id: string;
    name: string;
    startTime: string;
    endTime: string;
    sortOrder: number;
}

export interface TimetableEntryDto {
    id: string;
    dayOfWeek: DayOfWeekEnum;
    timeSlotName: string;
    startTime: string;
    endTime: string;
    subjectName: string;
    staffName?: string;
    classSectionName?: string;
    room?: string;
}

export interface CreateTimeSlotRequest {
    name: string;
    startTime: string;
    endTime: string;
    sortOrder: number;
}

export interface CreateTimetableEntryRequest {
    classSectionId: string;
    academicYearId: string;
    subjectId: string;
    staffId: string;
    dayOfWeek: DayOfWeekEnum;
    timeSlotId: string;
    room?: string;
}

export interface UpdateTimetableEntryRequest {
    id: string;
    subjectId: string;
    staffId: string;
    dayOfWeek: DayOfWeekEnum;
    timeSlotId: string;
    room?: string;
}

// ============================================================================
// PROMOTIONS
// ============================================================================

export interface PromotionPreviewDto {
    studentId: string;
    studentName: string;
    rollNumber: string;
    currentClassSection: string;
    overallPercentage: number;
    isEligible: boolean;
}

export interface BulkPromoteStudentsRequest {
    fromClassSectionId: string;
    toClassSectionId: string;
    academicYearId: string;
    studentIds: string[];
}

// ============================================================================
// COMMON TYPES
// ============================================================================

export interface ApiError {
    errors: string[];
}

export interface PaginationParams {
    pageNumber?: number;
    pageSize?: number;
}

export interface SearchParams extends PaginationParams {
    searchTerm?: string;
}

// ============================================================================
// API RESPONSE HELPERS
// ============================================================================

export type ApiResponse<T> = T | ApiError;

export function isApiError(response: any): response is ApiError {
    return response && Array.isArray(response.errors);
}

export function isSuccess<T>(response: ApiResponse<T>): response is T {
    return !isApiError(response);
}

// ============================================================================
// QUERY PARAMETERS
// ============================================================================

export interface GetStudentsQueryParams extends SearchParams {
    classSectionId?: string;
    isActive?: boolean;
}

export interface GetStaffQueryParams extends SearchParams {
    staffType?: StaffType;
    isActive?: boolean;
}

export interface GetClassSectionsQueryParams {
    classId?: string;
}

export interface GetSubjectMappingsQueryParams {
    classSectionId?: string;
    academicYearId?: string;
}

export interface GetExamTermsQueryParams {
    academicYearId: string;
}

export interface GetGradeDefinitionsQueryParams {
    academicYearId: string;
}

export interface GetExamsQueryParams {
    examTermId?: string;
    classSectionId?: string;
    subjectId?: string;
}

export interface GetExamResultsQueryParams {
    examId?: string;
    studentId?: string;
    examTermId?: string;
}

export interface GetReportCardQueryParams {
    examTermId: string;
}

export interface GetTimetableByClassQueryParams {
    classSectionId: string;
    academicYearId: string;
}

export interface GetTeacherTimetableQueryParams {
    staffId: string;
    academicYearId: string;
}

export interface GetPromotionPreviewQueryParams {
    fromClassSectionId: string;
    academicYearId: string;
}

// ============================================================================
// UTILITY TYPES
// ============================================================================

export type RequiredFields<T, K extends keyof T> = T & Required<Pick<T, K>>;
export type OptionalFields<T, K extends keyof T> = Omit<T, K> & Partial<Pick<T, K>>;

// Helper to create a student with required fields only
export type CreateStudentMinimal = Pick<
    AdmitStudentRequest,
    'firstName' | 'lastName' | 'dateOfBirth' | 'gender' | 'email' | 'password'
>;

// Helper to create a staff with required fields only
export type CreateStaffMinimal = Pick<
    OnboardStaffRequest,
    'firstName' | 'lastName' | 'email' | 'password' | 'joiningDate' | 'staffType'
>;
