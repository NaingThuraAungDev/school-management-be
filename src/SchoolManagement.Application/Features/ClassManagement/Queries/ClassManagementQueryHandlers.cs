using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.ClassManagement;

namespace SchoolManagement.Application.Features.ClassManagement.Queries;

public class GetClassesQueryHandler : IRequestHandler<GetClassesQuery, Result<List<ClassDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetClassesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<ClassDto>>> Handle(GetClassesQuery request, CancellationToken cancellationToken)
    {
        var classes = await _context.Classes
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .Select(c => new ClassDto
            {
                Id = c.Id,
                Name = c.Name,
                SortOrder = c.SortOrder,
                Description = c.Description
            })
            .ToListAsync(cancellationToken);

        return Result<List<ClassDto>>.Success(classes);
    }
}

public class GetSectionsQueryHandler : IRequestHandler<GetSectionsQuery, Result<List<SectionDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetSectionsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<SectionDto>>> Handle(GetSectionsQuery request, CancellationToken cancellationToken)
    {
        var sections = await _context.Sections
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.SortOrder)
            .Select(s => new SectionDto
            {
                Id = s.Id,
                Name = s.Name,
                SortOrder = s.SortOrder
            })
            .ToListAsync(cancellationToken);

        return Result<List<SectionDto>>.Success(sections);
    }
}

public class GetClassSectionsQueryHandler : IRequestHandler<GetClassSectionsQuery, Result<List<ClassSectionDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetClassSectionsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<ClassSectionDto>>> Handle(GetClassSectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ClassSections
            .Include(cs => cs.Class)
            .Include(cs => cs.Section)
            .Where(cs => !cs.IsDeleted)
            .AsQueryable();

        if (request.ClassId.HasValue)
            query = query.Where(cs => cs.ClassId == request.ClassId.Value);

        var classSections = await query
            .OrderBy(cs => cs.Class.SortOrder).ThenBy(cs => cs.Section.SortOrder)
            .Select(cs => new ClassSectionDto
            {
                Id = cs.Id,
                ClassId = cs.ClassId,
                ClassName = cs.Class.Name,
                SectionId = cs.SectionId,
                SectionName = cs.Section.Name,
                DisplayName = $"{cs.Class.Name}-{cs.Section.Name}",
                Capacity = cs.Capacity,
                StudentCount = cs.Students.Count(s => !s.IsDeleted && s.IsActive)
            })
            .ToListAsync(cancellationToken);

        return Result<List<ClassSectionDto>>.Success(classSections);
    }
}

public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, Result<List<SubjectDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetSubjectsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<SubjectDto>>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        var subjects = await _context.Subjects
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Name)
            .Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Description = s.Description
            })
            .ToListAsync(cancellationToken);

        return Result<List<SubjectDto>>.Success(subjects);
    }
}

public class GetSubjectMappingsQueryHandler : IRequestHandler<GetSubjectMappingsQuery, Result<List<SubjectTeacherMappingDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetSubjectMappingsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<SubjectTeacherMappingDto>>> Handle(GetSubjectMappingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SubjectTeacherMappings
            .Include(m => m.Subject)
            .Include(m => m.Staff)
            .Include(m => m.ClassSection).ThenInclude(cs => cs.Class)
            .Include(m => m.ClassSection).ThenInclude(cs => cs.Section)
            .Where(m => !m.IsDeleted)
            .AsQueryable();

        if (request.ClassSectionId.HasValue)
            query = query.Where(m => m.ClassSectionId == request.ClassSectionId.Value);

        if (request.AcademicYearId.HasValue)
            query = query.Where(m => m.AcademicYearId == request.AcademicYearId.Value);

        var mappings = await query
            .Select(m => new SubjectTeacherMappingDto
            {
                Id = m.Id,
                SubjectId = m.SubjectId,
                SubjectName = m.Subject.Name,
                StaffId = m.StaffId,
                StaffName = $"{m.Staff.FirstName} {m.Staff.LastName}",
                ClassSectionId = m.ClassSectionId,
                ClassSectionName = $"{m.ClassSection.Class.Name}-{m.ClassSection.Section.Name}",
                AcademicYearId = m.AcademicYearId
            })
            .ToListAsync(cancellationToken);

        return Result<List<SubjectTeacherMappingDto>>.Success(mappings);
    }
}
