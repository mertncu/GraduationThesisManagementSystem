using GTMS.Domain.Common;
using GTMS.Domain.Entities.Identity;

namespace GTMS.Domain.Entities.Submission;

public class Comment : BaseEntity
{
    public Guid SubmissionId { get; set; }
    public Submission Submission { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string CommentText { get; set; } = null!;
}