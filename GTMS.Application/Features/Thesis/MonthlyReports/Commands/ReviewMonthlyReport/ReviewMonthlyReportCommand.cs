using MediatR;

namespace GTMS.Application.Features.Thesis.MonthlyReports.Commands.ReviewMonthlyReport;

public class ReviewMonthlyReportCommand : IRequest
{
    public Guid ReportId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
}
