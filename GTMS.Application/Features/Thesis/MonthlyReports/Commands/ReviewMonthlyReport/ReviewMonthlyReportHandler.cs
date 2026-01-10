using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Thesis; // For MonthlyReport
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.MonthlyReports.Commands.ReviewMonthlyReport;

public class ReviewMonthlyReportHandler : IRequestHandler<ReviewMonthlyReportCommand>
{
    private readonly IGtmsDbContext _context;

    public ReviewMonthlyReportHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReviewMonthlyReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _context.MonthlyReports
            .FirstOrDefaultAsync(r => r.Id == request.ReportId, cancellationToken);
            
        if (report == null)
        {
            throw new NotFoundException($"Monthly Report with ID {request.ReportId} not found.");
        }
        
        // Update report
        report.AdvisorComment = request.Comment;
        report.Status = request.IsApproved ? "Approved" : "Needs Revision";
        report.ReviewedAt = DateTime.UtcNow;
        report.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
