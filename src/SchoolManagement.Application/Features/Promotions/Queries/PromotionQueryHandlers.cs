using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Promotions;

namespace SchoolManagement.Application.Features.Promotions.Queries;

public class GetPromotionPreviewQueryHandler : IRequestHandler<GetPromotionPreviewQuery, Result<List<PromotionPreviewDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPromotionPreviewQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<PromotionPreviewDto>>> Handle(GetPromotionPreviewQuery request, CancellationToken cancellationToken)
    {
        var students = await _context.Students
            .Include(s => s.ClassSection).ThenInclude(cs => cs!.Class)
            .Include(s => s.ClassSection).ThenInclude(cs => cs!.Section)
            .Where(s => s.ClassSectionId == request.FromClassSectionId && s.IsActive && !s.IsDeleted)
            .OrderBy(s => s.RollNumber)
            .Select(s => new PromotionPreviewDto
            {
                StudentId = s.Id,
                StudentName = $"{s.FirstName} {s.LastName}",
                RollNumber = s.RollNumber,
                CurrentClassSection = s.ClassSection != null
                    ? $"{s.ClassSection.Class.Name}-{s.ClassSection.Section.Name}"
                    : "Unassigned",
                IsEligible = true
            })
            .ToListAsync(cancellationToken);

        return Result<List<PromotionPreviewDto>>.Success(students);
    }
}
