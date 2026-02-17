using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Timetable;

namespace SchoolManagement.Application.Features.Timetable.Queries;

public record GetTimetableByClassQuery(Guid ClassSectionId, Guid AcademicYearId) : IRequest<Result<List<TimetableEntryDto>>>;
public record GetTeacherTimetableQuery(Guid StaffId, Guid AcademicYearId) : IRequest<Result<List<TimetableEntryDto>>>;
public record GetTimeSlotsQuery() : IRequest<Result<List<TimeSlotDto>>>;
