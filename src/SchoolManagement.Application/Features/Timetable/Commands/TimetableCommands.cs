using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Timetable;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Features.Timetable.Commands;

public record CreateTimetableEntryCommand(
    Guid ClassSectionId,
    Guid SubjectTeacherMappingId,
    Guid TimeSlotId,
    DayOfWeekEnum DayOfWeek,
    Guid AcademicYearId
) : IRequest<Result<TimetableEntryDto>>;

public record UpdateTimetableEntryCommand(
    Guid Id,
    Guid SubjectTeacherMappingId,
    Guid TimeSlotId,
    DayOfWeekEnum DayOfWeek
) : IRequest<Result<TimetableEntryDto>>;

public record DeleteTimetableEntryCommand(Guid Id) : IRequest<Result>;

public record CreateTimeSlotCommand(
    TimeSpan StartTime,
    TimeSpan EndTime,
    string Label,
    int SortOrder,
    bool IsBreak = false
) : IRequest<Result<TimeSlotDto>>;
