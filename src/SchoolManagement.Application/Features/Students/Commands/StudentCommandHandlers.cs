using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Students;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Students.Commands;

public class AdmitStudentCommandHandler : IRequestHandler<AdmitStudentCommand, Result<StudentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public AdmitStudentCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Result<StudentDto>> Handle(AdmitStudentCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingStudent = await _context.Students
            .AnyAsync(s => s.Email == request.Email && !s.IsDeleted, cancellationToken);
        if (existingStudent)
            return Result<StudentDto>.Failure("A student with this email already exists.");

        // Create Identity user
        var (userId, message) = await _identityService.CreateUserAsync(request.Email, request.Password, "Student");
        if (string.IsNullOrEmpty(userId))
            return Result<StudentDto>.Failure($"Failed to create user account: {message}");

        // Generate Admission ID and Roll Number
        var admissionId = await GenerateAdmissionIdAsync(cancellationToken);
        var rollNumber = await GenerateRollNumberAsync(request.ClassSectionId, request.AcademicYearId, cancellationToken);

        var student = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            RollNumber = rollNumber,
            AdmissionId = admissionId,
            ClassSectionId = request.ClassSectionId,
            AcademicYearId = request.AcademicYearId,
            UserId = userId
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);

        // Load related data for response
        var classSection = request.ClassSectionId.HasValue
            ? await _context.ClassSections
                .Include(cs => cs.Class)
                .Include(cs => cs.Section)
                .FirstOrDefaultAsync(cs => cs.Id == request.ClassSectionId.Value, cancellationToken)
            : null;

        return Result<StudentDto>.Success(new StudentDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            Email = student.Email,
            Phone = student.Phone,
            Address = student.Address,
            RollNumber = student.RollNumber,
            AdmissionId = student.AdmissionId,
            AdmissionDate = student.AdmissionDate,
            IsActive = student.IsActive,
            ClassSectionId = student.ClassSectionId,
            ClassSectionName = classSection != null ? $"{classSection.Class.Name}-{classSection.Section.Name}" : null
        }, "Student admitted successfully.");
    }

    private async Task<string> GenerateAdmissionIdAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _context.Students
            .CountAsync(s => s.AdmissionDate.Year == year, cancellationToken);
        return $"ADM-{year}-{(count + 1):D5}";
    }

    private async Task<string> GenerateRollNumberAsync(Guid? classSectionId, Guid? academicYearId, CancellationToken cancellationToken)
    {
        if (!classSectionId.HasValue)
            return "UNASSIGNED";

        var count = await _context.Students
            .CountAsync(s => s.ClassSectionId == classSectionId && s.AcademicYearId == academicYearId && !s.IsDeleted, cancellationToken);
        return $"{(count + 1):D3}";
    }
}

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result<StudentDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateStudentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (student == null)
            return Result<StudentDto>.Failure("Student not found.");

        // Check email uniqueness (exclude current student)
        var emailExists = await _context.Students
            .AnyAsync(s => s.Email == request.Email && s.Id != request.Id && !s.IsDeleted, cancellationToken);
        if (emailExists)
            return Result<StudentDto>.Failure("Another student with this email already exists.");

        student.FirstName = request.FirstName;
        student.LastName = request.LastName;
        student.DateOfBirth = request.DateOfBirth;
        student.Gender = request.Gender;
        student.Email = request.Email;
        student.Phone = request.Phone;
        student.Address = request.Address;
        student.ClassSectionId = request.ClassSectionId;
        student.IsActive = request.IsActive;
        student.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var classSection = request.ClassSectionId.HasValue
            ? await _context.ClassSections
                .Include(cs => cs.Class)
                .Include(cs => cs.Section)
                .FirstOrDefaultAsync(cs => cs.Id == request.ClassSectionId.Value, cancellationToken)
            : null;

        return Result<StudentDto>.Success(new StudentDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            Email = student.Email,
            Phone = student.Phone,
            Address = student.Address,
            RollNumber = student.RollNumber,
            AdmissionId = student.AdmissionId,
            AdmissionDate = student.AdmissionDate,
            IsActive = student.IsActive,
            ClassSectionId = student.ClassSectionId,
            ClassSectionName = classSection != null ? $"{classSection.Class.Name}-{classSection.Section.Name}" : null
        }, "Student updated successfully.");
    }
}

public class LinkGuardianCommandHandler : IRequestHandler<LinkGuardianCommand, Result<GuardianDto>>
{
    private readonly IApplicationDbContext _context;

    public LinkGuardianCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GuardianDto>> Handle(LinkGuardianCommand request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && !s.IsDeleted, cancellationToken);

        if (student == null)
            return Result<GuardianDto>.Failure("Student not found.");

        // Check if guardian with same mobile already exists
        var existingGuardian = await _context.Guardians
            .FirstOrDefaultAsync(g => g.Mobile == request.Mobile && !g.IsDeleted, cancellationToken);

        Guardian guardian;
        if (existingGuardian != null)
        {
            guardian = existingGuardian;
        }
        else
        {
            guardian = new Guardian
            {
                Name = request.Name,
                Mobile = request.Mobile,
                Email = request.Email,
                Relationship = request.Relationship,
                Address = request.Address,
                Occupation = request.Occupation
            };
            _context.Guardians.Add(guardian);
        }

        // Check if link already exists
        var linkExists = await _context.StudentGuardians
            .AnyAsync(sg => sg.StudentId == request.StudentId && sg.GuardianId == guardian.Id, cancellationToken);

        if (!linkExists)
        {
            var studentGuardian = new StudentGuardian
            {
                StudentId = request.StudentId,
                GuardianId = guardian.Id,
                IsPrimaryContact = request.IsPrimaryContact
            };
            _context.StudentGuardians.Add(studentGuardian);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<GuardianDto>.Success(new GuardianDto
        {
            Id = guardian.Id,
            Name = guardian.Name,
            Mobile = guardian.Mobile,
            Email = guardian.Email,
            Relationship = guardian.Relationship,
            Address = guardian.Address,
            Occupation = guardian.Occupation,
            IsPrimaryContact = request.IsPrimaryContact
        }, "Guardian linked successfully.");
    }
}

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<DocumentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public UploadDocumentCommandHandler(IApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<DocumentDto>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && !s.IsDeleted, cancellationToken);

        if (student == null)
            return Result<DocumentDto>.Failure("Student not found.");

        var filePath = await _fileStorageService.UploadFileAsync(
            request.File,
            $"documents/{request.StudentId}",
            cancellationToken);

        var document = new Document
        {
            StudentId = request.StudentId,
            DocumentType = request.DocumentType,
            FileName = request.File.FileName,
            FilePath = filePath,
            ContentType = request.File.ContentType,
            FileSize = request.File.Length
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<DocumentDto>.Success(new DocumentDto
        {
            Id = document.Id,
            DocumentType = document.DocumentType,
            FileName = document.FileName,
            FilePath = _fileStorageService.GetFileUrl(document.FilePath),
            ContentType = document.ContentType,
            FileSize = document.FileSize,
            UploadedAt = document.UploadedAt
        }, "Document uploaded successfully.");
    }
}

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteStudentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (student == null)
            return Result.Failure("Student not found.");

        student.IsDeleted = true;
        student.IsActive = false;
        student.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Student deleted successfully.");
    }
}
