using GTMS.Application.Features.Thesis.MonthlyReports.Dtos;
using MediatR;

namespace GTMS.Application.Features.Thesis.MonthlyReports.Queries.GetMonthlyReportsByThesisId;

public class GetMonthlyReportsByThesisIdQuery : IRequest<List<MonthlyReportDto>>
{
    public Guid ThesisId { get; set; }

    public GetMonthlyReportsByThesisIdQuery(Guid thesisId)
    {
        ThesisId = thesisId;
    }
}
