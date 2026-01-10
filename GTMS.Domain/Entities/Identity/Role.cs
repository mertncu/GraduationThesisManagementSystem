using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Identity;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}