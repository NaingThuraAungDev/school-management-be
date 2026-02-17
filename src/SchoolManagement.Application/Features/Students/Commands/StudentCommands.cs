using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Students;
using SchoolManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace SchoolManagement.Application.Features.Students.Commands;

public record AdmitStudentCommand(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string Email,
    string Password,
    string? Phone,
    string? Address,
    Guid? ClassSectionId,
    Guid? AcademicYearId
) : IRequest<Result<StudentDto>>;

public record UpdateStudentCommand(
    Guid Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string Email,
    string? Phone,
    string? Address,
    Guid? ClassSectionId,
    bool IsActive
) : IRequest<Result<StudentDto>>;

public record LinkGuardianCommand(
    Guid StudentId,
    string Name,
    string Mobile,
    string? Email,
    GuardianRelationship Relationship,
    string? Address,
    string? Occupation,
    bool IsPrimaryContact
) : IRequest<Result<GuardianDto>>;

public record UploadDocumentCommand(
    Guid StudentId,
    DocumentType DocumentType,
    IFormFile File
) : IRequest<Result<DocumentDto>>;

public record DeleteStudentCommand(Guid Id) : IRequest<Result>;
