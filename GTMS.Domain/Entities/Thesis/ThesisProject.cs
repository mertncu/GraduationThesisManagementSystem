using GTMS.Domain.Common;
using GTMS.Domain.Entities.Academic;
using GTMS.Domain.Entities.Identity;

namespace GTMS.Domain.Entities.Thesis;

public class ThesisProject : BaseEntity
{
    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;

    public Guid MainAdvisorId { get; set; }
    public User MainAdvisor { get; set; } = null!;

    public Guid? CoAdvisorId { get; set; }
    public User? CoAdvisor { get; set; }

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public Guid ProgramId { get; set; }
    public Program Program { get; set; } = null!;

    public Guid TermId { get; set; }
    public AcademicTerm Term { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Abstract { get; set; } = null!;

    public Guid ThesisStatusId { get; set; }
    public ThesisStatus ThesisStatus { get; set; } = null!;

    public DateTime? ApprovedAt { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public ICollection<ThesisMilestone> ThesisMilestones { get; set; } = new List<ThesisMilestone>();
}