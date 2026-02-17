using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.ClassManagement;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.ClassManagement.Commands;

public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, Result<ClassDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateClassCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<ClassDto>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Classes.AnyAsync(c => c.Name == request.Name && !c.IsDeleted, cancellationToken);
        if (exists)
            return Result<ClassDto>.Failure("A class with this name already exists.");

        var entity = new Class
        {
            Name = request.Name,
            SortOrder = request.SortOrder,
            Description = request.Description
        };

        _context.Classes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ClassDto>.Success(new ClassDto
        {
            Id = entity.Id,
            Name = entity.Name,
            SortOrder = entity.SortOrder,
            Description = entity.Description
        }, "Class created successfully.");
    }
}

public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, Result<SectionDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateSectionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<SectionDto>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Sections.AnyAsync(s => s.Name == request.Name && !s.IsDeleted, cancellationToken);
        if (exists)
            return Result<SectionDto>.Failure("A section with this name already exists.");

        var entity = new Section
        {
            Name = request.Name,
            SortOrder = request.SortOrder
        };

        _context.Sections.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<SectionDto>.Success(new SectionDto
        {
            Id = entity.Id,
            Name = entity.Name,
            SortOrder = entity.SortOrder
        }, "Section created successfully.");
    }
}

public class CreateClassSectionCommandHandler : IRequestHandler<CreateClassSectionCommand, Result<ClassSectionDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateClassSectionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<ClassSectionDto>> Handle(CreateClassSectionCommand request, CancellationToken cancellationToken)
    {
        var classEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.Id == request.ClassId && !c.IsDeleted, cancellationToken);
        if (classEntity == null)
            return Result<ClassSectionDto>.Failure("Class not found.");

        var section = await _context.Sections
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && !s.IsDeleted, cancellationToken);
        if (section == null)
            return Result<ClassSectionDto>.Failure("Section not found.");

        var exists = await _context.ClassSections
            .AnyAsync(cs => cs.ClassId == request.ClassId && cs.SectionId == request.SectionId && !cs.IsDeleted, cancellationToken);
        if (exists)
            return Result<ClassSectionDto>.Failure("This class-section combination already exists.");

        var entity = new ClassSection
        {
            ClassId = request.ClassId,
            SectionId = request.SectionId,
            Capacity = request.Capacity
        };

        _context.ClassSections.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ClassSectionDto>.Success(new ClassSectionDto
        {
            Id = entity.Id,
            ClassId = classEntity.Id,
            ClassName = classEntity.Name,
            SectionId = section.Id,
            SectionName = section.Name,
            DisplayName = $"{classEntity.Name}-{section.Name}",
            Capacity = entity.Capacity,
            StudentCount = 0
        }, "Class-section created successfully.");
    }
}

public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, Result<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateSubjectCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<SubjectDto>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Subjects
            .AnyAsync(s => (s.Name == request.Name || s.Code == request.Code) && !s.IsDeleted, cancellationToken);
        if (exists)
            return Result<SubjectDto>.Failure("A subject with this name or code already exists.");

        var entity = new Subject
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description
        };

        _context.Subjects.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<SubjectDto>.Success(new SubjectDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description
        }, "Subject created successfully.");
    }
}

public class MapSubjectTeacherCommandHandler : IRequestHandler<MapSubjectTeacherCommand, Result<SubjectTeacherMappingDto>>
{
    private readonly IApplicationDbContext _context;

    public MapSubjectTeacherCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<SubjectTeacherMappingDto>> Handle(MapSubjectTeacherCommand request, CancellationToken cancellationToken)
    {
        // Validate references exist
        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == request.SubjectId && !s.IsDeleted, cancellationToken);
        if (subject == null) return Result<SubjectTeacherMappingDto>.Failure("Subject not found.");

        var staff = await _context.StaffMembers.FirstOrDefaultAsync(s => s.Id == request.StaffId && !s.IsDeleted, cancellationToken);
        if (staff == null) return Result<SubjectTeacherMappingDto>.Failure("Staff member not found.");

        var classSection = await _context.ClassSections
            .Include(cs => cs.Class).Include(cs => cs.Section)
            .FirstOrDefaultAsync(cs => cs.Id == request.ClassSectionId && !cs.IsDeleted, cancellationToken);
        if (classSection == null) return Result<SubjectTeacherMappingDto>.Failure("Class-section not found.");

        // Check duplicate mapping
        var exists = await _context.SubjectTeacherMappings
            .AnyAsync(m => m.SubjectId == request.SubjectId && m.StaffId == request.StaffId
                           && m.ClassSectionId == request.ClassSectionId && m.AcademicYearId == request.AcademicYearId
                           && !m.IsDeleted, cancellationToken);
        if (exists)
            return Result<SubjectTeacherMappingDto>.Failure("This subject-teacher-class mapping already exists.");

        var entity = new SubjectTeacherMapping
        {
            SubjectId = request.SubjectId,
            StaffId = request.StaffId,
            ClassSectionId = request.ClassSectionId,
            AcademicYearId = request.AcademicYearId
        };

        _context.SubjectTeacherMappings.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<SubjectTeacherMappingDto>.Success(new SubjectTeacherMappingDto
        {
            Id = entity.Id,
            SubjectId = subject.Id,
            SubjectName = subject.Name,
            StaffId = staff.Id,
            StaffName = $"{staff.FirstName} {staff.LastName}",
            ClassSectionId = classSection.Id,
            ClassSectionName = $"{classSection.Class.Name}-{classSection.Section.Name}",
            AcademicYearId = request.AcademicYearId
        }, "Subject-teacher mapping created successfully.");
    }
}
