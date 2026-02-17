using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class AcademicYear : BaseEntity
{
    public string Year { get; set; } = string.Empty; // e.g., "2025-2026"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;

    // Navigation
    public ICollection<SubjectTeacherMapping> SubjectTeacherMappings { get; set; } = new List<SubjectTeacherMapping>();
    public ICollection<TimetableEntry> TimetableEntries { get; set; } = new List<TimetableEntry>();
    public ICollection<ExamTerm> ExamTerms { get; set; } = new List<ExamTerm>();
    public ICollection<GradeDefinition> GradeDefinitions { get; set; } = new List<GradeDefinition>();
    public ICollection<ReportCardTemplate> ReportCardTemplates { get; set; } = new List<ReportCardTemplate>();
    public ICollection<PromotionRecord> PromotionRecords { get; set; } = new List<PromotionRecord>();
}
