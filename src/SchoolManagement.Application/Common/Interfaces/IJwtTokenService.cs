namespace SchoolManagement.Application.Common.Interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(string userId, string email, IList<string> roles, string? userType = null);
    string GenerateRefreshToken();
}
