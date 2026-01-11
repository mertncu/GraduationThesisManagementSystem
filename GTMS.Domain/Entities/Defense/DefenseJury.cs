using GTMS.Domain.Common;
using GTMS.Domain.Entities.Academic;

namespace GTMS.Domain.Entities.Defense;

public class DefenseJury : BaseEntity
{
    public Guid DefenseSessionId { get; set; }
    public DefenseSession DefenseSession { get; set; } = null!;

    public Guid? AdvisorId { get; set; }
    public Advisor? Advisor { get; set; }

    public string? ExternalName { get; set; }
    public string? ExternalInstitution { get; set; }
    public string? ExternalEmail { get; set; }
    
    public bool IsChair { get; set; }
}
