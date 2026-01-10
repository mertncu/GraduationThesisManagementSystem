using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Submission;

public class AttachmentType : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}