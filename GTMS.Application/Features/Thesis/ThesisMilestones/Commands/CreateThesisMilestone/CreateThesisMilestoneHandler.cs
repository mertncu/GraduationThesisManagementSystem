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

    public CreateThesisMilestoneHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
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

        return entity.Id;
    }
}
