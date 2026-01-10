using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Thesis;

public class ThesisStatus : BaseEntity
{
    public string Name { get; set; } = null!;
    public bool IsFinal { get; set; }
}