using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.System;

public class ActivityLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string Action { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public Guid EntityId { get; set; }
    public DateTime Timestamp { get; set; }
    public string DetailJson { get; set; } = null!;
}