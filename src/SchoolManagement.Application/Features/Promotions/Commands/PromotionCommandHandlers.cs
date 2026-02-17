using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Promotions;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Promotions.Commands;

public class BulkPromoteStudentsCommandHandler : IRequestHandler<BulkPromoteStudentsCommand, Result<List<PromotionRecordDto>>>
{
    private readonly IApplicationDbContext _context;

    public BulkPromoteStudentsCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<PromotionRecordDto>>> Handle(BulkPromoteStudentsCommand request, CancellationToken cancellationToken)
    {
        // Validate class sections
        var fromCs = await _context.ClassSections
            .Include(cs => cs.Class).Include(cs => cs.Section)
            .FirstOrDefaultAsync(cs => cs.Id == request.FromClassSectionId && !cs.IsDeleted, cancellationToken);
        if (fromCs == null) return Result<List<PromotionRecordDto>>.Failure("Source class-section not found.");

        var toCs = await _context.ClassSections
            .Include(cs => cs.Class).Include(cs => cs.Section)
            .FirstOrDefaultAsync(cs => cs.Id == request.ToClassSectionId && !cs.IsDeleted, cancellationToken);
        if (toCs == null) return Result<List<PromotionRecordDto>>.Failure("Target class-section not found.");

        // Get students to promote
        var studentsQuery = _context.Students
            .Where(s => s.ClassSectionId == request.FromClassSectionId && s.IsActive && !s.IsDeleted);

        if (request.StudentIds != null && request.StudentIds.Count != 0)
            studentsQuery = studentsQuery.Where(s => request.StudentIds.Contains(s.Id));

        var students = await studentsQuery.ToListAsync(cancellationToken);

        if (students.Count == 0)
            return Result<List<PromotionRecordDto>>.Failure("No students found to promote.");

        var promotionRecords = new List<PromotionRecordDto>();
        var rollCounter = await _context.Students
            .CountAsync(s => s.ClassSectionId == request.ToClassSectionId && !s.IsDeleted, cancellationToken);

        foreach (var student in students)
        {
            rollCounter++;
            // Create promotion record
            var record = new PromotionRecord
            {
                StudentId = student.Id,
                FromClassSectionId = request.FromClassSectionId,
                ToClassSectionId = request.ToClassSectionId,
                AcademicYearId = request.AcademicYearId
            };
            _context.PromotionRecords.Add(record);

            // Move student to new class
            student.ClassSectionId = request.ToClassSectionId;
            student.RollNumber = $"{rollCounter:D3}";
            student.UpdatedAt = DateTime.UtcNow;

            promotionRecords.Add(new PromotionRecordDto
            {
                Id = record.Id,
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                FromClassSection = $"{fromCs.Class.Name}-{fromCs.Section.Name}",
                ToClassSection = $"{toCs.Class.Name}-{toCs.Section.Name}",
                PromotedAt = record.PromotedAt
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<List<PromotionRecordDto>>.Success(promotionRecords,
            $"{promotionRecords.Count} students promoted successfully.");
    }
}
