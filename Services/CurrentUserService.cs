using System.Security.Claims;
using URLShortner.API.Interfaces;

namespace URLShortner.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userId, out int id))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                return id;
            }
        }
    }
}
