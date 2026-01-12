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
    
    public DefenseResultDto? DefenseResult { get; set; }
}

public class DefenseResultDto
{
    public double? QualityScore { get; set; }
    public double? PresentationScore { get; set; }
    public double? QAScore { get; set; }
    public double? TotalScore { get; set; }
    public string Result { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
