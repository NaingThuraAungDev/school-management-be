using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class ExamTerm : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., "Mid-Term Exam", "Final Exam"
    public ExamTermType TermType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Guid AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;

    // Navigation
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
