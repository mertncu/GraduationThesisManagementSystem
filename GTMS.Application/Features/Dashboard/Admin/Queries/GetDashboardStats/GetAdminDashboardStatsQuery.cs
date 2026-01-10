using MediatR;
using GTMS.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Dashboard.Admin.Queries.GetDashboardStats;

public class GetAdminDashboardStatsQuery : IRequest<AdminDashboardVm>
{
}

public class GetAdminDashboardStatsQueryHandler : IRequestHandler<GetAdminDashboardStatsQuery, AdminDashboardVm>
{
    private readonly IGtmsDbContext _context;

    public GetAdminDashboardStatsQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardVm> Handle(GetAdminDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        return new AdminDashboardVm
        {
            TotalUsers = await _context.Users.CountAsync(cancellationToken),
            TotalTheses = await _context.ThesisProjects.CountAsync(cancellationToken),
            ActiveTerms = await _context.AcademicTerms.CountAsync(t => t.StartDate <= today && t.EndDate >= today, cancellationToken),
            PendingApprovals = await _context.ThesisApplications.CountAsync(p => p.ApplicationStatus.Name == "Pending", cancellationToken)
        };
    }
}
