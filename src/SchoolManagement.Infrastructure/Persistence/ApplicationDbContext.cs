using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Identity;

namespace SchoolManagement.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<StudentGuardian> StudentGuardians => Set<StudentGuardian>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Staff> StaffMembers => Set<Staff>();
    public DbSet<StaffRole> StaffRoles => Set<StaffRole>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<ClassSection> ClassSections => Set<ClassSection>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<SubjectTeacherMapping> SubjectTeacherMappings => Set<SubjectTeacherMapping>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<TimetableEntry> TimetableEntries => Set<TimetableEntry>();
    public DbSet<ExamTerm> ExamTerms => Set<ExamTerm>();
    public DbSet<GradeDefinition> GradeDefinitions => Set<GradeDefinition>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<StudentExamResult> StudentExamResults => Set<StudentExamResult>();
    public DbSet<ReportCardTemplate> ReportCardTemplates => Set<ReportCardTemplate>();
    public DbSet<PromotionRecord> PromotionRecords => Set<PromotionRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter for soft delete
        builder.Entity<Student>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Guardian>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Staff>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Class>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Section>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ClassSection>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Subject>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<SubjectTeacherMapping>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AcademicYear>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<TimeSlot>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<TimetableEntry>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ExamTerm>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<GradeDefinition>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Exam>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StudentExamResult>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ReportCardTemplate>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PromotionRecord>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
