using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Submission;

public class SubmissionStatus : BaseEntity
{
    public string Name { get; set; } = null!;
}