using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Defense.Requests.Commands.RequestDefense;

public class RequestDefenseCommandHandler : IRequestHandler<RequestDefenseCommand, Guid>
{
    private readonly IGtmsDbContext _context;

    public RequestDefenseCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RequestDefenseCommand request, CancellationToken cancellationToken)
    {
        var thesis = await _context.ThesisProjects
            .Include(t => t.ThesisStatus)
            .FirstOrDefaultAsync(t => t.Id == request.ThesisId, cancellationToken);

        if (thesis == null)
            throw new NotFoundException("ThesisProject", request.ThesisId);

        // Check Milestones
        var milestones = await _context.ThesisMilestones
            .Where(m => m.ThesisId == request.ThesisId)
            .Include(m => m.Submissions)
            .ThenInclude(s => s.SubmissionStatus)
            .ToListAsync(cancellationToken);

        if (!milestones.Any())
        {
             // Optional: Can't request defense if no milestones exist?
             // throw new InvalidOperationException("No milestones defined for this thesis.");
        }

        foreach (var milestone in milestones)
        {
            var isApproved = milestone.Submissions.Any(s => s.SubmissionStatus.Name == "Approved");
            if (!isApproved)
            {
                throw new InvalidOperationException($"Milestone '{milestone.Name}' is not approved yet. All milestones must be completed.");
            }
        }
        
        var defenseRequestedStatus = await _context.ThesisStatuses
            .FirstOrDefaultAsync(s => s.Name == "DefenseRequested", cancellationToken);

        if (defenseRequestedStatus == null)
        {
             throw new InvalidOperationException("DefenseRequested status not found in DB. Please contact Admin.");
        }

        thesis.ThesisStatusId = defenseRequestedStatus.Id;
        thesis.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return thesis.Id;
    }
}
