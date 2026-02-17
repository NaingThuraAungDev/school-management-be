using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Exams;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Features.Exams.Commands;

public record CreateExamTermCommand(string Name, ExamTermType TermType, DateTime StartDate, DateTime EndDate, Guid AcademicYearId) : IRequest<Result<ExamTermDto>>;
public record CreateGradeDefinitionCommand(string Label, decimal MinPercentage, decimal MaxPercentage, int GradePoint, string? Description, Guid AcademicYearId) : IRequest<Result<GradeDefinitionDto>>;
public record CreateExamCommand(Guid ExamTermId, Guid SubjectId, Guid ClassSectionId, DateTime ExamDate, decimal MaxMarks, decimal PassingMarks) : IRequest<Result<ExamDto>>;
public record RecordExamResultCommand(Guid ExamId, Guid StudentId, decimal MarksObtained, string? Remarks) : IRequest<Result<StudentExamResultDto>>;
public record CreateReportCardTemplateCommand(string Name, string TemplateConfig, Guid AcademicYearId) : IRequest<Result<ReportCardTemplateDto>>;
