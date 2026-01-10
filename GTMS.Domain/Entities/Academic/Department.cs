using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Academic;

public class Department : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}