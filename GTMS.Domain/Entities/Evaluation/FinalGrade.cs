using GTMS.Domain.Common;
using GTMS.Domain.Entities.Thesis;
using GTMS.Domain.Entities.Identity;
namespace GTMS.Domain.Entities.Evaluation;

public class FinalGrade : BaseEntity
{
    public Guid ThesisId { get; set; }
    public ThesisProject Thesis { get; set; } = null!;

    public Guid RubricId { get; set; }
    public EvaluationRubric Rubric { get; set; } = null!;

    public decimal NumericGrade { get; set; }
    public string LetterGrade { get; set; } = null!;
    public Guid ApprovedByUserId { get; set; }
    public User ApprovedByUser { get; set; } = null!;
    public DateTime ApprovedAt { get; set; }
}