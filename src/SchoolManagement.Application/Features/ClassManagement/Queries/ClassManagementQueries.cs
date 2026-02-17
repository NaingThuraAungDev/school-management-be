using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.ClassManagement;

namespace SchoolManagement.Application.Features.ClassManagement.Queries;

public record GetClassesQuery() : IRequest<Result<List<ClassDto>>>;
public record GetSectionsQuery() : IRequest<Result<List<SectionDto>>>;
public record GetClassSectionsQuery(Guid? ClassId = null) : IRequest<Result<List<ClassSectionDto>>>;
public record GetSubjectsQuery() : IRequest<Result<List<SubjectDto>>>;
public record GetSubjectMappingsQuery(Guid? ClassSectionId = null, Guid? AcademicYearId = null) : IRequest<Result<List<SubjectTeacherMappingDto>>>;
