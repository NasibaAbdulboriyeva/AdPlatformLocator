using AdPlatformLocator.Application.Abstarctions;
using AdPlatformLocator.Domain.Entities;
using System.Collections.Concurrent;

namespace AdPlatformLocator.Infrastructure.Persistence
{

    public class InMemoryAdPlatformRepository : IAdPlatformRepository
    {
        private readonly ConcurrentDictionary<string, List<string>> _locationIndex = new();

        public void Clear() => _locationIndex.Clear();

        public void AddPlatforms(IEnumerable<AdPlatform> platforms)
        {
            foreach (var platform in platforms)
            {
                foreach (var location in platform.Locations)
                {
                    _locationIndex.AddOrUpdate(
                        location.Trim().ToLower(), // Нормализуем локации
                        new List<string> { platform.Name },
                        (_, list) => { list.Add(platform.Name); return list; });

                    Console.WriteLine($"Added: {platform.Name} -> {location}"); // Логирование
                }
            }
        }

        public IEnumerable<string> FindPlatformsForLocation(string location)
        {
            var searchLocation = location?.Trim().ToLower() ?? string.Empty;
            Console.WriteLine($"Searching for: {searchLocation}"); // Логирование

            var result = new HashSet<string>();

            while (!string.IsNullOrEmpty(searchLocation))
            {
                if (_locationIndex.TryGetValue(searchLocation, out var platforms))
                {
                    Console.WriteLine($"Found {platforms.Count} platforms for {searchLocation}");
                    foreach (var p in platforms) result.Add(p);
                }

                var lastSlash = searchLocation.LastIndexOf('/');
                searchLocation = lastSlash >= 0 ? searchLocation[..lastSlash] : null;
            }

            return result;
        }
    }
}
