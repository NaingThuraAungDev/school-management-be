using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class PromotionRecord : BaseEntity
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid FromClassSectionId { get; set; }
    public ClassSection FromClassSection { get; set; } = null!;

    public Guid ToClassSectionId { get; set; }
    public ClassSection ToClassSection { get; set; } = null!;

    public Guid AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;

    public DateTime PromotedAt { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
}
