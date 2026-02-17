using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Class : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., "Grade 5", "Grade 6"
    public int SortOrder { get; set; }
    public string? Description { get; set; }

    // Navigation
    public ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
}
