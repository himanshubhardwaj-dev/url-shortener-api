using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortner.API.DTOs.Auth;
using URLShortner.API.Interfaces;

namespace URLShortner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
    }
}
