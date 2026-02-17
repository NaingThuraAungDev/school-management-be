using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class StudentExamResult : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public decimal MarksObtained { get; set; }
    public decimal Percentage { get; set; }

    public Guid? GradeDefinitionId { get; set; }
    public GradeDefinition? GradeDefinition { get; set; }

    public string? Remarks { get; set; }
}
