using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.DTOs.Staff;

public class StaffDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Qualification { get; set; }
    public DateTime JoiningDate { get; set; }
    public StaffType StaffType { get; set; }
    public bool IsActive { get; set; }
    public List<StaffRoleDto> Roles { get; set; } = new();
}

public class StaffRoleDto
{
    public Guid Id { get; set; }
    public StaffRoleType Role { get; set; }
    public Guid? ClassSectionId { get; set; }
    public string? ClassSectionName { get; set; }
}

public class StaffListDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public StaffType StaffType { get; set; }
    public bool IsActive { get; set; }
}
