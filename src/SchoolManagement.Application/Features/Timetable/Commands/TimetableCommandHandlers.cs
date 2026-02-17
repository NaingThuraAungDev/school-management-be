using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Timetable;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Features.Timetable.Commands;

public class CreateTimetableEntryCommandHandler : IRequestHandler<CreateTimetableEntryCommand, Result<TimetableEntryDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateTimetableEntryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<TimetableEntryDto>> Handle(CreateTimetableEntryCommand request, CancellationToken cancellationToken)
    {
        // Validate references
        var classSection = await _context.ClassSections
            .Include(cs => cs.Class).Include(cs => cs.Section)
            .FirstOrDefaultAsync(cs => cs.Id == request.ClassSectionId && !cs.IsDeleted, cancellationToken);
        if (classSection == null) return Result<TimetableEntryDto>.Failure("Class-section not found.");

        var mapping = await _context.SubjectTeacherMappings
            .Include(m => m.Subject).Include(m => m.Staff)
            .FirstOrDefaultAsync(m => m.Id == request.SubjectTeacherMappingId && !m.IsDeleted, cancellationToken);
        if (mapping == null) return Result<TimetableEntryDto>.Failure("Subject-teacher mapping not found.");

        var timeSlot = await _context.TimeSlots.FirstOrDefaultAsync(t => t.Id == request.TimeSlotId && !t.IsDeleted, cancellationToken);
        if (timeSlot == null) return Result<TimetableEntryDto>.Failure("Time slot not found.");

        // Clash detection: class-section clash
        var classClash = await _context.TimetableEntries
            .AnyAsync(t => t.ClassSectionId == request.ClassSectionId
                           && t.TimeSlotId == request.TimeSlotId
                           && t.DayOfWeek == request.DayOfWeek
                           && t.AcademicYearId == request.AcademicYearId
                           && !t.IsDeleted, cancellationToken);
        if (classClash)
            return Result<TimetableEntryDto>.Failure($"Clash: {classSection.Class.Name}-{classSection.Section.Name} already has a class at this time slot on {request.DayOfWeek}.");

        // Clash detection: teacher clash
        var teacherClash = await _context.TimetableEntries
            .Include(t => t.SubjectTeacherMapping)
            .AnyAsync(t => t.SubjectTeacherMapping.StaffId == mapping.StaffId
                           && t.TimeSlotId == request.TimeSlotId
                           && t.DayOfWeek == request.DayOfWeek
                           && t.AcademicYearId == request.AcademicYearId
                           && !t.IsDeleted, cancellationToken);
        if (teacherClash)
            return Result<TimetableEntryDto>.Failure($"Clash: {mapping.Staff.FirstName} {mapping.Staff.LastName} is already assigned to another class at this time slot on {request.DayOfWeek}.");

        var entry = new TimetableEntry
        {
            ClassSectionId = request.ClassSectionId,
            SubjectTeacherMappingId = request.SubjectTeacherMappingId,
            TimeSlotId = request.TimeSlotId,
            DayOfWeek = request.DayOfWeek,
            AcademicYearId = request.AcademicYearId
        };

        _context.TimetableEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<TimetableEntryDto>.Success(new TimetableEntryDto
        {
            Id = entry.Id,
            ClassSectionId = classSection.Id,
            ClassSectionName = $"{classSection.Class.Name}-{classSection.Section.Name}",
            SubjectTeacherMappingId = mapping.Id,
            SubjectName = mapping.Subject.Name,
            TeacherName = $"{mapping.Staff.FirstName} {mapping.Staff.LastName}",
            TimeSlotId = timeSlot.Id,
            TimeSlotLabel = timeSlot.Label,
            DayOfWeek = request.DayOfWeek
        }, "Timetable entry created successfully.");
    }
}

