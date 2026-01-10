using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Thesis;
using MediatR;

namespace GTMS.Application.Features.Thesis.MonthlyReports.Commands.CreateMonthlyReport;

public class CreateMonthlyReportHandler : IRequestHandler<CreateMonthlyReportCommand, Guid>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateMonthlyReportHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateMonthlyReportCommand request, CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        
        // Improve: Check if Thesis belongs to Student
        // For now, assuming request.ThesisId is valid or checked by Controller/Query.
        // But better is to check database.

         var report = new MonthlyReport
        {
            Id = Guid.NewGuid(),
            ThesisId = request.ThesisId,
            StudentId = studentId,
            Content = request.Content,
            ReportDate = DateTime.UtcNow,
            Status = "Submitted",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.MonthlyReports.Add(report);
        await _context.SaveChangesAsync(cancellationToken);

        return report.Id;
    }
}
