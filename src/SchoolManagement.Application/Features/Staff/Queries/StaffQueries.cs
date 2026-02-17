using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Staff;

namespace SchoolManagement.Application.Features.Staff.Queries;

public record GetStaffByIdQuery(Guid Id) : IRequest<Result<StaffDto>>;

public record GetStaffListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    bool? IsActive = true
) : IRequest<Result<PaginatedList<StaffListDto>>>;
