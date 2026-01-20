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
    private readonly IEmailService _emailService;

    public ReviewSubmissionHandler(IGtmsDbContext context, ICurrentUserService currentUserService, IEmailService emailService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _emailService = emailService;
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

        // Send Email Notification
        var studentUser = await _context.Users.FindAsync(thesis.StudentId);
        if (studentUser != null && !string.IsNullOrEmpty(studentUser.Email))
        {
            var subject = $"Submission Reviewed: {submission.SubmissionStatus.Name}";
            var color = request.Status == "Approved" ? "green" : "orange"; // Assuming Revision is the other main status
            var body = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h2>Submission Reviewed</h2>
                    <p>Dear {studentUser.FirstName},</p>
                    <p>Your submission has been reviewed by your advisor.</p>
                    <p><strong>Result:</strong> <span style='color: {color}; font-weight: bold;'>{request.Status}</span></p>
                    <p><strong>Feedback:</strong> {request.Feedback ?? "No feedback provided."}</p>
                    <br>
                    <p>Please log in to GTMS to view more details.</p>
                    <br>
                    <p>Best regards,<br>GTMS System</p>
                </div>";

            await _emailService.SendEmailAsync(studentUser.Email, subject, body, cancellationToken);
        }

        return Unit.Value;
    }
}
