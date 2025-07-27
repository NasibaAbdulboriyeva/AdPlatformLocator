using AdPlatformLocator.Application.Abstarctions.Services;
using AdPlatformLocator.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AdPlatformLocator.Web.Controllers
{

    [ApiController]
    [Route("api/adplatforms")]
    public class AdPlatformController : ControllerBase
    {
        private readonly IAdPlatformService _service;
        private readonly ILogger<AdPlatformController> _logger;

        public AdPlatformController(IAdPlatformService service, ILogger<AdPlatformController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            using var stream = file.OpenReadStream();
            _service.LoadFromFile(stream);

            return Ok("Ad platforms uploaded successfully.");
        }

        [HttpGet("search")]
        public ActionResult<List<AdPlatformResponse>> Search([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Location is required.");
            }

            var result = _service.FindPlatformsForLocation(new SearchRequest
            {
                LocationPath = location
            });

            return Ok(result);
        }
    }
}