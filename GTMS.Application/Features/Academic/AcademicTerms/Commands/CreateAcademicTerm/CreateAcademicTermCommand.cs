using MediatR;

namespace GTMS.Application.Features.Academic.AcademicTerms.Commands.CreateAcademicTerm;

public record CreateAcademicTermCommand : IRequest<Guid>
{
    public string Name { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
