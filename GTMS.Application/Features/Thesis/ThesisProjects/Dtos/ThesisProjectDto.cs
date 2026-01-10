namespace GTMS.Application.Features.Thesis.ThesisProjects.Dtos;

public class ThesisProjectDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string AdvisorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
}
