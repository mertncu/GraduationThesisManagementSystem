using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Academic;

public class Student : BaseEntity
{
    public Guid UserId { get; set; }
    public GTMS.Domain.Entities.Identity.User User { get; set; } = null!;

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public Guid ProgramId { get; set; }
    public Program Program { get; set; } = null!;

    public string StudentNumber { get; set; } = null!;
    public int EnrollmentYear { get; set; }
    public bool IsActive { get; set; }
}