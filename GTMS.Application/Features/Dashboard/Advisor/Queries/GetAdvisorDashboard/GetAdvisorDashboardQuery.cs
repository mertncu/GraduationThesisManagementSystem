using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Defense;
using GTMS.Domain.Entities.Thesis;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Dashboard.Advisor.Queries.GetAdvisorDashboard;

public class GetAdvisorDashboardQuery : IRequest<AdvisorDashboardVm>
{
    public Guid AdvisorUserId { get; set; }
}

public class AdvisorDashboardVm
{
    public int ActiveThesesCount { get; set; }
    public int PendingProposalsCount { get; set; }
    public int UpcomingDefensesCount { get; set; }
    public int CompletedThesesCount { get; set; }
    
    public List<PendingProposalDto> PendingProposals { get; set; } = new();
    public List<UpcomingEventDto> UpcomingEvents { get; set; } = new();
}

public class PendingProposalDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}

public class UpcomingEventDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty; // "Defense", "Meeting"
}

public class GetAdvisorDashboardQueryHandler : IRequestHandler<GetAdvisorDashboardQuery, AdvisorDashboardVm>
{
    private readonly IGtmsDbContext _context;

    public GetAdvisorDashboardQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<AdvisorDashboardVm> Handle(GetAdvisorDashboardQuery request, CancellationToken cancellationToken)
    {
        var vm = new AdvisorDashboardVm();

        // 1. Pending Proposals
        var pendingProposals = await _context.ThesisApplications
            .Include(a => a.Student)
            .Include(a => a.ApplicationStatus)
            .Where(a => a.AdvisorId == request.AdvisorUserId && a.ApplicationStatus.Name == "Pending")
            .OrderBy(a => a.SubmittedAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        vm.PendingProposalsCount = await _context.ThesisApplications
             .CountAsync(a => a.AdvisorId == request.AdvisorUserId && a.ApplicationStatus.Name == "Pending", cancellationToken);

        vm.PendingProposals = pendingProposals.Select(p => new PendingProposalDto
        {
            Id = p.Id,
            StudentName = $"{p.Student.FirstName} {p.Student.LastName}",
            Title = p.ProposedTitle,
            SubmittedAt = p.SubmittedAt
        }).ToList();

        // 2. Active Theses
        vm.ActiveThesesCount = await _context.ThesisProjects
            .CountAsync(t => (t.MainAdvisorId == request.AdvisorUserId || t.CoAdvisorId == request.AdvisorUserId) 
                             && t.ThesisStatus.Name == "Ongoing", cancellationToken);
                             
        vm.CompletedThesesCount = await _context.ThesisProjects
            .CountAsync(t => (t.MainAdvisorId == request.AdvisorUserId || t.CoAdvisorId == request.AdvisorUserId) 
                             && t.ThesisStatus.Name == "Completed", cancellationToken);

        // 3. Upcoming Defenses (Where I am Jury or Advisor)
        var upcomingDefenses = await _context.DefenseSessions
            .Include(ds => ds.ThesisProject)
            .Include(ds => ds.DefenseEvent)
            .Where(ds => (ds.ThesisProject.MainAdvisorId == request.AdvisorUserId || 
                          ds.JuryMembers.Any(j => j.AdvisorId == request.AdvisorUserId))
                          && ds.DefenseEvent.Date >= DateTime.Today)
            .OrderBy(ds => ds.DefenseEvent.Date)
            .ThenBy(ds => ds.StartTime)
            .Take(5)
            .ToListAsync(cancellationToken);

        vm.UpcomingDefensesCount = upcomingDefenses.Count; // This is actually 'Next 5', query for total if needed.

        vm.UpcomingEvents = upcomingDefenses.Select(d => new UpcomingEventDto
        {
            Title = $"Defense: {d.ThesisProject.Title}",
            Date = d.DefenseEvent.Date.Date.Add(d.StartTime),
            Type = "Defense"
        }).ToList();

        return vm;
    }
}
