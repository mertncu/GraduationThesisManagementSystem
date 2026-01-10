using GTMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetThesisProposals;

public class ThesisProposalsVm 
{
    public List<ThesisProposalDto> Proposals { get; set; } = new();
    public bool CanCreateProposal { get; set; }
}

public class GetThesisProposalsQuery : IRequest<ThesisProposalsVm>
{
}

public class GetThesisProposalsQueryHandler : IRequestHandler<GetThesisProposalsQuery, ThesisProposalsVm>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetThesisProposalsQueryHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ThesisProposalsVm> Handle(GetThesisProposalsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return new ThesisProposalsVm();

        var query = _context.ThesisApplications
            .Include(t => t.Student)
            .Include(t => t.Advisor)
            .Include(t => t.Term)
            .Include(t => t.ApplicationStatus)
            .AsNoTracking()
            .AsQueryable();


        // Ideally ICurrentUserService should provide Roles. 
        // If not, we rely on the fact that we can filter by StudentId == userId or AdvisorId == userId if checks fail.
        // But for "Global" view (Admin), we need role info.
        // Let's implement logic based on what we match.
        
        // Actually, Controller used User.IsInRole. ICurrentUserService usually provides UserId.
        // I will just filter: if User is Student, filter by StudentId. If Advisor, by AdvisorId.
        // But how do we know which one? 
        // We can check if user exists in Students table or Advisors table.
        
        // Better: Try to match both and see. Or simpler: The controller logic was cleaner with Roles. 
        // I will assume ICurrentUserService has UserId, and I will try to fetch Student and Advisor records for this UserId to determine context.
        
        var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        var advisor = await _context.Advisors.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

        if (student != null)
        {
            query = query.Where(t => t.StudentId == userId);
        }
        else if (advisor != null)
        {
            query = query.Where(t => t.AdvisorId == userId);
        }
        // If neither (e.g. Admin), return all.

        var proposals = await query
            .OrderByDescending(t => t.SubmittedAt)
            .Select(t => new ThesisProposalDto
            {
                Id = t.Id,
                Title = t.ProposedTitle,
                Abstract = t.ProposedAbstract,
                StudentName = $"{t.Student.FirstName} {t.Student.LastName}",
                AdvisorName = $"{t.Advisor.FirstName} {t.Advisor.LastName}",
                TermName = t.Term.Name,
                Status = t.ApplicationStatus.Name,
                SubmittedAt = t.SubmittedAt,
                DecidedAt = t.DecidedAt
            })
            .ToListAsync(cancellationToken);

        bool canCreate = true;
        if (student != null)
        {
             // Logic from controller: 
             // canCreate = !Any(Active or Pending)
             canCreate = !await _context.ThesisApplications.AnyAsync(t => t.StudentId == userId && 
                                                                          (t.ApplicationStatus.Name == "Approved" || t.ApplicationStatus.Name == "Pending"), cancellationToken);
        }
        else
        {
            canCreate = false; // Only students create proposals
        }

        return new ThesisProposalsVm
        {
            Proposals = proposals,
            CanCreateProposal = canCreate
        };
    }
}
