using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Submission;

public class Attachment : BaseEntity
{
    public Guid ThesisId { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid AttachmentTypeId { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = null!;
    public Guid UploadedByUserId { get; set; }
    public DateTime UploadedAt { get; set; }
}