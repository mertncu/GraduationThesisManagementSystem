namespace GTMS.Application.Features.Dashboard.Admin.Queries.GetDashboardStats;

public class AdminDashboardVm
{
    public int TotalUsers { get; set; }
    public int TotalTheses { get; set; }
    public int ActiveTerms { get; set; }
    public int PendingApprovals { get; set; }
}
