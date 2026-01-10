using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Academic.AcademicTerms.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Academic.AcademicTerms.Queries.GetAcademicTerms;

public record GetAcademicTermsQuery : IRequest<List<AcademicTermDto>>;

public class GetAcademicTermsQueryHandler : IRequestHandler<GetAcademicTermsQuery, List<AcademicTermDto>>
{
    private readonly IGtmsDbContext _context;

    public GetAcademicTermsQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<AcademicTermDto>> Handle(GetAcademicTermsQuery request, CancellationToken cancellationToken)
    {
        return await _context.AcademicTerms
            .OrderByDescending(t => t.StartDate)
            .Select(t => new AcademicTermDto
            {
                Id = t.Id,
                Name = t.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            })
            .ToListAsync(cancellationToken);
    }
}
