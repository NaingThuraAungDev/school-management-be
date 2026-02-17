using MediatR;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Promotions;

namespace SchoolManagement.Application.Features.Promotions.Queries;

public record GetPromotionPreviewQuery(Guid FromClassSectionId) : IRequest<Result<List<PromotionPreviewDto>>>;
