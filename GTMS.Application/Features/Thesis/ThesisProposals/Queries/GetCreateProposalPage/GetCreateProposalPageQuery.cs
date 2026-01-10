using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Common.Dtos;
using GTMS.Application.Features.Thesis.ThesisProposals.Dtos;
using GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetCreateProposalFormData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetCreateProposalPage;

public record GetCreateProposalPageQuery : IRequest<CreateProposalPageVm>;

public class GetCreateProposalPageQueryHandler : IRequestHandler<GetCreateProposalPageQuery, CreateProposalPageVm>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCreateProposalPageQueryHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CreateProposalPageVm> Handle(GetCreateProposalPageQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        bool canCreate = true;
        string warning = string.Empty;

        // Check if student has active proposal
        if (userId != null)
        {
             var hasActive = await _context.ThesisApplications
                 .AnyAsync(t => t.StudentId == userId && 
                               (t.ApplicationStatus.Name == "Approved" || t.ApplicationStatus.Name == "Pending"), cancellationToken);
             if (hasActive)
             {
                 canCreate = false;
                 warning = "You already have an active or pending thesis proposal.";
             }
        }

        var advisors = await _context.Advisors
             .Include(a => a.User)
             .AsNoTracking()
             .Select(a => new SelectListItemDto 
             { 
                 Id = a.Id, 
                 Name = $"{a.User.FirstName} {a.User.LastName}" 
             })
             .ToListAsync(cancellationToken);

        var terms = await _context.AcademicTerms
             .Where(t => t.StartDate <= DateTime.UtcNow && t.EndDate >= DateTime.UtcNow) // Only active terms? Or future. For now, all or logic. 
             // Logic: Usually we pick upcoming or current. Let's just return all active/future.
             .OrderByDescending(t => t.StartDate)
             .AsNoTracking()
             .Select(t => new SelectListItemDto 
             { 
                 Id = t.Id, 
                 Name = t.Name 
             })
             .ToListAsync(cancellationToken);

        return new CreateProposalPageVm
        {
            CanCreate = canCreate,
            WarningMessage = warning,
            Advisors = advisors,
            Terms = terms
        };
    }
}
