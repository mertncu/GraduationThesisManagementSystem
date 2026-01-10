using GTMS.Domain.Common;
using GTMS.Domain.Entities.Thesis;
namespace GTMS.Domain.Entities.Evaluation;

public class DefenseSession : BaseEntity
{
    public Guid ThesisId { get; set; }
    public ThesisProject Thesis { get; set; } = null!;

    public Guid CommitteeId { get; set; }
    public Committee Committee { get; set; } = null!;

    public Guid DefenseStatusId { get; set; }
    public DefenseStatus DefenseStatus { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string Room { get; set; } = null!;
    public bool IsOnline { get; set; }
    public string? OnlineMeetingLink { get; set; }
    public DateTime? ActualStartAt { get; set; }
    public DateTime? ActualEndAt { get; set; }
}