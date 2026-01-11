using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Defense;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Defense.Events.Commands.CreateDefenseEvent;

public class CreateDefenseEventHandler : IRequestHandler<CreateDefenseEventCommand, Guid>
{
    private readonly IGtmsDbContext _context;

    public CreateDefenseEventHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDefenseEventCommand request, CancellationToken cancellationToken)
    {
        var term = await _context.AcademicTerms.FindAsync(new object[] { request.TermId }, cancellationToken);
        if (term == null)
            throw new NotFoundException("AcademicTerm", request.TermId);

        var existingEvent = await _context.DefenseEvents
            .AnyAsync(e => e.TermId == request.TermId, cancellationToken);
        
        if (existingEvent)
        {
            throw new InvalidOperationException("A Defense Day is already defined for this term. Only one is allowed.");
        }

        var defenseEvent = new DefenseEvent
        {
            Id = Guid.NewGuid(),
            TermId = request.TermId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDurationMinutes = request.SlotDurationMinutes,
            Location = request.Location,
            CreatedAt = DateTime.UtcNow,

        };

        _context.DefenseEvents.Add(defenseEvent);
        await _context.SaveChangesAsync(cancellationToken);

        return defenseEvent.Id;
    }
}
