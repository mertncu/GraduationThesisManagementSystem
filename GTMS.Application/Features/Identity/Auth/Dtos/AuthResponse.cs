namespace GTMS.Application.Features.Identity.Auth.Dtos;

public record AuthResponse(Guid UserId, string Email, string FirstName, string LastName, string AccessToken, string RefreshToken, DateTime ExpiresAt, string UserRole);
