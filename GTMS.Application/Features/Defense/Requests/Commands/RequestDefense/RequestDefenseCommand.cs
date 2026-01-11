using MediatR;

namespace GTMS.Application.Features.Defense.Requests.Commands.RequestDefense;

public class RequestDefenseCommand : IRequest<Guid>
{
    public Guid ThesisId { get; set; }
    public string StudentComment { get; set; } = string.Empty;
}
