using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

/// <summary>
/// Maps a Subject + Teacher to a ClassSection.
/// e.g., "Mr. Smith teaches Math to Grade 5-A"
/// </summary>
public class SubjectTeacherMapping : BaseEntity
{
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public Guid StaffId { get; set; }
    public Staff Staff { get; set; } = null!;

    public Guid ClassSectionId { get; set; }
    public ClassSection ClassSection { get; set; } = null!;

    public Guid AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;
}
