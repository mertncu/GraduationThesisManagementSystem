using GTMS.Domain.Common;
using GTMS.Domain.Entities.Academic;
using GTMS.Domain.Entities.Identity;

namespace GTMS.Domain.Entities.Thesis;

public class ThesisApplication : BaseEntity
{
    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;

    public Guid AdvisorId { get; set; }
    public User Advisor { get; set; } = null!;

    public Guid TermId { get; set; }
    public AcademicTerm Term { get; set; } = null!;

    public string ProposedTitle { get; set; } = null!;
    public string ProposedAbstract { get; set; } = null!;

    public Guid ApplicationStatusId { get; set; }
    public ApplicationStatus ApplicationStatus { get; set; } = null!;

    public string? AdvisorComment { get; set; }

    public DateTime SubmittedAt { get; set; }
    public DateTime? DecidedAt { get; set; }
    public Guid? DecidedByUserId { get; set; }
    public User? DecidedByUser { get; set; }
}