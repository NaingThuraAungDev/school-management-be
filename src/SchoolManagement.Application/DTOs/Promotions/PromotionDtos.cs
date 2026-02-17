namespace SchoolManagement.Application.DTOs.Promotions;

public class PromotionPreviewDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public string CurrentClassSection { get; set; } = string.Empty;
    public bool IsEligible { get; set; } = true;
    public string? Remarks { get; set; }
}

public class PromotionRecordDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string FromClassSection { get; set; } = string.Empty;
    public string ToClassSection { get; set; } = string.Empty;
    public DateTime PromotedAt { get; set; }
    public string? Remarks { get; set; }
}
