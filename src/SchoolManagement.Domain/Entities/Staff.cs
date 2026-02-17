using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class Staff : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Qualification { get; set; }
    public DateTime JoiningDate { get; set; }
    public StaffType StaffType { get; set; }
    public bool IsActive { get; set; } = true;

    // FK to Identity User
    public string? UserId { get; set; }

    // Navigation
    public ICollection<StaffRole> StaffRoles { get; set; } = new List<StaffRole>();
    public ICollection<SubjectTeacherMapping> SubjectTeacherMappings { get; set; } = new List<SubjectTeacherMapping>();
}
