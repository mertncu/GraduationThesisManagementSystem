namespace GTMS.Application.Features.Thesis.ThesisMilestones.Dtos;

public class ThesisMilestoneDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsLocked { get; set; }
    public bool IsCompleted { get; set; } 
    public List<GTMS.Application.Features.Submission.Submissions.Dtos.SubmissionDto> Submissions { get; set; } = new();
}
