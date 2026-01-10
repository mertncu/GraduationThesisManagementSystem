using GTMS.Application.Common.Interfaces;
using GTMS.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IGtmsDbContext _context;

    public DeleteUserCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {request.UserId} not found.");
        }

        user.IsActive = false;
        user.DeletedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id, cancellationToken);
        if (student != null)
        {
            student.IsActive = false;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
