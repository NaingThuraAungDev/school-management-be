using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.DTOs.Timetable;

public class TimeSlotDto
{
    public Guid Id { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Label { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsBreak { get; set; }
}

public class TimetableEntryDto
{
    public Guid Id { get; set; }
    public Guid ClassSectionId { get; set; }
    public string ClassSectionName { get; set; } = string.Empty;
    public Guid SubjectTeacherMappingId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public Guid TimeSlotId { get; set; }
    public string TimeSlotLabel { get; set; } = string.Empty;
    public DayOfWeekEnum DayOfWeek { get; set; }
}

public class TimetableClashResponseDto
{
    public bool HasClash { get; set; }
    public string? ClashDescription { get; set; }
}
