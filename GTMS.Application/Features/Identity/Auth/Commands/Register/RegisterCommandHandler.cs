using GTMS.Application.Common.Interfaces;
using GTMS.Application.Common.Exceptions;



using GTMS.Application.Features.Identity.Auth.Dtos;
using GTMS.Domain.Entities.Academic;
using GTMS.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IGtmsDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(IGtmsDbContext context, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            throw new ValidationException(new[] 
             { 
                 new FluentValidation.Results.ValidationFailure("Email", "Email is already taken.") 
             });
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true
        };

        var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student", cancellationToken);
        if (studentRole != null)
        {
            user.UserRoles.Add(new UserRole { RoleId = studentRole.Id });
        }
        
        _context.Users.Add(user);
        var accessToken = _tokenService.GenerateJwtToken(user, new[] { "Student" });
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        user.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, accessToken, refreshToken.Token, DateTime.UtcNow.AddMinutes(15), "User");
    }
}
