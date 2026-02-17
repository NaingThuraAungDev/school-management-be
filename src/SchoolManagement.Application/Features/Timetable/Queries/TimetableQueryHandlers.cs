using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Timetable;

namespace SchoolManagement.Application.Features.Timetable.Queries;

public class GetTimetableByClassQueryHandler : IRequestHandler<GetTimetableByClassQuery, Result<List<TimetableEntryDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTimetableByClassQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<TimetableEntryDto>>> Handle(GetTimetableByClassQuery request, CancellationToken cancellationToken)
    {
        var entries = await _context.TimetableEntries
            .Include(t => t.ClassSection).ThenInclude(cs => cs.Class)
            .Include(t => t.ClassSection).ThenInclude(cs => cs.Section)
            .Include(t => t.SubjectTeacherMapping).ThenInclude(m => m.Subject)
            .Include(t => t.SubjectTeacherMapping).ThenInclude(m => m.Staff)
            .Include(t => t.TimeSlot)
            .Where(t => t.ClassSectionId == request.ClassSectionId
                        && t.AcademicYearId == request.AcademicYearId
                        && !t.IsDeleted)
            .OrderBy(t => t.DayOfWeek).ThenBy(t => t.TimeSlot.SortOrder)
            .Select(t => new TimetableEntryDto
            {
                Id = t.Id,
                ClassSectionId = t.ClassSectionId,
                ClassSectionName = $"{t.ClassSection.Class.Name}-{t.ClassSection.Section.Name}",
                SubjectTeacherMappingId = t.SubjectTeacherMappingId,
                SubjectName = t.SubjectTeacherMapping.Subject.Name,
                TeacherName = $"{t.SubjectTeacherMapping.Staff.FirstName} {t.SubjectTeacherMapping.Staff.LastName}",
                TimeSlotId = t.TimeSlotId,
                TimeSlotLabel = t.TimeSlot.Label,
                DayOfWeek = t.DayOfWeek
            })
            .ToListAsync(cancellationToken);

        return Result<List<TimetableEntryDto>>.Success(entries);
    }
}

public class GetTeacherTimetableQueryHandler : IRequestHandler<GetTeacherTimetableQuery, Result<List<TimetableEntryDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTeacherTimetableQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<TimetableEntryDto>>> Handle(GetTeacherTimetableQuery request, CancellationToken cancellationToken)
    {
        var entries = await _context.TimetableEntries
            .Include(t => t.ClassSection).ThenInclude(cs => cs.Class)
            .Include(t => t.ClassSection).ThenInclude(cs => cs.Section)
            .Include(t => t.SubjectTeacherMapping).ThenInclude(m => m.Subject)
            .Include(t => t.SubjectTeacherMapping).ThenInclude(m => m.Staff)
            .Include(t => t.TimeSlot)
            .Where(t => t.SubjectTeacherMapping.StaffId == request.StaffId
                        && t.AcademicYearId == request.AcademicYearId
                        && !t.IsDeleted)
            .OrderBy(t => t.DayOfWeek).ThenBy(t => t.TimeSlot.SortOrder)
            .Select(t => new TimetableEntryDto
            {
                Id = t.Id,
                ClassSectionId = t.ClassSectionId,
                ClassSectionName = $"{t.ClassSection.Class.Name}-{t.ClassSection.Section.Name}",
                SubjectTeacherMappingId = t.SubjectTeacherMappingId,
                SubjectName = t.SubjectTeacherMapping.Subject.Name,
                TeacherName = $"{t.SubjectTeacherMapping.Staff.FirstName} {t.SubjectTeacherMapping.Staff.LastName}",
                TimeSlotId = t.TimeSlotId,
                TimeSlotLabel = t.TimeSlot.Label,
                DayOfWeek = t.DayOfWeek
            })
            .ToListAsync(cancellationToken);

        return Result<List<TimetableEntryDto>>.Success(entries);
    }
}

public class GetTimeSlotsQueryHandler : IRequestHandler<GetTimeSlotsQuery, Result<List<TimeSlotDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTimeSlotsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<TimeSlotDto>>> Handle(GetTimeSlotsQuery request, CancellationToken cancellationToken)
    {
        var slots = await _context.TimeSlots
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.SortOrder)
            .Select(t => new TimeSlotDto
            {
                Id = t.Id,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                Label = t.Label,
                SortOrder = t.SortOrder,
                IsBreak = t.IsBreak
            })
            .ToListAsync(cancellationToken);

        return Result<List<TimeSlotDto>>.Success(slots);
    }
}
