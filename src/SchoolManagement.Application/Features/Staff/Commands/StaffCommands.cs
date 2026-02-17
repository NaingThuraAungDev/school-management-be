using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Staff;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Features.Staff.Commands;

public record OnboardStaffCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Phone,
    string? Qualification,
    DateTime JoiningDate,
    StaffType StaffType
) : IRequest<Result<StaffDto>>;

public record UpdateStaffCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Qualification,
    StaffType StaffType,
    bool IsActive
) : IRequest<Result<StaffDto>>;

public record AssignStaffRoleCommand(
    Guid StaffId,
    StaffRoleType Role,
    Guid? ClassSectionId
) : IRequest<Result<StaffRoleDto>>;

public record DeleteStaffCommand(Guid Id) : IRequest<Result>;
