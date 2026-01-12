using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetProposalDetails;

public class ProposalDetailsVm
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string AdvisorName { get; set; } = string.Empty;
    public string TermName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AdvisorComment { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? DecidedAt { get; set; }
    
    // Helper to determine if current user is the student (for UI logic if needed)
    public bool IsMyProposal { get; set; }
}

public class GetProposalDetailsQuery : IRequest<ProposalDetailsVm>
{
    public Guid ProposalId { get; set; }
}

public class GetProposalDetailsQueryHandler : IRequestHandler<GetProposalDetailsQuery, ProposalDetailsVm>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetProposalDetailsQueryHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ProposalDetailsVm> Handle(GetProposalDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null) throw new UnauthorizedAccessException();

        var proposal = await _context.ThesisApplications
            .Include(t => t.Student)
            .Include(t => t.Advisor)
            .Include(t => t.Term)
            .Include(t => t.ApplicationStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.ProposalId, cancellationToken);

        if (proposal == null) throw new NotFoundException("Proposal not found");

        var isStudent = proposal.StudentId == currentUserId;
        var isAdvisor = proposal.AdvisorId == currentUserId;
        var isAdmin = _currentUserService.Role == "Admin"; // simplistic check

        if (!isStudent && !isAdvisor && !isAdmin)
        {
            throw new UnauthorizedAccessException("You are not authorized to view this proposal details.");
        }

        return new ProposalDetailsVm
        {
            Id = proposal.Id,
            Title = proposal.ProposedTitle,
            Abstract = proposal.ProposedAbstract,
            StudentName = $"{proposal.Student.FirstName} {proposal.Student.LastName}",
            AdvisorName = $"{proposal.Advisor.FirstName} {proposal.Advisor.LastName}",
            TermName = proposal.Term.Name,
            Status = proposal.ApplicationStatus.Name,
            AdvisorComment = proposal.AdvisorComment,
            SubmittedAt = proposal.SubmittedAt,
            DecidedAt = proposal.DecidedAt,
            IsMyProposal = isStudent
        };
    }
}
