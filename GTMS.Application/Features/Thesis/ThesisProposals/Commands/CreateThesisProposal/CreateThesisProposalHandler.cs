using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Thesis;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Commands.CreateThesisProposal;

public class CreateThesisProposalHandler : IRequestHandler<CreateThesisProposalCommand, Guid>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateThesisProposalHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateThesisProposalCommand request, CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId;
        if (studentId == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var advisorExists = await _context.Users.AnyAsync(u => u.Id == request.AdvisorId, cancellationToken);
        if (!advisorExists)
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("AdvisorId", "Advisor not found.") });
        }

        var status = await _context.ThesisStatuses.FirstOrDefaultAsync(s => s.Name == "Proposed", cancellationToken);
        if (status == null)
        {
            throw new NotFoundException("ThesisStatus", "Proposed");
        }

        var department = await _context.Departments.FirstOrDefaultAsync(cancellationToken);
        var program = await _context.Programs.FirstOrDefaultAsync(cancellationToken);

        if (department == null || program == null)
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("System", "No Department or Program found in system.") });
        }

        var entity = new ThesisProject
        {
            StudentId = studentId.Value,
            MainAdvisorId = request.AdvisorId,
            DepartmentId = department.Id,
            ProgramId = program.Id,
            TermId = request.TermId,
            Title = request.Title,
            Abstract = request.Abstract,
            ThesisStatusId = status.Id,
        };

        _context.ThesisProjects.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
