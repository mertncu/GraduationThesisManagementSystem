using MediatR;
using GTMS.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Defense.Jury.Commands.RemoveJuryMember;

public class RemoveJuryMemberCommand : IRequest
{
    public Guid JuryMemberId { get; set; }
}

public class RemoveJuryMemberCommandHandler : IRequestHandler<RemoveJuryMemberCommand>
{
    private readonly IGtmsDbContext _context;

    public RemoveJuryMemberCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveJuryMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _context.DefenseJuries
            .FirstOrDefaultAsync(m => m.Id == request.JuryMemberId, cancellationToken);

        if (member == null)
            throw new ArgumentException("Jury member not found.");

        _context.DefenseJuries.Remove(member);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
