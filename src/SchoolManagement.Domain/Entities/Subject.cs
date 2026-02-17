using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., "Mathematics", "Science"
    public string Code { get; set; } = string.Empty; // e.g., "MATH", "SCI"
    public string? Description { get; set; }

    // Navigation
    public ICollection<SubjectTeacherMapping> SubjectTeacherMappings { get; set; } = new List<SubjectTeacherMapping>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
