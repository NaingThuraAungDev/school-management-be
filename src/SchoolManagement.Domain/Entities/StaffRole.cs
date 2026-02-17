using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class StaffRole : BaseEntity
{
    public Guid StaffId { get; set; }
    public Staff Staff { get; set; } = null!;

    public StaffRoleType Role { get; set; }

    // Optional: links to ClassSection if role is ClassTeacher/HOD
    public Guid? ClassSectionId { get; set; }
    public ClassSection? ClassSection { get; set; }
}
