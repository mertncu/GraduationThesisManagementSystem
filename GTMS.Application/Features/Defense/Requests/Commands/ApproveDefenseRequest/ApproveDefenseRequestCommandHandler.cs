using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Defense;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Defense.Requests.Commands.ApproveDefenseRequest;

public class ApproveDefenseRequestCommandHandler : IRequestHandler<ApproveDefenseRequestCommand, Guid>
{
    private readonly IGtmsDbContext _context;

    public ApproveDefenseRequestCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ApproveDefenseRequestCommand request, CancellationToken cancellationToken)
    {
        // 1. Get Thesis
        var thesis = await _context.ThesisProjects
            .Include(t => t.ThesisStatus)
            .FirstOrDefaultAsync(t => t.Id == request.ThesisId, cancellationToken);

        if (thesis == null)
            throw new NotFoundException("ThesisProject", request.ThesisId);

        // Validate Status
        if (thesis.ThesisStatus.Name != "DefenseRequested")
            throw new InvalidOperationException("Thesis is not in 'DefenseRequested' status.");

        // 2. Find Defense Event for this Term
        var defenseEvent = await _context.DefenseEvents
            .Include(d => d.Sessions)
            .FirstOrDefaultAsync(d => d.TermId == thesis.TermId, cancellationToken); // Assuming Thesis has TermId

        if (defenseEvent == null)
            throw new InvalidOperationException("No Defense Day defined for this term. Contact Admin.");

        // 3. Find Available Slot
        // Logic: Iterate from StartTime to EndTime by SlotDuration.
        // Check if any existing session occupies that slot.
        
        var availableStartTime = TimeSpan.Zero;
        bool slotFound = false;

        var currentSlotStart = defenseEvent.StartTime;
        var endOfDay = defenseEvent.EndTime;
        var slotDuration = TimeSpan.FromMinutes(defenseEvent.SlotDurationMinutes);

        while (currentSlotStart + slotDuration <= endOfDay)
        {
            // Check collision
            bool isOccupied = defenseEvent.Sessions.Any(s => s.StartTime == currentSlotStart);
            
            if (!isOccupied)
            {
                availableStartTime = currentSlotStart;
                slotFound = true;
                break;
            }

            currentSlotStart = currentSlotStart.Add(slotDuration);
        }

        if (!slotFound)
        {
            throw new InvalidOperationException("No available slots left in the Defense Day for this term.");
        }

        // 4. Create Defense Session
        var session = new DefenseSession
        {
            Id = Guid.NewGuid(),
            DefenseEventId = defenseEvent.Id,
            ThesisId = thesis.Id,
            StartTime = availableStartTime,
            EndTime = availableStartTime.Add(slotDuration),

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.DefenseSessions.Add(session);

        // 5. Update Thesis Status -> DefenseScheduled
        var scheduledStatus = await _context.ThesisStatuses.FirstOrDefaultAsync(s => s.Name == "DefenseScheduled", cancellationToken);
        if (scheduledStatus == null) throw new InvalidOperationException("DefenseScheduled status not found.");

        thesis.ThesisStatusId = scheduledStatus.Id;
        thesis.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}
