namespace GTMS.Application.Features.Defense.Events.Queries.GetDefenseEvents;

public class DefenseEventDto
{
    public Guid Id { get; set; }
    public string TermName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int SlotDurationMinutes { get; set; }
    public int TotalSessions { get; set; }
}
