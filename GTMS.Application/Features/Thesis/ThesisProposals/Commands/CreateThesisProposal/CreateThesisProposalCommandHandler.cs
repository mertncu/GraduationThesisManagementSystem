using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Thesis;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Commands.CreateThesisProposal;

public class CreateThesisProposalCommandHandler : IRequestHandler<CreateThesisProposalCommand, Guid>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateThesisProposalCommandHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateThesisProposalCommand request, CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId;
        if (studentId == null || studentId == Guid.Empty)
        {
             throw new UnauthorizedAccessException("User is not authenticated.");
        }

        // Check if student already has a proposal for this term? (Optional rule, skipping for now)

        // Get Pending Status
        var status = await _context.ApplicationStatuses
            .FirstOrDefaultAsync(s => s.Name == "Pending", cancellationToken);
        
        if (status == null)
        {
            // Fallback or error? Should be seeded.
            // Let's create it if missing for robustness, or throw.
            throw new InvalidOperationException("Application Status 'Pending' not found in database.");
        }

        var entity = new ThesisApplication
        {
            Id = Guid.NewGuid(),
            StudentId = studentId.Value,
            AdvisorId = (Guid)request.AdvisorId,
            TermId = (Guid)request.TermId,
            ProposedTitle = request.Title,
            ProposedAbstract = request.Abstract,
            ApplicationStatusId = status.Id,
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ThesisApplications.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
