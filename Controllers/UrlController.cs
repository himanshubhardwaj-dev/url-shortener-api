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
            if(pageNumber < 1)
            {
                return BadRequest("Page Number must be greater than 0.");
            }
            if(pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Page size must be between 1 and 100");
            }
            var response = await _urlService.GetAllAsync(pageNumber, pageSize);
            return Ok(response);
        }
    }
}
