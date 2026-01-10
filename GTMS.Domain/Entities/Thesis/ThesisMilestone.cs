using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Thesis;

public class ThesisMilestone : BaseEntity
{
    public Guid ThesisId { get; set; }
    public ThesisProject Thesis { get; set; } = null!;

    public Guid MilestoneTypeId { get; set; }
    public MilestoneType MilestoneType { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int OrderNo { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsLocked { get; set; }
    
    public ICollection<GTMS.Domain.Entities.Submission.Submission> Submissions { get; set; } = new List<GTMS.Domain.Entities.Submission.Submission>();
}