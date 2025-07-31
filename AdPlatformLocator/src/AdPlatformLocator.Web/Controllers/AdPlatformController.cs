using AdPlatformLocator.Application.Abstarctions.Services;
using AdPlatformLocator.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AdPlatformLocator.Web.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AdPlatformController : ControllerBase
    {
        private readonly IAdPlatformService _platformService;
        private readonly ILogger<AdPlatformController> _logger;

        public AdPlatformController(
            IAdPlatformService platformService,
            ILogger<AdPlatformController> logger)
        {
            _platformService = platformService;
            _logger = logger;
        }

        /// <summary>
        /// Загружает рекламные площадки из файла
        /// </summary>
        /// <param name="file">Текстовый файл с данными о площадках</param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPlatforms([FromForm] UploadPlatformRequest request)
        {
            if (request?.File == null || request.File.Length == 0)
            {
                return BadRequest("File is required");
            }

            try
            {
                await using var stream = request.File.OpenReadStream();
                await _platformService.LoadPlatformsFromFile(stream);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest(new
                {
                    message = "Invalid file format",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading platforms");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Ищет рекламные площадки для указанной локации
        /// </summary>
        /// <param name="location">Локация в формате /ru/msk</param>
        /// <returns>Список подходящих площадок</returns>
        [HttpGet("search")]
        public IActionResult SearchPlatforms([FromQuery] string location)
        {
            try
            {
                _logger.LogInformation("Search request for location: {Location}", location);

                var platforms = _platformService.FindPlatformsForLocation(location).ToList();

                _logger.LogInformation("Found {Count} platforms: {Platforms}",
                    platforms.Count, string.Join(", ", platforms));

                return Ok(new PlatformSearchResult
                {
                    Location = location,
                    Platforms = platforms,
                    Count = platforms.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search error for location: {Location}", location);
                return StatusCode(500, new { error = ex.Message });
            }

        }
    }
}