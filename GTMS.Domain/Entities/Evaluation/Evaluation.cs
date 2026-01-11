using GTMS.Domain.Common;
using GTMS.Domain.Entities.Defense;
namespace GTMS.Domain.Entities.Evaluation;

public class Evaluation : BaseEntity
{
    public Guid DefenseSessionId { get; set; }
    public DefenseSession DefenseSession { get; set; } = null!;

    public Guid CommitteeMemberId { get; set; }
    public CommitteeMember CommitteeMember { get; set; } = null!;

    public Guid RubricCriteriaId { get; set; }
    public RubricCriteria RubricCriteria { get; set; } = null!;

    public decimal Score { get; set; }
    public string? Comment { get; set; }
}