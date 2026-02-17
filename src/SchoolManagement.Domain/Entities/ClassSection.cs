using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class ClassSection : BaseEntity
{
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public Guid SectionId { get; set; }
    public Section Section { get; set; } = null!;

    public int Capacity { get; set; } = 40;

    // Navigation
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<SubjectTeacherMapping> SubjectTeacherMappings { get; set; } = new List<SubjectTeacherMapping>();
    public ICollection<TimetableEntry> TimetableEntries { get; set; } = new List<TimetableEntry>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
