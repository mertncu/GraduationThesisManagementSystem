using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Thesis;

public class MilestoneType : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}