using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.DTOs.Students;

public class StudentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string RollNumber { get; set; } = string.Empty;
    public string AdmissionId { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }
    public bool IsActive { get; set; }
    public Guid? ClassSectionId { get; set; }
    public string? ClassSectionName { get; set; }
    public List<GuardianDto> Guardians { get; set; } = new();
    public List<DocumentDto> Documents { get; set; } = new();
}

public class GuardianDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? Email { get; set; }
    public GuardianRelationship Relationship { get; set; }
    public string? Address { get; set; }
    public string? Occupation { get; set; }
    public bool IsPrimaryContact { get; set; }
}

public class DocumentDto
{
    public Guid Id { get; set; }
    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class StudentListDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public string AdmissionId { get; set; } = string.Empty;
    public string? ClassSectionName { get; set; }
    public bool IsActive { get; set; }
}
