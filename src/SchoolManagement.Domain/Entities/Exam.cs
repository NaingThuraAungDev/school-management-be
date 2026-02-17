using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Exam : BaseEntity
{
    public Guid ExamTermId { get; set; }
    public ExamTerm ExamTerm { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public Guid ClassSectionId { get; set; }
    public ClassSection ClassSection { get; set; } = null!;

    public DateTime ExamDate { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal PassingMarks { get; set; }

    // Navigation
    public ICollection<StudentExamResult> StudentExamResults { get; set; } = new List<StudentExamResult>();
}
