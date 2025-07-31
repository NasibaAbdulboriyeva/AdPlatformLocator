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
            if (platforms == null)
            {
                throw new ArgumentNullException(nameof(platforms), "Platforms collection cannot be null");
            }

            foreach (var platform in platforms)
            {
                if (platform?.Locations == null) continue;

                foreach (var location in platform.Locations)
                {
                    if (string.IsNullOrWhiteSpace(location)) continue;

                    var normalizedLocation = location.Trim().ToLower()
                        .Replace("//", "/", StringComparison.Ordinal);

                    _locationIndex.AddOrUpdate(
                        normalizedLocation,
                        new List<string> { platform.Name },
                        (_, list) =>
                        {
                            if (!list.Contains(platform.Name))
                                list.Add(platform.Name);
                            return list;
                        });
                }
            }
        }

        public IEnumerable<string> FindPlatformsForLocation(string location)
        {
            var searchLocation = location?.Trim().ToLower() ?? string.Empty;
            var result = new HashSet<string>();

            // Проверяем все уровни иерархии
            while (!string.IsNullOrEmpty(searchLocation))
            {
                if (_locationIndex.TryGetValue(searchLocation, out var platforms))
                {
                    foreach (var platform in platforms)
                    {
                        result.Add(platform);
                    }
                }

                var lastSlash = searchLocation.LastIndexOf('/');
                searchLocation = lastSlash > 0 ? searchLocation[..lastSlash] : null;
            }

            return result;
        }
    }
    }