public class UpdateTimetableEntryCommandHandler : IRequestHandler<UpdateTimetableEntryCommand, Result<TimetableEntryDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateTimetableEntryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<TimetableEntryDto>> Handle(UpdateTimetableEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.TimetableEntries
            .Include(t => t.ClassSection).ThenInclude(cs => cs.Class)
            .Include(t => t.ClassSection).ThenInclude(cs => cs.Section)
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

        if (entry == null) return Result<TimetableEntryDto>.Failure("Timetable entry not found.");

        var mapping = await _context.SubjectTeacherMappings
            .Include(m => m.Subject).Include(m => m.Staff)
            .FirstOrDefaultAsync(m => m.Id == request.SubjectTeacherMappingId && !m.IsDeleted, cancellationToken);
        if (mapping == null) return Result<TimetableEntryDto>.Failure("Subject-teacher mapping not found.");

        var timeSlot = await _context.TimeSlots.FirstOrDefaultAsync(t => t.Id == request.TimeSlotId && !t.IsDeleted, cancellationToken);
        if (timeSlot == null) return Result<TimetableEntryDto>.Failure("Time slot not found.");

        // Clash detection (exclude current entry)
        var classClash = await _context.TimetableEntries
            .AnyAsync(t => t.Id != request.Id
                           && t.ClassSectionId == entry.ClassSectionId
                           && t.TimeSlotId == request.TimeSlotId
                           && t.DayOfWeek == request.DayOfWeek
                           && t.AcademicYearId == entry.AcademicYearId
                           && !t.IsDeleted, cancellationToken);
        if (classClash)
            return Result<TimetableEntryDto>.Failure("Clash: class-section already has a class at this time slot.");

        var teacherClash = await _context.TimetableEntries
            .Include(t => t.SubjectTeacherMapping)
            .AnyAsync(t => t.Id != request.Id
                           && t.SubjectTeacherMapping.StaffId == mapping.StaffId
                           && t.TimeSlotId == request.TimeSlotId
                           && t.DayOfWeek == request.DayOfWeek
                           && t.AcademicYearId == entry.AcademicYearId
                           && !t.IsDeleted, cancellationToken);
        if (teacherClash)
            return Result<TimetableEntryDto>.Failure("Clash: teacher is already assigned to another class at this time slot.");

        entry.SubjectTeacherMappingId = request.SubjectTeacherMappingId;
        entry.TimeSlotId = request.TimeSlotId;
        entry.DayOfWeek = request.DayOfWeek;
        entry.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<TimetableEntryDto>.Success(new TimetableEntryDto
        {
            Id = entry.Id,
            ClassSectionId = entry.ClassSectionId,
            ClassSectionName = $"{entry.ClassSection.Class.Name}-{entry.ClassSection.Section.Name}",
            SubjectTeacherMappingId = mapping.Id,
            SubjectName = mapping.Subject.Name,
            TeacherName = $"{mapping.Staff.FirstName} {mapping.Staff.LastName}",
            TimeSlotId = timeSlot.Id,
            TimeSlotLabel = timeSlot.Label,
            DayOfWeek = request.DayOfWeek
        }, "Timetable entry updated successfully.");
    }
}

public class DeleteTimetableEntryCommandHandler : IRequestHandler<DeleteTimetableEntryCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteTimetableEntryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result> Handle(DeleteTimetableEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.TimetableEntries
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

        if (entry == null) return Result.Failure("Timetable entry not found.");

        entry.IsDeleted = true;
        entry.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Timetable entry deleted successfully.");
    }
}

public class CreateTimeSlotCommandHandler : IRequestHandler<CreateTimeSlotCommand, Result<TimeSlotDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateTimeSlotCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<TimeSlotDto>> Handle(CreateTimeSlotCommand request, CancellationToken cancellationToken)
    {
        var entity = new TimeSlot
        {
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Label = request.Label,
            SortOrder = request.SortOrder,
            IsBreak = request.IsBreak
        };

        _context.TimeSlots.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<TimeSlotDto>.Success(new TimeSlotDto
        {
            Id = entity.Id,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            Label = entity.Label,
            SortOrder = entity.SortOrder,
            IsBreak = entity.IsBreak
        }, "Time slot created successfully.");
    }
}
