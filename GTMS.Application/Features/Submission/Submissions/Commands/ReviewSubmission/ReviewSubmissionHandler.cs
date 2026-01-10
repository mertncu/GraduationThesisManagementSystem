using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Submission;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Submission.Submissions.Commands.ReviewSubmission;

public class ReviewSubmissionHandler : IRequestHandler<ReviewSubmissionCommand, Unit>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReviewSubmissionHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ReviewSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions
            .Include(s => s.SubmissionStatus)
            .FirstOrDefaultAsync(s => s.Id == request.SubmissionId, cancellationToken);

        if (submission == null)
        {
            throw new NotFoundException(nameof(Submission), request.SubmissionId);
        }

        // Verify Advisor permissions (Logic: Current user must be the advisor of the thesis)
        // Simplified check: Ensure user is an Advisor. 
        // Better check: Is he the Advisor of THIS thesis?
        // Let's load Thesis to check advisor
        var thesis = await _context.ThesisProjects.FindAsync(new object[] { submission.ThesisId }, cancellationToken);
        if (thesis == null || thesis.MainAdvisorId != _currentUserService.UserId)
        {
             // For now, if we can't verify ownership easily or if admin overrides, we might skip.
             // But strict rule: Only Main Advisor.
             if(_currentUserService.UserId != null && thesis?.MainAdvisorId != _currentUserService.UserId)
             {
                 throw new UnauthorizedAccessException("Only the Main Advisor can review submissions.");
             }
        }

        // Find status
        var statusEntity = await _context.SubmissionStatuses.FirstOrDefaultAsync(s => s.Name == request.Status, cancellationToken);
        if (statusEntity == null)
        {
            throw new NotFoundException("SubmissionStatus", request.Status);
        }

        submission.SubmissionStatus = statusEntity;
        submission.SubmissionStatusId = statusEntity.Id;
        submission.Feedback = request.Feedback;
        submission.ReviewedAt = DateTime.UtcNow;
        submission.ReviewedByUserId = _currentUserService.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
