using GTMS.Application.Features.Thesis.ThesisProposals.Dtos;
using MediatR;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetAdvisorProposals;

public record GetAdvisorProposalsQuery : IRequest<List<ThesisProposalDto>>;
