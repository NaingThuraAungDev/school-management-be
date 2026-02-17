using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Exams;

namespace SchoolManagement.Application.Features.Exams.Queries;

public record GetExamTermsQuery(Guid AcademicYearId) : IRequest<Result<List<ExamTermDto>>>;
public record GetGradeDefinitionsQuery(Guid AcademicYearId) : IRequest<Result<List<GradeDefinitionDto>>>;
public record GetExamsQuery(Guid? ExamTermId = null, Guid? ClassSectionId = null) : IRequest<Result<List<ExamDto>>>;
public record GetExamResultsQuery(Guid ExamId) : IRequest<Result<List<StudentExamResultDto>>>;
public record GetReportCardQuery(Guid StudentId, Guid ExamTermId) : IRequest<Result<ReportCardDto>>;
