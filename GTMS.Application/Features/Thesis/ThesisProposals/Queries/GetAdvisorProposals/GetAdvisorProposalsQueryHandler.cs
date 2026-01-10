using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Thesis.ThesisProposals.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetAdvisorProposals;

public class GetAdvisorProposalsQueryHandler : IRequestHandler<GetAdvisorProposalsQuery, List<ThesisProposalDto>>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetAdvisorProposalsQueryHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<ThesisProposalDto>> Handle(GetAdvisorProposalsQuery request, CancellationToken cancellationToken)
    {
        var advisorUserId = _currentUserService.UserId;
        if (advisorUserId == null) throw new UnauthorizedAccessException();

        var proposals = await _context.ThesisApplications
            .Include(t => t.Student)
            .Include(t => t.Advisor)
            .Include(t => t.Term)
            .Include(t => t.ApplicationStatus)
            .Where(t => t.AdvisorId == advisorUserId)
            .OrderByDescending(t => t.SubmittedAt)
            .Select(t => new ThesisProposalDto
            {
                Id = t.Id,
                StudentName = $"{t.Student.FirstName} {t.Student.LastName}",
                AdvisorName = $"{t.Advisor.FirstName} {t.Advisor.LastName}",
                TermName = t.Term.Name,
                Title = t.ProposedTitle,
                Abstract = t.ProposedAbstract,
                Status = t.ApplicationStatus.Name,
                SubmittedAt = t.SubmittedAt
            })
            .ToListAsync(cancellationToken);

        return proposals;
    }
}
