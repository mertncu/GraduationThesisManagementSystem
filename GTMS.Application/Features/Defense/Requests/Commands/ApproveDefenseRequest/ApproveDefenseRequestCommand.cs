using MediatR;

namespace GTMS.Application.Features.Defense.Requests.Commands.ApproveDefenseRequest;

public class ApproveDefenseRequestCommand : IRequest<Guid>
{
    public Guid ThesisId { get; set; }
}
