using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Thesis;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Dashboard.Student.Queries.GetStudentDashboard;

public class GetStudentDashboardQuery : IRequest<StudentDashboardVm?>
{
}

public class GetStudentDashboardQueryHandler : IRequestHandler<GetStudentDashboardQuery, StudentDashboardVm?>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetStudentDashboardQueryHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<StudentDashboardVm?> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return null; // Or throw generic error, logic in controller was just returning View()
        }

        var thesis = await _context.ThesisProjects
            .Include(t => t.Term) // Added Include
            .Include(t => t.ThesisMilestones)
                .ThenInclude(m => m.Submissions)
                    .ThenInclude(s => s.SubmissionStatus)
            .Include(t => t.MainAdvisor)
            .FirstOrDefaultAsync(t => t.StudentId == userId, cancellationToken);

        if (thesis == null)
        {
            return null;
        }

        var nextDeadlineMilestone = thesis.ThesisMilestones
            .Where(m => !m.Submissions.Any(s => s.SubmissionStatus != null && s.SubmissionStatus.Name == "Approved") && m.DueDate >= DateTime.UtcNow)
            .OrderBy(m => m.DueDate)
            .FirstOrDefault();

        var recentFeedbackSubmission = thesis.ThesisMilestones
            .SelectMany(m => m.Submissions)
            .Where(s => s.Feedback != null)
            .OrderByDescending(s => s.ReviewedAt)
            .FirstOrDefault();

        return new StudentDashboardVm
        {
            ThesisId = thesis.Id,
            ThesisTitle = thesis.Title,
            Term = thesis.Term.Name, // Added
            ThesisAbstract = thesis.Abstract,
            AdvisorName = $"{thesis.MainAdvisor.FirstName} {thesis.MainAdvisor.LastName}",
            RemainingMilestones = thesis.ThesisMilestones.Count(m => !m.Submissions.Any(s => s.SubmissionStatus != null && s.SubmissionStatus.Name == "Approved")),
            TotalMilestones = thesis.ThesisMilestones.Count,
            NextDeadline = nextDeadlineMilestone != null ? new DashboardDeadlineDto
            {
                MilestoneName = nextDeadlineMilestone.Name,
                DueDate = nextDeadlineMilestone.DueDate,
                DaysRemaining = (nextDeadlineMilestone.DueDate - DateTime.UtcNow).Days
            } : null,
            RecentFeedback = recentFeedbackSubmission != null ? new DashboardFeedbackDto
            {
                MilestoneName = recentFeedbackSubmission.Milestone.Name,
                FeedbackText = recentFeedbackSubmission.Feedback!,
                ReviewerName = $"{thesis.MainAdvisor.FirstName} {thesis.MainAdvisor.LastName}",
                ReviewedAt = recentFeedbackSubmission.ReviewedAt!.Value
            } : null,
            UpcomingMilestones = thesis.ThesisMilestones
                .Where(m => !m.Submissions.Any(s => s.SubmissionStatus != null && s.SubmissionStatus.Name == "Approved"))
                .OrderBy(m => m.DueDate)
                .Take(3)
                .Select(m => new DashboardMilestoneDto
                {
                    Name = m.Name,
                    DueDate = m.DueDate
                })
                .ToList(),
            RecentSubmissions = thesis.ThesisMilestones
                .SelectMany(m => m.Submissions)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(3)
                .Select(s => new DashboardSubmissionDto
                {
                    MilestoneName = s.Milestone.Name,
                    SubmittedAt = s.SubmittedAt,
                    Status = s.SubmissionStatus?.Name ?? "Pending"
                })
                .ToList(),
            CalendarEvents = thesis.ThesisMilestones
                .Select(m => new CalendarEventDto
                {
                    Title = m.Name,
                    Date = m.DueDate,
                    Type = "Milestone"
                })
                .ToList()
        };
    }
}
