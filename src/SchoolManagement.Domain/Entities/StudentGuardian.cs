using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class StudentGuardian : BaseEntity
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid GuardianId { get; set; }
    public Guardian Guardian { get; set; } = null!;

    public bool IsPrimaryContact { get; set; } = false;
}
