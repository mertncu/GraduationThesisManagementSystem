using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using MediatR;

namespace GTMS.Application.Features.Academic.AcademicTerms.Commands.DeleteAcademicTerm;

public record DeleteAcademicTermCommand(Guid Id) : IRequest;

public class DeleteAcademicTermCommandHandler : IRequestHandler<DeleteAcademicTermCommand>
{
    private readonly IGtmsDbContext _context;

    public DeleteAcademicTermCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteAcademicTermCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.AcademicTerms
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Academic.AcademicTerm), request.Id);
        }

        _context.AcademicTerms.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
