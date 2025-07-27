using AdPlatformLocator.Application.Abstarctions.Services;
using AdPlatformLocator.Application.Abstarctions;
using AdPlatformLocator.Domain.Entities;
using System.Text;
using AdPlatformLocator.Application.Dtos;

namespace AdPlatformLocator.Application.Services
{
    public class AdPlatformService : IAdPlatformService
    {
        private readonly IAdPlatformRepository _repository;

        public AdPlatformService(IAdPlatformRepository repository)
        {
            _repository = repository;
        }

        public void LoadFromFile(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream, Encoding.UTF8);
            var platforms = new List<AdPlatform>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(':');
                if (parts.Length != 2) continue;

                var name = parts[0].Trim();
                var locationsRaw = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);

                var locations = locationsRaw
                    .Select(l => new Location(l.Trim()))
                    .ToList();

                platforms.Add(new AdPlatform(name, locations));
            }

            _repository.OverWriteAll(platforms);
        }

        public List<AdPlatformResponse> FindPlatformsForLocation(SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LocationPath))
                throw new ArgumentException("Location is required");

            var location = new Location(request.LocationPath);

            var all = _repository.GetAll();

            var result = all
                .Where(p => p.Locations.Any(loc => location.IsNestedUnder(loc)))
                .Select(p => new AdPlatformResponse { Name = p.Name })
                .ToList();

            return result;
        }

    }
}