using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Notification;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid NotificationTypeId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public virtual NotificationType NotificationType { get; set; } = null!;
}