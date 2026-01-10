using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Common.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Common.Queries.GetProgramsByDepartment;

public class GetProgramsByDepartmentQuery : IRequest<List<ProgramDto>>
{
    public Guid DepartmentId { get; set; }

    public GetProgramsByDepartmentQuery(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
}

public class GetProgramsByDepartmentQueryHandler : IRequestHandler<GetProgramsByDepartmentQuery, List<ProgramDto>>
{
    private readonly IGtmsDbContext _context;

    public GetProgramsByDepartmentQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProgramDto>> Handle(GetProgramsByDepartmentQuery request, CancellationToken cancellationToken)
    {
        return await _context.Programs
            .Where(p => p.DepartmentId == request.DepartmentId)
            .Select(p => new ProgramDto { Id = p.Id, Name = p.Name })
            .ToListAsync(cancellationToken);
    }
}
