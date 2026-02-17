using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class Student : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string RollNumber { get; set; } = string.Empty;
    public string AdmissionId { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // FK to Identity User
    public string? UserId { get; set; }

    // FK to ClassSection
    public Guid? ClassSectionId { get; set; }
    public ClassSection? ClassSection { get; set; }

    // FK to AcademicYear
    public Guid? AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    // Navigation
    public ICollection<StudentGuardian> StudentGuardians { get; set; } = new List<StudentGuardian>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<StudentExamResult> ExamResults { get; set; } = new List<StudentExamResult>();
    public ICollection<PromotionRecord> PromotionRecords { get; set; } = new List<PromotionRecord>();
}
