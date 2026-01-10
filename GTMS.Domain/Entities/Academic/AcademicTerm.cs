using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Academic;

public class AcademicTerm : BaseEntity
{
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}