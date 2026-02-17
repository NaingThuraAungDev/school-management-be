using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Section : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., "A", "B", "C"
    public int SortOrder { get; set; }

    // Navigation
    public ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
}
