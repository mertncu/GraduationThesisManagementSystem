namespace GTMS.Application.Features.Thesis.MonthlyReports.Dtos;

public class MonthlyReportDto
{
    public Guid Id { get; set; }
    public DateTime ReportDate { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AdvisorComment { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string StudentName { get; set; } = string.Empty;
}
