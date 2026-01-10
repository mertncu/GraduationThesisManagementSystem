using GTMS.Domain.Common;
using GTMS.Domain.Entities.Thesis;
namespace GTMS.Domain.Entities.Evaluation;

public class Committee : BaseEntity
{
    public Guid ThesisId { get; set; }
    public ThesisProject Thesis { get; set; } = null!;
    public string Notes { get; set; } = null!;
}