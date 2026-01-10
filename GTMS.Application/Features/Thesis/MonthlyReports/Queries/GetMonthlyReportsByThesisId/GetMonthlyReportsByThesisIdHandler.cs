using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Thesis.MonthlyReports.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.MonthlyReports.Queries.GetMonthlyReportsByThesisId;

public class GetMonthlyReportsByThesisIdHandler : IRequestHandler<GetMonthlyReportsByThesisIdQuery, List<MonthlyReportDto>>
{
    private readonly IGtmsDbContext _context;

    public GetMonthlyReportsByThesisIdHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<MonthlyReportDto>> Handle(GetMonthlyReportsByThesisIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.MonthlyReports
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Where(r => r.ThesisId == request.ThesisId)
            .OrderByDescending(r => r.ReportDate)
            .Select(r => new MonthlyReportDto
            {
                Id = r.Id,
                ReportDate = r.ReportDate,
                Content = r.Content,
                Status = r.Status,
                AdvisorComment = r.AdvisorComment,
                ReviewedAt = r.ReviewedAt,
                StudentName = $"{r.Student.User.FirstName} {r.Student.User.LastName}"
            })
            .ToListAsync(cancellationToken);
    }
}
