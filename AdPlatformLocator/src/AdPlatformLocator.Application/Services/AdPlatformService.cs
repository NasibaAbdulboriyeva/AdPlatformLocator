
using AdPlatformLocator.Application.Abstarctions;
using AdPlatformLocator.Application.Abstarctions.Services;


namespace AdPlatformLocator.Application.Services
{
    public class AdPlatformService : IAdPlatformService
    {
        private readonly IAdPlatformRepository _adRepository;
        private readonly IAdFileParser _fileParser;

        public AdPlatformService(IAdPlatformRepository adRepository, IAdFileParser fileParser)
        {
            _adRepository = adRepository;
            _fileParser = fileParser;
        }

        public async Task LoadPlatformsFromFile(Stream fileStream)
        {
            var platforms = await _fileParser.ParseAsync(fileStream);
            _adRepository.Clear();
            _adRepository.AddPlatforms(platforms);
        }

        public IEnumerable<string> FindPlatformsForLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location cannot be empty", nameof(location));

            return _adRepository.FindPlatformsForLocation(location);
        }
    }
}