using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Thesis;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisMilestones.Commands.CreateThesisMilestone;

public class CreateThesisMilestoneHandler : IRequestHandler<CreateThesisMilestoneCommand, Guid>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public CreateThesisMilestoneHandler(IGtmsDbContext context, ICurrentUserService currentUserService, IEmailService emailService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<Guid> Handle(CreateThesisMilestoneCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var thesis = await _context.ThesisProjects
            .FirstOrDefaultAsync(t => t.Id == request.ThesisId, cancellationToken);

        if (thesis == null)
        {
            throw new NotFoundException("ThesisProject", request.ThesisId);
        }

        // Allow Main Advisor OR the Student owner to create milestones
        if (thesis.MainAdvisorId != currentUserId && thesis.StudentId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the Main Advisor or the Student can create milestones for this thesis.");
        }

        var milestoneType = await _context.MilestoneTypes.FirstOrDefaultAsync(cancellationToken);
        if (milestoneType == null)
        {
             throw new NotFoundException("MilestoneType", "Default");
        }

        var entity = new ThesisMilestone
        {
            ThesisId = request.ThesisId,
            Name = request.Name,
            Description = request.Description,
            DueDate = request.DueDate,
            MilestoneTypeId = milestoneType.Id,
            IsLocked = false,
            OrderNo = await _context.ThesisMilestones
                        .Where(m => m.ThesisId == request.ThesisId)
                        .Select(m => (int?)m.OrderNo)
                        .MaxAsync(cancellationToken) ?? 1
        };

        _context.ThesisMilestones.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        // Send Email if created by Advisor
        if (thesis.MainAdvisorId == currentUserId)
        {
            var studentUser = await _context.Users.FindAsync(thesis.StudentId);
            if (studentUser != null && !string.IsNullOrEmpty(studentUser.Email))
            {
                var subject = $"New Milestone Assigned: {request.Name}";
                var body = $@"
                    <div style='font-family: Arial, sans-serif;'>
                        <h2>New Milestone Assigned</h2>
                        <p>Dear {studentUser.FirstName},</p>
                        <p>A new milestone <strong>'{request.Name}'</strong> has been assigned to your thesis project.</p>
                        <p><strong>Description:</strong> {request.Description}</p>
                        <p><strong>Due Date:</strong> {request.DueDate.ToString("d")}</p>
                        <br>
                        <p>Please check the system for more details.</p>
                        <br>
                        <p>Best regards,<br>GTMS System</p>
                    </div>";

                await _emailService.SendEmailAsync(studentUser.Email, subject, body, cancellationToken);
            }
        }

        return entity.Id;
    }
}
