using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class TimetableEntry : BaseEntity
{
    public Guid ClassSectionId { get; set; }
    public ClassSection ClassSection { get; set; } = null!;

    public Guid SubjectTeacherMappingId { get; set; }
    public SubjectTeacherMapping SubjectTeacherMapping { get; set; } = null!;

    public Guid TimeSlotId { get; set; }
    public TimeSlot TimeSlot { get; set; } = null!;

    public DayOfWeekEnum DayOfWeek { get; set; }

    public Guid AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;
}
