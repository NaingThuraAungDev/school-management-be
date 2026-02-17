using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class TimeSlot : BaseEntity
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Label { get; set; } = string.Empty; // e.g., "Period 1", "Lunch Break"
    public int SortOrder { get; set; }
    public bool IsBreak { get; set; } = false;

    // Navigation
    public ICollection<TimetableEntry> TimetableEntries { get; set; } = new List<TimetableEntry>();
}
