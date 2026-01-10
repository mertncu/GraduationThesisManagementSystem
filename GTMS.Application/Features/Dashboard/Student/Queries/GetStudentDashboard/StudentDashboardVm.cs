namespace GTMS.Application.Features.Dashboard.Student.Queries.GetStudentDashboard;

public class StudentDashboardVm
{
    public Guid ThesisId { get; set; }
    public string ThesisTitle { get; set; } = string.Empty;
    public string ThesisAbstract { get; set; } = string.Empty;
    public string AdvisorName { get; set; } = string.Empty;
    public int RemainingMilestones { get; set; }
    public int TotalMilestones { get; set; }
    public DashboardDeadlineDto? NextDeadline { get; set; }
    public DashboardFeedbackDto? RecentFeedback { get; set; }
    public List<DashboardMilestoneDto> UpcomingMilestones { get; set; } = new();
    public List<DashboardSubmissionDto> RecentSubmissions { get; set; } = new();
    public List<CalendarEventDto> CalendarEvents { get; set; } = new();
}

public class CalendarEventDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class DashboardDeadlineDto
{
    public string MilestoneName { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int DaysRemaining { get; set; }
}

public class DashboardFeedbackDto
{
    public string MilestoneName { get; set; } = string.Empty;
    public string FeedbackText { get; set; } = string.Empty;
    public string ReviewerName { get; set; } = string.Empty;
    public DateTime ReviewedAt { get; set; }
}

public class DashboardMilestoneDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
}

public class DashboardSubmissionDto
{
    public string MilestoneName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
