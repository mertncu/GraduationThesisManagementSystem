using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Evaluation;

public class EvaluationRubric : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}