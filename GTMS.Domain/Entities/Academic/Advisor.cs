using GTMS.Domain.Common;
using GTMS.Domain.Entities.Identity;

namespace GTMS.Domain.Entities.Academic;

public class Advisor : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }

    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
}