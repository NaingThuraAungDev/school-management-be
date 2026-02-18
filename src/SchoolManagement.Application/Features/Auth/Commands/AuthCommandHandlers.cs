using MediatR;
using Microsoft.Extensions.Configuration;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Auth;

namespace SchoolManagement.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Authenticate user
        var loginResult = await _identityService.LoginAsync(request.Email, request.Password);
        if (!loginResult.Succeeded)
            return Result<LoginResponseDto>.Failure(loginResult.Message);

        // Generate tokens
        var token = await _jwtTokenService.GenerateTokenAsync(
            loginResult.UserId!, 
            loginResult.Email!, 
            loginResult.Roles!, 
            loginResult.UserType);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token to database
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Refresh token valid for 7 days
        await _identityService.UpdateRefreshTokenAsync(loginResult.UserId!, refreshToken, refreshTokenExpiry);

        // Get JWT expiry time from configuration
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Email = loginResult.Email!,
            UserType = loginResult.UserType!,
            Roles = loginResult.Roles!.ToList()
        };

        return Result<LoginResponseDto>.Success(response, "Login successful.");
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return Result.Failure("User not authenticated.");

        var result = await _identityService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        return result.Succeeded ? Result.Success(result.Message) : Result.Failure(result.Message);
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ResetPasswordAsync(request.UserId, request.NewPassword);
        return result.Succeeded ? Result.Success(result.Message) : Result.Failure(result.Message);
    }
}
