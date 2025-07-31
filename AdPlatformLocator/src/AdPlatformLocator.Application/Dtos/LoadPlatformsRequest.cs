using Microsoft.AspNetCore.Http;

namespace AdPlatformLocator.Application.Dtos
{
    public class LoadPlatformsRequest
    {
        public IFormFile File { get; set; }
    }
}
