using MediatR;

namespace GTMS.Application.Features.Thesis.MonthlyReports.Commands.CreateMonthlyReport;

public class CreateMonthlyReportCommand : IRequest<Guid>
{
    public Guid ThesisId { get; set; }
    public string Content { get; set; } = string.Empty;
}
