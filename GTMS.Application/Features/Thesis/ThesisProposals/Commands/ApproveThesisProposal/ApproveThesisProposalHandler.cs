using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Commands.ApproveThesisProposal;

public class ApproveThesisProposalHandler : IRequestHandler<ApproveThesisProposalCommand, Unit>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ApproveThesisProposalHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ApproveThesisProposalCommand request, CancellationToken cancellationToken)
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

        if (thesis.MainAdvisorId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the Main Advisor can approve or reject this thesis proposal.");
        }

        string targetStatusName = request.IsApproved ? "Approved" : "Rejected";
        var targetStatus = await _context.ThesisStatuses
            .FirstOrDefaultAsync(s => s.Name == targetStatusName, cancellationToken);
        
        if (targetStatus == null)
        {
            throw new NotFoundException("ThesisStatus", targetStatusName);
        }

        thesis.ThesisStatusId = targetStatus.Id;
        
        if (request.IsApproved)
        {
            thesis.ApprovedAt = DateTime.UtcNow;
        }
        
        if (!string.IsNullOrEmpty(request.Feedback))
        {
            // TODO: Feedback storage logic. 
            // Currently 'Comment' requires a SubmissionId, which we don't have at this stage (Thesis Proposal).
            // We need to either add 'Feedback' to ThesisProject or create a 'ThesisComment' entity.
            // Skipping persistence of feedback for this vertical slice to verify Status flow.
            
            /*
            var comment = new GTMS.Domain.Entities.Submission.Comment
            {
                CommentText = request.Feedback,
                UserId = currentUserId.Value,
                CreatedAt = DateTime.UtcNow
            };
            */
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
