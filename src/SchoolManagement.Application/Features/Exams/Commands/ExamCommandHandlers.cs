using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Exams;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Exams.Commands;

public class CreateExamTermCommandHandler : IRequestHandler<CreateExamTermCommand, Result<ExamTermDto>>
{
    private readonly IApplicationDbContext _context;
    public CreateExamTermCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<ExamTermDto>> Handle(CreateExamTermCommand request, CancellationToken cancellationToken)
    {
        var entity = new ExamTerm
        {
            Name = request.Name,
            TermType = request.TermType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AcademicYearId = request.AcademicYearId
        };

        _context.ExamTerms.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ExamTermDto>.Success(new ExamTermDto
        {
            Id = entity.Id,
            Name = entity.Name,
            TermType = entity.TermType,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            AcademicYearId = entity.AcademicYearId
        }, "Exam term created successfully.");
    }
}

public class CreateGradeDefinitionCommandHandler : IRequestHandler<CreateGradeDefinitionCommand, Result<GradeDefinitionDto>>
{
    private readonly IApplicationDbContext _context;
    public CreateGradeDefinitionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<GradeDefinitionDto>> Handle(CreateGradeDefinitionCommand request, CancellationToken cancellationToken)
    {
        var entity = new GradeDefinition
        {
            Label = request.Label,
            MinPercentage = request.MinPercentage,
            MaxPercentage = request.MaxPercentage,
            GradePoint = request.GradePoint,
            Description = request.Description,
            AcademicYearId = request.AcademicYearId
        };

        _context.GradeDefinitions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<GradeDefinitionDto>.Success(new GradeDefinitionDto
        {
            Id = entity.Id,
            Label = entity.Label,
            MinPercentage = entity.MinPercentage,
            MaxPercentage = entity.MaxPercentage,
            GradePoint = entity.GradePoint,
            Description = entity.Description,
            AcademicYearId = entity.AcademicYearId
        }, "Grade definition created successfully.");
    }
}

public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, Result<ExamDto>>
{
    private readonly IApplicationDbContext _context;
    public CreateExamCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<ExamDto>> Handle(CreateExamCommand request, CancellationToken cancellationToken)
    {
        var examTerm = await _context.ExamTerms.FirstOrDefaultAsync(e => e.Id == request.ExamTermId && !e.IsDeleted, cancellationToken);
        if (examTerm == null) return Result<ExamDto>.Failure("Exam term not found.");

        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == request.SubjectId && !s.IsDeleted, cancellationToken);
        if (subject == null) return Result<ExamDto>.Failure("Subject not found.");

        var classSection = await _context.ClassSections
            .Include(cs => cs.Class).Include(cs => cs.Section)
            .FirstOrDefaultAsync(cs => cs.Id == request.ClassSectionId && !cs.IsDeleted, cancellationToken);
        if (classSection == null) return Result<ExamDto>.Failure("Class-section not found.");

        var entity = new Exam
        {
            ExamTermId = request.ExamTermId,
            SubjectId = request.SubjectId,
            ClassSectionId = request.ClassSectionId,
            ExamDate = request.ExamDate,
            MaxMarks = request.MaxMarks,
            PassingMarks = request.PassingMarks
        };

        _context.Exams.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ExamDto>.Success(new ExamDto
        {
            Id = entity.Id,
            ExamTermId = examTerm.Id,
            ExamTermName = examTerm.Name,
            SubjectId = subject.Id,
            SubjectName = subject.Name,
            ClassSectionId = classSection.Id,
            ClassSectionName = $"{classSection.Class.Name}-{classSection.Section.Name}",
            ExamDate = entity.ExamDate,
            MaxMarks = entity.MaxMarks,
            PassingMarks = entity.PassingMarks
        }, "Exam created successfully.");
    }
}

public class RecordExamResultCommandHandler : IRequestHandler<RecordExamResultCommand, Result<StudentExamResultDto>>
{
    private readonly IApplicationDbContext _context;
    public RecordExamResultCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<StudentExamResultDto>> Handle(RecordExamResultCommand request, CancellationToken cancellationToken)
    {
        var exam = await _context.Exams
            .Include(e => e.Subject)
            .Include(e => e.ExamTerm)
            .FirstOrDefaultAsync(e => e.Id == request.ExamId && !e.IsDeleted, cancellationToken);
        if (exam == null) return Result<StudentExamResultDto>.Failure("Exam not found.");

        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId && !s.IsDeleted, cancellationToken);
        if (student == null) return Result<StudentExamResultDto>.Failure("Student not found.");

        if (request.MarksObtained > exam.MaxMarks)
            return Result<StudentExamResultDto>.Failure($"Marks obtained cannot exceed max marks ({exam.MaxMarks}).");

        // Calculate percentage and resolve grade
        var percentage = (request.MarksObtained / exam.MaxMarks) * 100;
        var grade = await _context.GradeDefinitions
            .Where(g => g.AcademicYearId == exam.ExamTerm.AcademicYearId && !g.IsDeleted)
            .Where(g => percentage >= g.MinPercentage && percentage <= g.MaxPercentage)
            .FirstOrDefaultAsync(cancellationToken);

        // Check if result already exists
        var existingResult = await _context.StudentExamResults
            .FirstOrDefaultAsync(r => r.ExamId == request.ExamId && r.StudentId == request.StudentId && !r.IsDeleted, cancellationToken);

        if (existingResult != null)
        {
            existingResult.MarksObtained = request.MarksObtained;
            existingResult.Percentage = percentage;
            existingResult.GradeDefinitionId = grade?.Id;
            existingResult.Remarks = request.Remarks;
            existingResult.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            existingResult = new StudentExamResult
            {
                ExamId = request.ExamId,
                StudentId = request.StudentId,
                MarksObtained = request.MarksObtained,
                Percentage = percentage,
                GradeDefinitionId = grade?.Id,
                Remarks = request.Remarks
            };
            _context.StudentExamResults.Add(existingResult);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<StudentExamResultDto>.Success(new StudentExamResultDto
        {
            Id = existingResult.Id,
            ExamId = exam.Id,
            StudentId = student.Id,
            StudentName = $"{student.FirstName} {student.LastName}",
            SubjectName = exam.Subject.Name,
            MarksObtained = request.MarksObtained,
            MaxMarks = exam.MaxMarks,
            Percentage = percentage,
            GradeLabel = grade?.Label,
            Remarks = request.Remarks
        }, "Exam result recorded successfully.");
    }
}

public class CreateReportCardTemplateCommandHandler : IRequestHandler<CreateReportCardTemplateCommand, Result<ReportCardTemplateDto>>
{
    private readonly IApplicationDbContext _context;
    public CreateReportCardTemplateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<ReportCardTemplateDto>> Handle(CreateReportCardTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = new ReportCardTemplate
        {
            Name = request.Name,
            TemplateConfig = request.TemplateConfig,
            AcademicYearId = request.AcademicYearId
        };

        _context.ReportCardTemplates.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ReportCardTemplateDto>.Success(new ReportCardTemplateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            TemplateConfig = entity.TemplateConfig,
            IsActive = entity.IsActive,
            AcademicYearId = entity.AcademicYearId
        }, "Report card template created successfully.");
    }
}
