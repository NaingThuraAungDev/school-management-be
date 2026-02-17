using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Staff;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Staff.Commands;

public class OnboardStaffCommandHandler : IRequestHandler<OnboardStaffCommand, Result<StaffDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public OnboardStaffCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Result<StaffDto>> Handle(OnboardStaffCommand request, CancellationToken cancellationToken)
    {
        // Check email uniqueness
        var exists = await _context.StaffMembers
            .AnyAsync(s => s.Email == request.Email && !s.IsDeleted, cancellationToken);
        if (exists)
            return Result<StaffDto>.Failure("A staff member with this email already exists.");

        // Determine role based on staff type
        var identityRole = request.StaffType switch
        {
            Domain.Enums.StaffType.Admin => "Admin",
            Domain.Enums.StaffType.Teacher => "Teacher",
            _ => "Staff"
        };

        // Create Identity user
        var (userId, message) = await _identityService.CreateUserAsync(request.Email, request.Password, identityRole);
        if (string.IsNullOrEmpty(userId))
            return Result<StaffDto>.Failure($"Failed to create user account: {message}");

        var staff = new Domain.Entities.Staff
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Qualification = request.Qualification,
            JoiningDate = request.JoiningDate,
            StaffType = request.StaffType,
            UserId = userId
        };

        _context.StaffMembers.Add(staff);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<StaffDto>.Success(new StaffDto
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            Phone = staff.Phone,
            Qualification = staff.Qualification,
            JoiningDate = staff.JoiningDate,
            StaffType = staff.StaffType,
            IsActive = staff.IsActive
        }, "Staff onboarded successfully.");
    }
}

public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand, Result<StaffDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateStaffCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StaffDto>> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await _context.StaffMembers
            .Include(s => s.StaffRoles)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (staff == null)
            return Result<StaffDto>.Failure("Staff member not found.");

        var emailExists = await _context.StaffMembers
            .AnyAsync(s => s.Email == request.Email && s.Id != request.Id && !s.IsDeleted, cancellationToken);
        if (emailExists)
            return Result<StaffDto>.Failure("Another staff member with this email already exists.");

        staff.FirstName = request.FirstName;
        staff.LastName = request.LastName;
        staff.Email = request.Email;
        staff.Phone = request.Phone;
        staff.Qualification = request.Qualification;
        staff.StaffType = request.StaffType;
        staff.IsActive = request.IsActive;
        staff.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<StaffDto>.Success(new StaffDto
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            Phone = staff.Phone,
            Qualification = staff.Qualification,
            JoiningDate = staff.JoiningDate,
            StaffType = staff.StaffType,
            IsActive = staff.IsActive,
            Roles = staff.StaffRoles.Select(r => new StaffRoleDto
            {
                Id = r.Id,
                Role = r.Role,
                ClassSectionId = r.ClassSectionId
            }).ToList()
        }, "Staff updated successfully.");
    }
}

public class AssignStaffRoleCommandHandler : IRequestHandler<AssignStaffRoleCommand, Result<StaffRoleDto>>
{
    private readonly IApplicationDbContext _context;

    public AssignStaffRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StaffRoleDto>> Handle(AssignStaffRoleCommand request, CancellationToken cancellationToken)
    {
        var staff = await _context.StaffMembers
            .FirstOrDefaultAsync(s => s.Id == request.StaffId && !s.IsDeleted, cancellationToken);

        if (staff == null)
            return Result<StaffRoleDto>.Failure("Staff member not found.");

        // Check if role already assigned
        var roleExists = await _context.StaffRoles
            .AnyAsync(r => r.StaffId == request.StaffId && r.Role == request.Role && r.ClassSectionId == request.ClassSectionId, cancellationToken);
        if (roleExists)
            return Result<StaffRoleDto>.Failure("This role is already assigned to the staff member.");

        var staffRole = new StaffRole
        {
            StaffId = request.StaffId,
            Role = request.Role,
            ClassSectionId = request.ClassSectionId
        };

        _context.StaffRoles.Add(staffRole);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<StaffRoleDto>.Success(new StaffRoleDto
        {
            Id = staffRole.Id,
            Role = staffRole.Role,
            ClassSectionId = staffRole.ClassSectionId
        }, "Role assigned successfully.");
    }
}

public class DeleteStaffCommandHandler : IRequestHandler<DeleteStaffCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteStaffCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await _context.StaffMembers
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (staff == null)
            return Result.Failure("Staff member not found.");

        staff.IsDeleted = true;
        staff.IsActive = false;
        staff.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Staff member deleted successfully.");
    }
}
