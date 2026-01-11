using GTMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Defense.Events.Queries.GetDefenseEvents;

public class GetDefenseEventsQueryHandler : IRequestHandler<GetDefenseEventsQuery, List<DefenseEventDto>>
{
    private readonly IGtmsDbContext _context;

    public GetDefenseEventsQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<DefenseEventDto>> Handle(GetDefenseEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _context.DefenseEvents
            .Include(e => e.Term)
            .Include(e => e.Sessions)
            .OrderByDescending(e => e.Date)
            .Select(e => new DefenseEventDto
            {
                Id = e.Id,
                TermName = e.Term.Name,
                Date = e.Date,
                StartTime = e.StartTime.ToString(@"hh\:mm"),
                EndTime = e.EndTime.ToString(@"hh\:mm"),
                Location = e.Location,
                SlotDurationMinutes = e.SlotDurationMinutes,
                TotalSessions = e.Sessions.Count
            })
            .ToListAsync(cancellationToken);

        return events;
    }
}
