using AdPlatformLocator.Application.Abstarctions;
using AdPlatformLocator.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AdPlatformLocator.Infrastructure.Persistence
{
    public class TextFileAdParser : IAdFileParser
    {
        private readonly ILogger<TextFileAdParser> _logger;

        public TextFileAdParser(ILogger<TextFileAdParser> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<AdPlatform>> ParseAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            var content = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("File is empty");
            }

            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var platforms = new List<AdPlatform>();
            var errors = new List<string>();

            foreach (var (line, index) in lines.Select((line, i) => (line, i + 1)))
            {
                try
                {
                    var platform = ParseLine(line);
                    if (platform == null)
                    {
                        errors.Add($"Line {index}: Empty platform data");
                        continue;
                    }

                    if (!platform.Locations.Any())
                    {
                        errors.Add($"Line {index}: No locations found for platform");
                        continue;
                    }

                    platforms.Add(platform);
                }
                catch (Exception ex)
                {
                    errors.Add($"Line {index}: {ex.Message}");
                }
            }

            if (errors.Count > 0)
            {
                throw new FormatException($"Invalid file format:\n{string.Join("\n", errors)}");
            }

            if (platforms.Count == 0)
            {
                throw new FormatException("No valid platforms found in file");
            }

            return platforms;
        }
            private AdPlatform? ParseLine(string line)
            {
                if (string.IsNullOrWhiteSpace(line))
                    return null;

                var parts = line.Split(':', 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2)
                    throw new FormatException($"Invalid line format - expected 'PlatformName:Location1,Location2'");

                var name = parts[0];
                if (string.IsNullOrWhiteSpace(name))
                    throw new FormatException("Platform name cannot be empty");

                var locations = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                return new AdPlatform(name, locations);
            }
        
    }
    }
