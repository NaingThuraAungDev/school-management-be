using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Exams;

namespace SchoolManagement.Application.Features.Exams.Queries;

public class GetExamTermsQueryHandler : IRequestHandler<GetExamTermsQuery, Result<List<ExamTermDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetExamTermsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<ExamTermDto>>> Handle(GetExamTermsQuery request, CancellationToken cancellationToken)
    {
        var terms = await _context.ExamTerms
            .Where(e => e.AcademicYearId == request.AcademicYearId && !e.IsDeleted)
            .OrderBy(e => e.StartDate)
            .Select(e => new ExamTermDto
            {
                Id = e.Id,
                Name = e.Name,
                TermType = e.TermType,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                AcademicYearId = e.AcademicYearId
            })
            .ToListAsync(cancellationToken);

        return Result<List<ExamTermDto>>.Success(terms);
    }
}

public class GetGradeDefinitionsQueryHandler : IRequestHandler<GetGradeDefinitionsQuery, Result<List<GradeDefinitionDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetGradeDefinitionsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<GradeDefinitionDto>>> Handle(GetGradeDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var grades = await _context.GradeDefinitions
            .Where(g => g.AcademicYearId == request.AcademicYearId && !g.IsDeleted)
            .OrderByDescending(g => g.MinPercentage)
            .Select(g => new GradeDefinitionDto
            {
                Id = g.Id,
                Label = g.Label,
                MinPercentage = g.MinPercentage,
                MaxPercentage = g.MaxPercentage,
                GradePoint = g.GradePoint,
                Description = g.Description,
                AcademicYearId = g.AcademicYearId
            })
            .ToListAsync(cancellationToken);

        return Result<List<GradeDefinitionDto>>.Success(grades);
    }
}

public class GetExamsQueryHandler : IRequestHandler<GetExamsQuery, Result<List<ExamDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetExamsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<ExamDto>>> Handle(GetExamsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Exams
            .Include(e => e.ExamTerm)
            .Include(e => e.Subject)
            .Include(e => e.ClassSection).ThenInclude(cs => cs.Class)
            .Include(e => e.ClassSection).ThenInclude(cs => cs.Section)
            .Where(e => !e.IsDeleted)
            .AsQueryable();

        if (request.ExamTermId.HasValue)
            query = query.Where(e => e.ExamTermId == request.ExamTermId.Value);

        if (request.ClassSectionId.HasValue)
            query = query.Where(e => e.ClassSectionId == request.ClassSectionId.Value);

        var exams = await query
            .OrderBy(e => e.ExamDate)
            .Select(e => new ExamDto
            {
                Id = e.Id,
                ExamTermId = e.ExamTermId,
                ExamTermName = e.ExamTerm.Name,
                SubjectId = e.SubjectId,
                SubjectName = e.Subject.Name,
                ClassSectionId = e.ClassSectionId,
                ClassSectionName = $"{e.ClassSection.Class.Name}-{e.ClassSection.Section.Name}",
                ExamDate = e.ExamDate,
                MaxMarks = e.MaxMarks,
                PassingMarks = e.PassingMarks
            })
            .ToListAsync(cancellationToken);

        return Result<List<ExamDto>>.Success(exams);
    }
}

public class GetExamResultsQueryHandler : IRequestHandler<GetExamResultsQuery, Result<List<StudentExamResultDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetExamResultsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<StudentExamResultDto>>> Handle(GetExamResultsQuery request, CancellationToken cancellationToken)
    {
        var results = await _context.StudentExamResults
            .Include(r => r.Exam).ThenInclude(e => e.Subject)
            .Include(r => r.Student)
            .Include(r => r.GradeDefinition)
            .Where(r => r.ExamId == request.ExamId && !r.IsDeleted)
            .OrderBy(r => r.Student.RollNumber)
            .Select(r => new StudentExamResultDto
            {
                Id = r.Id,
                ExamId = r.ExamId,
                StudentId = r.StudentId,
                StudentName = $"{r.Student.FirstName} {r.Student.LastName}",
                SubjectName = r.Exam.Subject.Name,
                MarksObtained = r.MarksObtained,
                MaxMarks = r.Exam.MaxMarks,
                Percentage = r.Percentage,
                GradeLabel = r.GradeDefinition != null ? r.GradeDefinition.Label : null,
                Remarks = r.Remarks
            })
            .ToListAsync(cancellationToken);

        return Result<List<StudentExamResultDto>>.Success(results);
    }
}

public class GetReportCardQueryHandler : IRequestHandler<GetReportCardQuery, Result<ReportCardDto>>
{
    private readonly IApplicationDbContext _context;
    public GetReportCardQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<ReportCardDto>> Handle(GetReportCardQuery request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .Include(s => s.ClassSection).ThenInclude(cs => cs!.Class)
            .Include(s => s.ClassSection).ThenInclude(cs => cs!.Section)
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && !s.IsDeleted, cancellationToken);

        if (student == null)
            return Result<ReportCardDto>.Failure("Student not found.");

        var examTerm = await _context.ExamTerms
            .Include(e => e.AcademicYear)
            .FirstOrDefaultAsync(e => e.Id == request.ExamTermId && !e.IsDeleted, cancellationToken);

        if (examTerm == null)
            return Result<ReportCardDto>.Failure("Exam term not found.");

        // Get all exam results for this student in this exam term
        var results = await _context.StudentExamResults
            .Include(r => r.Exam).ThenInclude(e => e.Subject)
            .Include(r => r.GradeDefinition)
            .Where(r => r.StudentId == request.StudentId
                        && r.Exam.ExamTermId == request.ExamTermId
                        && !r.IsDeleted)
            .ToListAsync(cancellationToken);

        var subjectResults = results.Select(r => new SubjectResultDto
        {
            SubjectName = r.Exam.Subject.Name,
            MarksObtained = r.MarksObtained,
            MaxMarks = r.Exam.MaxMarks,
            Percentage = r.Percentage,
            Grade = r.GradeDefinition?.Label,
            Remarks = r.Remarks
        }).ToList();

        var totalObtained = subjectResults.Sum(r => r.MarksObtained);
        var totalMax = subjectResults.Sum(r => r.MaxMarks);
        var overallPercentage = totalMax > 0 ? (totalObtained / totalMax) * 100 : 0;

        // Resolve overall grade
        var overallGrade = await _context.GradeDefinitions
            .Where(g => g.AcademicYearId == examTerm.AcademicYearId && !g.IsDeleted)
            .Where(g => overallPercentage >= g.MinPercentage && overallPercentage <= g.MaxPercentage)
            .Select(g => g.Label)
            .FirstOrDefaultAsync(cancellationToken);

        return Result<ReportCardDto>.Success(new ReportCardDto
        {
            StudentId = student.Id,
            StudentName = $"{student.FirstName} {student.LastName}",
            RollNumber = student.RollNumber,
            ClassSectionName = student.ClassSection != null
                ? $"{student.ClassSection.Class.Name}-{student.ClassSection.Section.Name}"
                : "Unassigned",
            ExamTermName = examTerm.Name,
            AcademicYear = examTerm.AcademicYear.Year,
            SubjectResults = subjectResults,
            TotalMarksObtained = totalObtained,
            TotalMaxMarks = totalMax,
            OverallPercentage = overallPercentage,
            OverallGrade = overallGrade
        });
    }
}
