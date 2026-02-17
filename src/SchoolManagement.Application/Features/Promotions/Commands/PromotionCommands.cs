using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Promotions;

namespace SchoolManagement.Application.Features.Promotions.Commands;

public record BulkPromoteStudentsCommand(
    Guid FromClassSectionId,
    Guid ToClassSectionId,
    Guid AcademicYearId,
    List<Guid>? StudentIds = null // If null, promote all active students
) : IRequest<Result<List<PromotionRecordDto>>>;
