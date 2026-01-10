using MediatR;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Commands.ApproveThesisProposal;

public record ApproveThesisProposalCommand : IRequest<Unit>
{
    public Guid ThesisId { get; init; }
    public bool IsApproved { get; init; }
    public string? Feedback { get; init; }
}
