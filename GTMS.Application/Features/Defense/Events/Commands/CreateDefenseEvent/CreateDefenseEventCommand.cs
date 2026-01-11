using MediatR;

namespace GTMS.Application.Features.Defense.Events.Commands.CreateDefenseEvent;

public class CreateDefenseEventCommand : IRequest<Guid>
{
    public Guid TermId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 60;
    public string Location { get; set; } = string.Empty;
}
