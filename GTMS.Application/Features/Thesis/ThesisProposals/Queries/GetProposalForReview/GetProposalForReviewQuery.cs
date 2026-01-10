using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetProposalForReview;

public class ReviewProposalVm
{
    public Guid ProposalId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string TermName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    
    // For POST binding/feedback
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
}

public class GetProposalForReviewQuery : IRequest<ReviewProposalVm>
{
    public Guid ProposalId { get; set; }
    public GetProposalForReviewQuery(Guid proposalId)
    {
        ProposalId = proposalId;
    }
}

public class GetProposalForReviewQueryHandler : IRequestHandler<GetProposalForReviewQuery, ReviewProposalVm>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetProposalForReviewQueryHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ReviewProposalVm> Handle(GetProposalForReviewQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        
        var proposal = await _context.ThesisApplications
            .Include(t => t.Student)
            .Include(t => t.Term)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.ProposalId, cancellationToken);
            
        if (proposal == null) throw new NotFoundException("Proposal not found");
        
        // Security Check
        if (proposal.AdvisorId != currentUserId) 
        {
             // Check if user is maybe an Admin?
             // For now, strict check as per original controller
             throw new UnauthorizedAccessException("You are not authorized to review this proposal.");
        }

        return new ReviewProposalVm
        {
            ProposalId = proposal.Id,
            StudentName = $"{proposal.Student.FirstName} {proposal.Student.LastName}",
            Title = proposal.ProposedTitle,
            Abstract = proposal.ProposedAbstract,
            TermName = proposal.Term.Name,
            SubmittedAt = proposal.SubmittedAt
        };
    }
}
