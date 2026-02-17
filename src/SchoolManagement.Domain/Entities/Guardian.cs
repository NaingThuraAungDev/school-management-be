using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class Guardian : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? Email { get; set; }
    public GuardianRelationship Relationship { get; set; }
    public string? Address { get; set; }
    public string? Occupation { get; set; }

    // Navigation
    public ICollection<StudentGuardian> StudentGuardians { get; set; } = new List<StudentGuardian>();
}
