namespace SchoolManagement.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? UserType { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
}
