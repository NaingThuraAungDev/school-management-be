using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.DTOs.Exams;

public class ExamTermDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExamTermType TermType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid AcademicYearId { get; set; }
}

public class GradeDefinitionDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal MinPercentage { get; set; }
    public decimal MaxPercentage { get; set; }
    public int GradePoint { get; set; }
    public string? Description { get; set; }
    public Guid AcademicYearId { get; set; }
}

public class ExamDto
{
    public Guid Id { get; set; }
    public Guid ExamTermId { get; set; }
    public string ExamTermName { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public Guid ClassSectionId { get; set; }
    public string ClassSectionName { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal PassingMarks { get; set; }
}

public class StudentExamResultDto
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public decimal MarksObtained { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal Percentage { get; set; }
    public string? GradeLabel { get; set; }
    public string? Remarks { get; set; }
}

public class ReportCardDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public string ClassSectionName { get; set; } = string.Empty;
    public string ExamTermName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public List<SubjectResultDto> SubjectResults { get; set; } = new();
    public decimal TotalMarksObtained { get; set; }
    public decimal TotalMaxMarks { get; set; }
    public decimal OverallPercentage { get; set; }
    public string? OverallGrade { get; set; }
}

public class SubjectResultDto
{
    public string SubjectName { get; set; } = string.Empty;
    public decimal MarksObtained { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal Percentage { get; set; }
    public string? Grade { get; set; }
    public string? Remarks { get; set; }
}

public class ReportCardTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TemplateConfig { get; set; } = "{}";
    public bool IsActive { get; set; }
    public Guid AcademicYearId { get; set; }
}
