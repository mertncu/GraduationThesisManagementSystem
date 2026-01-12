using GTMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Defense.Jury.Queries.GetDefenseJury;

public class GetDefenseJuryQuery : IRequest<JuryManagementVm>
{
    public Guid ThesisId { get; set; }
}

public class JuryManagementVm
{
    public Guid ThesisId { get; set; }
    public Guid DefenseSessionId { get; set; }
    public string ThesisTitle { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime DefenseDate { get; set; }
    
    public List<JuryMemberDto> Members { get; set; } = new();
    public List<AdvisorDto> AvailableAdvisors { get; set; } = new();
}

public class JuryMemberDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty; // "Internal" or external name
    public bool IsChair { get; set; }
    public bool IsExternal { get; set; }
}

public class AdvisorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

public class GetDefenseJuryQueryHandler : IRequestHandler<GetDefenseJuryQuery, JuryManagementVm>
{
    private readonly IGtmsDbContext _context;

    public GetDefenseJuryQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<JuryManagementVm> Handle(GetDefenseJuryQuery request, CancellationToken cancellationToken)
    {
        var thesis = await _context.ThesisProjects
            .Include(t => t.Student)
            .Include(t => t.DefenseSessions)
                .ThenInclude(ds => ds.DefenseEvent)
            .Include(t => t.DefenseSessions)
                .ThenInclude(ds => ds.JuryMembers)
                    .ThenInclude(jm => jm.Advisor)
                        .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(t => t.Id == request.ThesisId, cancellationToken);
            
        if (thesis == null)
            throw new Exception("Thesis not found");

        var session = thesis.DefenseSessions.FirstOrDefault();
        if (session == null)
             throw new Exception("Defense Session not scheduled yet.");

        var vm = new JuryManagementVm
        {
            ThesisId = thesis.Id,
            DefenseSessionId = session.Id,
            ThesisTitle = thesis.Title,
            StudentName = $"{thesis.Student.FirstName} {thesis.Student.LastName}",
            DefenseDate = session.DefenseEvent.Date.Date.Add(session.StartTime),
            Members = session.JuryMembers.Select(m => new JuryMemberDto
            {
                Id = m.Id,
                Name = m.AdvisorId.HasValue ? $"{m.Advisor!.User.FirstName} {m.Advisor.User.LastName}" : m.ExternalName!,
                Institution = m.AdvisorId.HasValue ? "Internal" : m.ExternalInstitution!,
                IsChair = m.IsChair,
                IsExternal = !m.AdvisorId.HasValue
            }).ToList()
        };

        // Fetch all advisors for dropdown
        var advisors = await _context.Advisors
            .Include(a => a.User)
            .Include(a => a.Department)
            .Where(a => a.User.IsActive)
            .ToListAsync(cancellationToken);

        vm.AvailableAdvisors = advisors.Select(a => new AdvisorDto
        {
            Id = a.Id,
            Name = $"{a.User.FirstName} {a.User.LastName}",
            Department = a.Department.Name
        }).ToList();

        return vm;
    }
}
