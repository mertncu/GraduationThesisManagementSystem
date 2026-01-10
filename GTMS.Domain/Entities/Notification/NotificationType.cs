using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Notification;

public class NotificationType : BaseEntity
{
    public string Name { get; set; } = null!;
}