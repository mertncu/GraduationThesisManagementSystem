using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using MediatR;

namespace GTMS.Application.Features.Academic.AcademicTerms.Commands.UpdateAcademicTerm;

public record UpdateAcademicTermCommand(Guid Id, string Name, DateTime StartDate, DateTime EndDate) : IRequest;

public class UpdateAcademicTermCommandHandler : IRequestHandler<UpdateAcademicTermCommand>
{
    private readonly IGtmsDbContext _context;

    public UpdateAcademicTermCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAcademicTermCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.AcademicTerms
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Academic.AcademicTerm), request.Id);
        }

        entity.Name = request.Name;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
