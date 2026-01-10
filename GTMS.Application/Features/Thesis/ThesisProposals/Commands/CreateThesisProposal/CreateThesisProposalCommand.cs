using MediatR;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Commands.CreateThesisProposal;

public record CreateThesisProposalCommand : IRequest<Guid>
{
    public string Title { get; init; } = null!;
    public string Abstract { get; init; } = null!;
    public Guid AdvisorId { get; init; }
    public Guid TermId { get; init; }
    public string? ResearchArea { get; init; }
}
