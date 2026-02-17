using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.ClassManagement;

namespace SchoolManagement.Application.Features.ClassManagement.Commands;

public record CreateClassCommand(string Name, int SortOrder, string? Description) : IRequest<Result<ClassDto>>;
public record CreateSectionCommand(string Name, int SortOrder) : IRequest<Result<SectionDto>>;
public record CreateClassSectionCommand(Guid ClassId, Guid SectionId, int Capacity = 40) : IRequest<Result<ClassSectionDto>>;
public record CreateSubjectCommand(string Name, string Code, string? Description) : IRequest<Result<SubjectDto>>;
public record MapSubjectTeacherCommand(Guid SubjectId, Guid StaffId, Guid ClassSectionId, Guid AcademicYearId) : IRequest<Result<SubjectTeacherMappingDto>>;
