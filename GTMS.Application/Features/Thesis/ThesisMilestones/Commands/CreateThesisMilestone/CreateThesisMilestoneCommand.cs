using MediatR;

namespace GTMS.Application.Features.Thesis.ThesisMilestones.Commands.CreateThesisMilestone;

public record CreateThesisMilestoneCommand : IRequest<Guid>
{
    public Guid ThesisId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public DateTime DueDate { get; init; }
}
