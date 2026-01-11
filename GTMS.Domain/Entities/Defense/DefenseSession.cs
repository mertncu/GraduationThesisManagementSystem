using GTMS.Domain.Common;
using GTMS.Domain.Entities.Thesis;

namespace GTMS.Domain.Entities.Defense;

public class DefenseSession : BaseEntity
{
    public Guid DefenseEventId { get; set; }
    public DefenseEvent DefenseEvent { get; set; } = null!;

    public Guid ThesisId { get; set; }
    public ThesisProject ThesisProject { get; set; } = null!;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public double? QualityScore { get; set; }
    public double? PresentationScore { get; set; }
    public double? QAScore { get; set; }
    public double? TotalScore { get; set; }
    public string? Result { get; set; }
    public string? Comment { get; set; }

    public ICollection<DefenseJury> JuryMembers { get; set; } = new List<DefenseJury>();
}
