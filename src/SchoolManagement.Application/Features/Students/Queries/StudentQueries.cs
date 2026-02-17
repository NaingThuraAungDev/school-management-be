using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Students;

namespace SchoolManagement.Application.Features.Students.Queries;

public record GetStudentByIdQuery(Guid Id) : IRequest<Result<StudentDto>>;

public record GetStudentsListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    Guid? ClassSectionId = null,
    bool? IsActive = true
) : IRequest<Result<PaginatedList<StudentListDto>>>;
