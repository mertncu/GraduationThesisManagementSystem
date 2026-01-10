using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Academic;

public class Program : BaseEntity
{
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsActive { get; set; }
}