using URLShortner.API.DTOs.Auth;
using URLShortner.API.Models;

namespace URLShortner.API.Interfaces
{
    public interface ITokenService
    {
        TokenResultDto GenerateToken(User user);
    }
}
