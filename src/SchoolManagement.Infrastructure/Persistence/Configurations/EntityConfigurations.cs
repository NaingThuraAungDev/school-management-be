using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(20);
        builder.Property(e => e.Address).HasMaxLength(500);
        builder.Property(e => e.RollNumber).HasMaxLength(50);
        builder.Property(e => e.AdmissionId).HasMaxLength(50).IsRequired();
        builder.Property(e => e.UserId).HasMaxLength(450);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.AdmissionId).IsUnique();
        builder.HasIndex(e => e.UserId);

        builder.HasOne(e => e.ClassSection)
            .WithMany()
            .HasForeignKey(e => e.ClassSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AcademicYear)
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Documents)
            .WithOne(d => d.Student)
            .HasForeignKey(d => d.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ExamResults)
            .WithOne(r => r.Student)
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class GuardianConfiguration : IEntityTypeConfiguration<Guardian>
{
    public void Configure(EntityTypeBuilder<Guardian> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Mobile).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(256);
        builder.Property(e => e.Address).HasMaxLength(500);
        builder.Property(e => e.Occupation).HasMaxLength(200);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);
    }
}

public class StudentGuardianConfiguration : IEntityTypeConfiguration<StudentGuardian>
{
    public void Configure(EntityTypeBuilder<StudentGuardian> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Student)
            .WithMany(s => s.StudentGuardians)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Guardian)
            .WithMany(g => g.StudentGuardians)
            .HasForeignKey(e => e.GuardianId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.StudentId, e.GuardianId }).IsUnique();
    }
}

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FileName).HasMaxLength(256).IsRequired();
        builder.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(e => e.ContentType).HasMaxLength(100);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);
    }
}

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(20);
        builder.Property(e => e.Qualification).HasMaxLength(200);
        builder.Property(e => e.UserId).HasMaxLength(450);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.UserId);

        builder.HasMany(e => e.StaffRoles)
            .WithOne(r => r.Staff)
            .HasForeignKey(r => r.StaffId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.SubjectTeacherMappings)
            .WithOne(m => m.Staff)
            .HasForeignKey(m => m.StaffId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StaffRoleConfiguration : IEntityTypeConfiguration<StaffRole>
{
    public void Configure(EntityTypeBuilder<StaffRole> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);
    }
}

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasIndex(e => e.Name).IsUnique();

        builder.HasMany(e => e.ClassSections)
            .WithOne(cs => cs.Class)
            .HasForeignKey(cs => cs.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasIndex(e => e.Name).IsUnique();
    }
}

public class ClassSectionConfiguration : IEntityTypeConfiguration<ClassSection>
{
    public void Configure(EntityTypeBuilder<ClassSection> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasOne(e => e.Class)
            .WithMany(c => c.ClassSections)
            .HasForeignKey(e => e.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Section)
            .WithMany()
            .HasForeignKey(e => e.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.ClassId, e.SectionId }).IsUnique();
    }
}

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasIndex(e => e.Code).IsUnique();
    }
}

public class SubjectTeacherMappingConfiguration : IEntityTypeConfiguration<SubjectTeacherMapping>
{
    public void Configure(EntityTypeBuilder<SubjectTeacherMapping> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasOne(e => e.Subject)
            .WithMany()
            .HasForeignKey(e => e.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Staff)
            .WithMany(s => s.SubjectTeacherMappings)
            .HasForeignKey(e => e.StaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ClassSection)
            .WithMany()
            .HasForeignKey(e => e.ClassSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.SubjectId, e.StaffId, e.ClassSectionId }).IsUnique();
    }
}

public class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
{
    public void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Year).HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasIndex(e => e.Year).IsUnique();
    }
}

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Label).HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);
    }
}

public class TimetableEntryConfiguration : IEntityTypeConfiguration<TimetableEntry>
{
    public void Configure(EntityTypeBuilder<TimetableEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasOne(e => e.ClassSection)
            .WithMany()
            .HasForeignKey(e => e.ClassSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SubjectTeacherMapping)
            .WithMany()
            .HasForeignKey(e => e.SubjectTeacherMappingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TimeSlot)
            .WithMany()
            .HasForeignKey(e => e.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AcademicYear)
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique: one entry per class-section + day + timeslot + academic year
        builder.HasIndex(e => new { e.ClassSectionId, e.DayOfWeek, e.TimeSlotId, e.AcademicYearId }).IsUnique();
    }
}

public class ExamTermConfiguration : IEntityTypeConfiguration<ExamTerm>
{
    public void Configure(EntityTypeBuilder<ExamTerm> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasOne(e => e.AcademicYear)
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class GradeDefinitionConfiguration : IEntityTypeConfiguration<GradeDefinition>
{
    public void Configure(EntityTypeBuilder<GradeDefinition> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Label).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(200);
        builder.Property(e => e.MinPercentage).HasPrecision(5, 2);
        builder.Property(e => e.MaxPercentage).HasPrecision(5, 2);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasOne(e => e.AcademicYear)
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.MaxMarks).HasPrecision(5, 2);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasOne(e => e.ExamTerm)
            .WithMany()
            .HasForeignKey(e => e.ExamTermId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Subject)
            .WithMany()
            .HasForeignKey(e => e.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ClassSection)
            .WithMany()
            .HasForeignKey(e => e.ClassSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.ExamTermId, e.SubjectId, e.ClassSectionId }).IsUnique();
    }
}

public class StudentExamResultConfiguration : IEntityTypeConfiguration<StudentExamResult>
{
    public void Configure(EntityTypeBuilder<StudentExamResult> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.MarksObtained).HasPrecision(5, 2);
        builder.Property(e => e.Percentage).HasPrecision(5, 2);
        builder.Property(e => e.Remarks).HasMaxLength(500);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasOne(e => e.Exam)
            .WithMany()
            .HasForeignKey(e => e.ExamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Student)
            .WithMany(s => s.ExamResults)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.GradeDefinition)
            .WithMany()
            .HasForeignKey(e => e.GradeDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.ExamId, e.StudentId }).IsUnique();
    }
}

public class ReportCardTemplateConfiguration : IEntityTypeConfiguration<ReportCardTemplate>
{
    public void Configure(EntityTypeBuilder<ReportCardTemplate> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.TemplateConfig).HasMaxLength(4000);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasOne(e => e.AcademicYear)
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PromotionRecordConfiguration : IEntityTypeConfiguration<PromotionRecord>
{
    public void Configure(EntityTypeBuilder<PromotionRecord> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Remarks).HasMaxLength(500);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasOne(e => e.Student)
            .WithMany(s => s.PromotionRecords)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.FromClassSection)
            .WithMany()
            .HasForeignKey(e => e.FromClassSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ToClassSection)
            .WithMany()
            .HasForeignKey(e => e.ToClassSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AcademicYear)
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
