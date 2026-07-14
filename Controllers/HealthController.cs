using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using URLShortner.API.Data;

namespace URLShortner.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly UrlShortnerDbContext _ctx;
        public HealthController(UrlShortnerDbContext ctx)
        {
            _ctx = ctx;
        }
        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
           bool isDatabaseConnected = await _ctx.Database.CanConnectAsync();
            if (isDatabaseConnected)
            {
                return Ok(new
                {
                    status = "Healthy",
                    database = "connected"
                });

            }
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "Unhealth",
                database = "Disconnected"
            });
        }
    }
}
