using FluentValidation;
using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Defense;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Defense.Jury.Commands.AddJuryMember;

public class AddJuryMemberCommand : IRequest<Guid>
{
    public Guid DefenseSessionId { get; set; }
    public Guid? AdvisorId { get; set; }
    public string? ExternalName { get; set; }
    public string? ExternalInstitution { get; set; }
    public string? ExternalEmail { get; set; }
    public bool IsChair { get; set; }
}

public class AddJuryMemberCommandValidator : AbstractValidator<AddJuryMemberCommand>
{
    public AddJuryMemberCommandValidator()
    {
        RuleFor(v => v.DefenseSessionId).NotEmpty();

        When(v => v.AdvisorId.HasValue, () =>
        {
            RuleFor(v => v.ExternalName).Empty().WithMessage("External details should be empty when selecting internal advisor.");
        });

        When(v => !v.AdvisorId.HasValue, () =>
        {
            RuleFor(v => v.ExternalName).NotEmpty().WithMessage("Name is required for external member.");
            RuleFor(v => v.ExternalInstitution).NotEmpty().WithMessage("Institution is required for external member.");
            RuleFor(v => v.ExternalEmail).NotEmpty().EmailAddress().WithMessage("Valid email required for external member.");
        });
    }
}

public class AddJuryMemberCommandHandler : IRequestHandler<AddJuryMemberCommand, Guid>
{
    private readonly IGtmsDbContext _context;

    public AddJuryMemberCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddJuryMemberCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.DefenseSessions
            .Include(d => d.JuryMembers)
            .FirstOrDefaultAsync(d => d.Id == request.DefenseSessionId, cancellationToken);

        if (session == null)
            throw new ArgumentException("Defense Session not found.");

        if (session.JuryMembers.Count >= 5)
            throw new InvalidOperationException("Maximum jury size (5) reached.");

        if (request.IsChair && session.JuryMembers.Any(j => j.IsChair))
            throw new InvalidOperationException("Jury Chair is already assigned.");

        var juryMember = new DefenseJury
        {
            Id = Guid.NewGuid(),
            DefenseSessionId = session.Id,
            AdvisorId = request.AdvisorId,
            ExternalName = request.ExternalName,
            ExternalInstitution = request.ExternalInstitution,
            ExternalEmail = request.ExternalEmail,
            IsChair = request.IsChair,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.DefenseJuries.Add(juryMember);
        await _context.SaveChangesAsync(cancellationToken);

        return juryMember.Id;
    }
}
