using Microsoft.AspNetCore.Identity;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public UserType UserType { get; set; }
    public Guid? StudentId { get; set; }
    public Guid? StaffId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
