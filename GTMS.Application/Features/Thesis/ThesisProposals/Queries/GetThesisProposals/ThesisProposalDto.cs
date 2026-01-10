namespace GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetThesisProposals;

public class ThesisProposalDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string AdvisorName { get; set; } = string.Empty;
    public string TermName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? DecidedAt { get; set; }
}
