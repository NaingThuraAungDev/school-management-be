namespace SchoolManagement.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, string? UserId, string? Email, string? UserType, IList<string>? Roles, string Message)> LoginAsync(string email, string password);
    Task<(string UserId, string Message)> CreateUserAsync(string email, string password, string role);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AddToRoleAsync(string userId, string role);
    Task<(bool Succeeded, string Message)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<(bool Succeeded, string Message)> ResetPasswordAsync(string userId, string newPassword);
    Task<(bool Succeeded, string RefreshToken)> UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime expiryTime);
}
