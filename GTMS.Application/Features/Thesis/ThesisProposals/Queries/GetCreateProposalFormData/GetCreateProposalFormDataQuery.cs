using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Common.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetCreateProposalFormData;

public class CreateProposalFormDataVm
{
    public List<SelectListItemDto> Advisors { get; set; } = new();
    public List<SelectListItemDto> Terms { get; set; } = new();
    public bool CanCreate { get; set; }
    public string? WarningMessage { get; set; }
}

public class GetCreateProposalFormDataQuery : IRequest<CreateProposalFormDataVm>
{
}

public class GetCreateProposalFormDataQueryHandler : IRequestHandler<GetCreateProposalFormDataQuery, CreateProposalFormDataVm>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCreateProposalFormDataQueryHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CreateProposalFormDataVm> Handle(GetCreateProposalFormDataQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var vm = new CreateProposalFormDataVm();

        // Validation Check
        bool hasActiveProposal = await _context.ThesisApplications.AnyAsync(t => t.StudentId == userId && 
                                                                               (t.ApplicationStatus.Name == "Approved" || t.ApplicationStatus.Name == "Pending"), cancellationToken);
        if (hasActiveProposal)
        {
            vm.CanCreate = false;
            vm.WarningMessage = "You already have an active or pending thesis proposal.";
            return vm;
        }

        vm.CanCreate = true;

        vm.Advisors = await _context.Users
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Advisor") && u.IsActive)
            .Select(u => new SelectListItemDto { Id = u.Id, Name = $"{u.FirstName} {u.LastName}" })
            .ToListAsync(cancellationToken);

        var today = DateTime.UtcNow;
        vm.Terms = await _context.AcademicTerms
            .Where(t => t.StartDate <= today && t.EndDate >= today)
            .Select(t => new SelectListItemDto { Id = t.Id, Name = t.Name })
            .ToListAsync(cancellationToken);

        return vm;
    }
}
