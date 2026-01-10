using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Identity.Auth.Dtos;
using GTMS.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IGtmsDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IGtmsDbContext context, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("Email", "Invalid credentials.") });
        }

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("Email", "Invalid credentials.") });
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var accessToken = _tokenService.GenerateJwtToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, accessToken, refreshToken.Token, DateTime.UtcNow.AddMinutes(15), roles.FirstOrDefault() ?? "User");
    }
}
