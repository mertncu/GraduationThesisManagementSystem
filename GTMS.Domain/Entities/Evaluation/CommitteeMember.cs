using GTMS.Domain.Common;
using GTMS.Domain.Entities.Academic;
using GTMS.Domain.Entities.Evaluation;
namespace GTMS.Domain.Entities.Evaluation;

public class CommitteeMember : BaseEntity
{
    public Guid CommitteeId { get; set; }
    public Committee Committee { get; set; } = null!;

    public Guid AdvisorId { get; set; }
    public Advisor Advisor { get; set; } = null!;

    public Guid CommitteeRoleId { get; set; }
    public CommitteeRole CommitteeRole { get; set; } = null!;
}