using SchoolManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Student> Students { get; }
    DbSet<Guardian> Guardians { get; }
    DbSet<StudentGuardian> StudentGuardians { get; }
    DbSet<Document> Documents { get; }
    DbSet<Staff> StaffMembers { get; }
    DbSet<StaffRole> StaffRoles { get; }
    DbSet<Class> Classes { get; }
    DbSet<Section> Sections { get; }
    DbSet<ClassSection> ClassSections { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<SubjectTeacherMapping> SubjectTeacherMappings { get; }
    DbSet<AcademicYear> AcademicYears { get; }
    DbSet<TimeSlot> TimeSlots { get; }
    DbSet<TimetableEntry> TimetableEntries { get; }
    DbSet<ExamTerm> ExamTerms { get; }
    DbSet<GradeDefinition> GradeDefinitions { get; }
    DbSet<Exam> Exams { get; }
    DbSet<StudentExamResult> StudentExamResults { get; }
    DbSet<ReportCardTemplate> ReportCardTemplates { get; }
    DbSet<PromotionRecord> PromotionRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
