using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortner.API.Interfaces;

namespace URLShortner.API.Controllers
{
    [Route("")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public RedirectController(IUrlService urlService)
        {
            _urlService = urlService;
        }
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToUrl(string shortCode)
        {
            var url = await _urlService.ResolveShortCodeAsync(shortCode);

            if (url == null)
                return NotFound();

            return Redirect(url.OriginalUrl);
        }
    }
}
