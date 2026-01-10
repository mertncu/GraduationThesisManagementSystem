using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Common.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Users.Queries.GetCreateUserFormData;

public class GetCreateUserFormDataQuery : IRequest<CreateUserFormDataVm>
{
}

public class CreateUserFormDataVm
{
    public List<DepartmentDto> Departments { get; set; } = new();
    public List<ProgramDto> Programs { get; set; } = new();
}

public class GetCreateUserFormDataQueryHandler : IRequestHandler<GetCreateUserFormDataQuery, CreateUserFormDataVm>
{
    private readonly IGtmsDbContext _context;

    public GetCreateUserFormDataQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<CreateUserFormDataVm> Handle(GetCreateUserFormDataQuery request, CancellationToken cancellationToken)
    {
        return new CreateUserFormDataVm
        {
            Departments = await _context.Departments
                .Select(d => new DepartmentDto { Id = d.Id, Name = d.Name })
                .ToListAsync(cancellationToken),
            Programs = await _context.Programs
                .Select(p => new ProgramDto { Id = p.Id, DepartmentId = p.DepartmentId, Name = p.Name })
                .ToListAsync(cancellationToken)
        };
    }
}
