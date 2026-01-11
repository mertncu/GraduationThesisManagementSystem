using GTMS.Domain.Common;
using GTMS.Domain.Entities.Academic;

namespace GTMS.Domain.Entities.Defense;

public class DefenseEvent : BaseEntity
{
    public Guid TermId { get; set; }
    public AcademicTerm Term { get; set; } = null!;

    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 60;
    public string Location { get; set; } = string.Empty;

    public ICollection<DefenseSession> Sessions { get; set; } = new List<DefenseSession>();
}
