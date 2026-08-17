using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortner.API.DTOs;
using URLShortner.API.Interfaces;

namespace URLShortner.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly IQRCodeService _qRCodeService;
        public UrlController(IUrlService urlService, IQRCodeService qRCodeService)
        {
            _urlService = urlService;
            _qRCodeService = qRCodeService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateShortUrlRequestDto requestDto)
        {
            try
            {

                var response = await _urlService.CreateAsync(requestDto);
                response.ShortUrl = $"{Request.Scheme}://{Request.Host}/{response.ShortCode}";
                return Created(response.ShortUrl, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{shortCode}")]
        public async Task<IActionResult> Delete(string shortCode)
        {
            var deleted = await _urlService.DeleteAsync(shortCode);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return BadRequest("Page Number must be greater than 0.");
            }
            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Page size must be between 1 and 100");
            }
            var response = await _urlService.GetAllAsync(pageNumber, pageSize);
            return Ok(response);
        }
        [HttpGet("{shortCode}/qrcode")]
        public async Task<IActionResult> GetQrCode(string shortCode)
        {
            var url = await _urlService.GetByShortCodeAsync(shortCode);
            if (url == null)
            {
                return NotFound();
            }
            var shortUrl = $"{Request.Scheme}://{Request.Host}/{url.ShortCode}";
            var qrCodeBytes = await _qRCodeService.GetorCreateQRCodeAsync(url.ShortCode, shortUrl);
            return File(qrCodeBytes, "image/png");
        }
        [HttpGet("{shortCode}/analytics")]
        public async Task<IActionResult> GetAnalytics(string shortCode)
        {
            var analytics = await _urlService.GetAnalyticsAsync(shortCode);
                if(analytics == null)
            {
                return NotFound();
            }
            analytics.ShortUrl = $"{Request.Scheme}://{Request.Host}/{analytics.ShortCode}";
            return Ok(analytics);
        }
    }
}
