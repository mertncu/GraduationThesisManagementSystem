using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Academic;
using MediatR;

namespace GTMS.Application.Features.Academic.AcademicTerms.Commands.CreateAcademicTerm;

public class CreateAcademicTermCommandHandler : IRequestHandler<CreateAcademicTermCommand, Guid>
{
    private readonly IGtmsDbContext _context;

    public CreateAcademicTermCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateAcademicTermCommand request, CancellationToken cancellationToken)
    {
        var entity = new AcademicTerm
        {
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        _context.AcademicTerms.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
