using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class ReportCardTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string TemplateConfig { get; set; } = "{}"; // JSON config for template layout
    public bool IsActive { get; set; } = true;

    public Guid AcademicYearId { get; set; }
    public AcademicYear AcademicYear { get; set; } = null!;
}
