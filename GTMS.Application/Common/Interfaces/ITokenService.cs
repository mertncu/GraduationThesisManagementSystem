using GTMS.Domain.Entities.Identity;

namespace GTMS.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(User user, IList<string> roles);
    RefreshToken GenerateRefreshToken();
}
