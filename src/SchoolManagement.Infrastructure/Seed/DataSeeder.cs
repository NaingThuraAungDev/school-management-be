using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Infrastructure.Identity;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Apply pending migrations
        await context.Database.MigrateAsync();

        // Seed Roles
        string[] roles = ["SuperAdmin", "Admin", "Teacher", "Student", "Parent"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed SuperAdmin user
        const string adminEmail = "admin@school.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                UserType = UserType.Staff
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@1234");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
            }
        }

        // Seed default Academic Year
        if (!await context.AcademicYears.AnyAsync())
        {
            var currentYear = DateTime.UtcNow.Year;
            context.AcademicYears.Add(new AcademicYear
            {
                Id = Guid.NewGuid(),
                Year = $"{currentYear}-{currentYear + 1}",
                StartDate = new DateTime(currentYear, 4, 1),
                EndDate = new DateTime(currentYear + 1, 3, 31),
                IsCurrent = true,
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        // Seed default Time Slots
        if (!await context.TimeSlots.AnyAsync())
        {
            var slots = new[]
            {
                new TimeSlot { Id = Guid.NewGuid(), Label = "Period 1", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(8, 45, 0), SortOrder = 1, IsBreak = false, CreatedAt = DateTime.UtcNow },
                new TimeSlot { Id = Guid.NewGuid(), Label = "Period 2", StartTime = new TimeSpan(8, 45, 0), EndTime = new TimeSpan(9, 30, 0), SortOrder = 2, IsBreak = false, CreatedAt = DateTime.UtcNow },
                new TimeSlot { Id = Guid.NewGuid(), Label = "Period 3", StartTime = new TimeSpan(9, 30, 0), EndTime = new TimeSpan(10, 15, 0), SortOrder = 3, IsBreak = false, CreatedAt = DateTime.UtcNow },
                new TimeSlot { Id = Guid.NewGuid(), Label = "Break", StartTime = new TimeSpan(10, 15, 0), EndTime = new TimeSpan(10, 45, 0), SortOrder = 4, IsBreak = true, CreatedAt = DateTime.UtcNow },
                new TimeSlot { Id = Guid.NewGuid(), Label = "Period 4", StartTime = new TimeSpan(10, 45, 0), EndTime = new TimeSpan(11, 30, 0), SortOrder = 5, IsBreak = false, CreatedAt = DateTime.UtcNow },
                new TimeSlot { Id = Guid.NewGuid(), Label = "Period 5", StartTime = new TimeSpan(11, 30, 0), EndTime = new TimeSpan(12, 15, 0), SortOrder = 6, IsBreak = false, CreatedAt = DateTime.UtcNow },
                new TimeSlot { Id = Guid.NewGuid(), Label = "Lunch", StartTime = new TimeSpan(12, 15, 0), EndTime = new TimeSpan(13, 0, 0), SortOrder = 7, IsBreak = true, CreatedAt = DateTime.UtcNow },
                new TimeSlot { Id = Guid.NewGuid(), Label = "Period 6", StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(13, 45, 0), SortOrder = 8, IsBreak = false, CreatedAt = DateTime.UtcNow },
                new TimeSlot { Id = Guid.NewGuid(), Label = "Period 7", StartTime = new TimeSpan(13, 45, 0), EndTime = new TimeSpan(14, 30, 0), SortOrder = 9, IsBreak = false, CreatedAt = DateTime.UtcNow },
            };
            context.TimeSlots.AddRange(slots);
            await context.SaveChangesAsync();
        }

        // Seed default Grade Definitions
        if (!await context.GradeDefinitions.AnyAsync())
        {
            var academicYear = await context.AcademicYears.FirstAsync(a => a.IsCurrent);
            var grades = new[]
            {
                new GradeDefinition { Id = Guid.NewGuid(), Label = "A+", MinPercentage = 90, MaxPercentage = 100, GradePoint = 10, Description = "Outstanding", AcademicYearId = academicYear.Id, CreatedAt = DateTime.UtcNow },
                new GradeDefinition { Id = Guid.NewGuid(), Label = "A", MinPercentage = 80, MaxPercentage = 89.99m, GradePoint = 9, Description = "Excellent", AcademicYearId = academicYear.Id, CreatedAt = DateTime.UtcNow },
                new GradeDefinition { Id = Guid.NewGuid(), Label = "B+", MinPercentage = 70, MaxPercentage = 79.99m, GradePoint = 8, Description = "Very Good", AcademicYearId = academicYear.Id, CreatedAt = DateTime.UtcNow },
                new GradeDefinition { Id = Guid.NewGuid(), Label = "B", MinPercentage = 60, MaxPercentage = 69.99m, GradePoint = 7, Description = "Good", AcademicYearId = academicYear.Id, CreatedAt = DateTime.UtcNow },
                new GradeDefinition { Id = Guid.NewGuid(), Label = "C+", MinPercentage = 50, MaxPercentage = 59.99m, GradePoint = 6, Description = "Above Average", AcademicYearId = academicYear.Id, CreatedAt = DateTime.UtcNow },
                new GradeDefinition { Id = Guid.NewGuid(), Label = "C", MinPercentage = 40, MaxPercentage = 49.99m, GradePoint = 5, Description = "Average", AcademicYearId = academicYear.Id, CreatedAt = DateTime.UtcNow },
                new GradeDefinition { Id = Guid.NewGuid(), Label = "D", MinPercentage = 33, MaxPercentage = 39.99m, GradePoint = 4, Description = "Below Average", AcademicYearId = academicYear.Id, CreatedAt = DateTime.UtcNow },
                new GradeDefinition { Id = Guid.NewGuid(), Label = "F", MinPercentage = 0, MaxPercentage = 32.99m, GradePoint = 0, Description = "Fail", AcademicYearId = academicYear.Id, CreatedAt = DateTime.UtcNow },
            };
            context.GradeDefinitions.AddRange(grades);
            await context.SaveChangesAsync();
        }
    }
}
