using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Staff;

namespace SchoolManagement.Application.Features.Staff.Queries;

public class GetStaffByIdQueryHandler : IRequestHandler<GetStaffByIdQuery, Result<StaffDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStaffByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StaffDto>> Handle(GetStaffByIdQuery request, CancellationToken cancellationToken)
    {
        var staff = await _context.StaffMembers
            .Include(s => s.StaffRoles).ThenInclude(r => r.ClassSection).ThenInclude(cs => cs!.Class)
            .Include(s => s.StaffRoles).ThenInclude(r => r.ClassSection).ThenInclude(cs => cs!.Section)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (staff == null)
            return Result<StaffDto>.Failure("Staff member not found.");

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
                ClassSectionId = r.ClassSectionId,
                ClassSectionName = r.ClassSection != null
                    ? $"{r.ClassSection.Class.Name}-{r.ClassSection.Section.Name}"
                    : null
            }).ToList()
        });
    }
}

public class GetStaffListQueryHandler : IRequestHandler<GetStaffListQuery, Result<PaginatedList<StaffListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetStaffListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<StaffListDto>>> Handle(GetStaffListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StaffMembers
            .Where(s => !s.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.FirstName.ToLower().Contains(term) ||
                s.LastName.ToLower().Contains(term) ||
                s.Email.ToLower().Contains(term));
        }

        var projectedQuery = query
            .OrderBy(s => s.FirstName)
            .Select(s => new StaffListDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                StaffType = s.StaffType,
                IsActive = s.IsActive
            });

        var result = await PaginatedList<StaffListDto>.CreateAsync(
            projectedQuery, request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<StaffListDto>>.Success(result);
    }
}
