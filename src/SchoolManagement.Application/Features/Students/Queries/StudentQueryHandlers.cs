using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Students;

namespace SchoolManagement.Application.Features.Students.Queries;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStudentByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .Include(s => s.ClassSection).ThenInclude(cs => cs!.Class)
            .Include(s => s.ClassSection).ThenInclude(cs => cs!.Section)
            .Include(s => s.StudentGuardians).ThenInclude(sg => sg.Guardian)
            .Include(s => s.Documents)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (student == null)
            return Result<StudentDto>.Failure("Student not found.");

        var dto = new StudentDto
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
            ClassSectionName = student.ClassSection != null
                ? $"{student.ClassSection.Class.Name}-{student.ClassSection.Section.Name}"
                : null,
            Guardians = student.StudentGuardians.Select(sg => new GuardianDto
            {
                Id = sg.Guardian.Id,
                Name = sg.Guardian.Name,
                Mobile = sg.Guardian.Mobile,
                Email = sg.Guardian.Email,
                Relationship = sg.Guardian.Relationship,
                Address = sg.Guardian.Address,
                Occupation = sg.Guardian.Occupation,
                IsPrimaryContact = sg.IsPrimaryContact
            }).ToList(),
            Documents = student.Documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                FileName = d.FileName,
                FilePath = d.FilePath,
                ContentType = d.ContentType,
                FileSize = d.FileSize,
                UploadedAt = d.UploadedAt
            }).ToList()
        };

        return Result<StudentDto>.Success(dto);
    }
}

public class GetStudentsListQueryHandler : IRequestHandler<GetStudentsListQuery, Result<PaginatedList<StudentListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetStudentsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<StudentListDto>>> Handle(GetStudentsListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Students
            .Include(s => s.ClassSection).ThenInclude(cs => cs!.Class)
            .Include(s => s.ClassSection).ThenInclude(cs => cs!.Section)
            .Where(s => !s.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        if (request.ClassSectionId.HasValue)
            query = query.Where(s => s.ClassSectionId == request.ClassSectionId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.FirstName.ToLower().Contains(term) ||
                s.LastName.ToLower().Contains(term) ||
                s.Email.ToLower().Contains(term) ||
                s.RollNumber.ToLower().Contains(term) ||
                s.AdmissionId.ToLower().Contains(term));
        }

        var projectedQuery = query
            .OrderBy(s => s.RollNumber)
            .Select(s => new StudentListDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                RollNumber = s.RollNumber,
                AdmissionId = s.AdmissionId,
                ClassSectionName = s.ClassSection != null
                    ? $"{s.ClassSection.Class.Name}-{s.ClassSection.Section.Name}"
                    : null,
                IsActive = s.IsActive
            });

        var result = await PaginatedList<StudentListDto>.CreateAsync(
            projectedQuery, request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<StudentListDto>>.Success(result);
    }
}
