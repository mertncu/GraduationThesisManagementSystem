using MediatR;

namespace GTMS.Application.Features.Defense.Events.Queries.GetDefenseEvents;

public class GetDefenseEventsQuery : IRequest<List<DefenseEventDto>>
{
}
