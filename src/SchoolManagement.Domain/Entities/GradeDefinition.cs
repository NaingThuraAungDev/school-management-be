using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class GradeDefinition : BaseEntity
{
    public string Label { get; set; } = string.Empty; // e.g., "A+", "A", "B+", "B", "C", "F"
    public decimal MinPercentage { get; set; }
    public decimal MaxPercentage { get; set; }
    public int GradePoint { get; set; } // e.g., 4, 3, 2, 1, 0
    public string? Description { get; set; } // e.g., "Excellent", "Good", "Average"

    public Guid AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;
}
