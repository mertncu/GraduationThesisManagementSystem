using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Academic.AcademicTerms.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

using GTMS.Domain.Entities.Academic;

namespace GTMS.Application.Features.Academic.AcademicTerms.Queries.GetAcademicTermById;

public record GetAcademicTermByIdQuery(Guid Id) : IRequest<AcademicTermDto>;

public class GetAcademicTermByIdQueryHandler : IRequestHandler<GetAcademicTermByIdQuery, AcademicTermDto>
{
    private readonly IGtmsDbContext _context;

    public GetAcademicTermByIdQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<AcademicTermDto> Handle(GetAcademicTermByIdQuery request, CancellationToken cancellationToken)
    {
        var term = await _context.AcademicTerms
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
            
        if (term == null)
        {
            throw new NotFoundException(nameof(AcademicTerm), request.Id);
        }

        return new AcademicTermDto
        {
            Id = term.Id,
            Name = term.Name,
            StartDate = term.StartDate,
            EndDate = term.EndDate
        };
    }
}
