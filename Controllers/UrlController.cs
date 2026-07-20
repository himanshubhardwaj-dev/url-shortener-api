using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortner.API.DTOs;
using URLShortner.API.Interfaces;

namespace URLShortner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;
        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateShortUrlRequestDto requestDto)
        {
            var response = await _urlService.CreateAsync(requestDto);
            return Created(response.ShortUrl, response);
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
