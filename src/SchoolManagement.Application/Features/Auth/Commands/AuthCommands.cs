using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Auth;

namespace SchoolManagement.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponseDto>>;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Result>;

public record ResetPasswordCommand(string UserId, string NewPassword) : IRequest<Result>;
