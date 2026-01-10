using GTMS.Domain.Common;

namespace GTMS.Domain.Entities.Evaluation;

public class RubricCriteria : BaseEntity
{
    public Guid RubricId { get; set; }
    public EvaluationRubric Rubric { get; set; } = null!;
    public string CriteriaName { get; set; } = null!;
    public int MaxScore { get; set; }
    public int OrderNo { get; set; }
}