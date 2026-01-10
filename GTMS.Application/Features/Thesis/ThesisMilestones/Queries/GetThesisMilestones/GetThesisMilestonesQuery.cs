using GTMS.Application.Features.Thesis.ThesisMilestones.Dtos;
using MediatR;

namespace GTMS.Application.Features.Thesis.ThesisMilestones.Queries.GetThesisMilestones;

public record GetThesisMilestonesQuery(Guid ThesisId) : IRequest<List<ThesisMilestoneDto>>;
