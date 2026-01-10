using MediatR;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Commands.ReviewThesisProposal;

public record ReviewThesisProposalCommand(
    Guid ProposalId,
    bool IsApproved,
    string? Comment
) : IRequest;
