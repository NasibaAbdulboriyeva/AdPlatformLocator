using Xunit;
using AdPlatformLocator.Domain.Entities;
using AdPlatformLocator.Infrastructure.Persistence;
using System.Diagnostics;


public class InMemoryAdPlatformRepositoryTests : IDisposable
{
    private readonly InMemoryAdPlatformRepository _repository;

    public InMemoryAdPlatformRepositoryTests()
    {
        _repository = new InMemoryAdPlatformRepository();

        _repository.AddPlatforms(new[]
        {
            new AdPlatform("Яндекс.Директ", new[] { "/ru" }),
            new AdPlatform("Ревдинский рабочий", new[] { "/ru/svrd/revda" }),
            new AdPlatform("Крутая реклама", new[] { "/ru/svrd" })
        });
    }

    public void Dispose()
    {
        _repository.Clear();
    }

    [Fact]
    public void FindPlatformsForLocation_ExactMatch_ReturnsPlatform()
    {
        // Act
        var result = _repository.FindPlatformsForLocation("/ru/svrd/revda").ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains("Ревдинский рабочий", result);
        Assert.Contains("Крутая реклама", result);
        Assert.Contains("Яндекс.Директ", result);
    }

    [Fact]
    public void FindPlatformsForLocation_ParentLocation_ReturnsPlatforms()
    {
        var result = _repository.FindPlatformsForLocation("/ru/svrd").ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains("Крутая реклама", result);
        Assert.Contains("Яндекс.Директ", result);
    }

    [Fact]
    public void FindPlatformsForLocation_NonExistingLocation_ReturnsEmpty()
    {
        var result = _repository.FindPlatformsForLocation("/unknown");
        Assert.Empty(result);
    }

    [Fact]
    public void AddPlatforms_DuplicatePlatform_DoesNotDuplicate()
    {
        var platforms = new[]
        {
            new AdPlatform("Яндекс.Директ", new[] { "/ru" })
        };

        _repository.AddPlatforms(platforms);
        var result = _repository.FindPlatformsForLocation("/ru").ToList();

        Assert.Single(result);
    }

    [Fact]
    public void Clear_RemovesAllPlatforms()
    {
        _repository.Clear();
        var result = _repository.FindPlatformsForLocation("/ru");
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("/ru/svrd/revda", new[] { "Крутая реклама", "Ревдинский рабочий", "Яндекс.Директ" })]
    [InlineData("/ru/svrd", new[] { "Крутая реклама", "Яндекс.Директ" })]
    [InlineData("/ru", new[] { "Яндекс.Директ" })]
    [InlineData("/unknown", new string[0])]
    public void FindPlatformsForLocation_VariousCases_ReturnsExpected(string location, string[] expected)
    {
        // Act
        var result = _repository.FindPlatformsForLocation(location).OrderBy(x => x).ToList();
        var expectedOrdered = expected.OrderBy(x => x).ToList();

        // Assert
        Assert.Equal(expectedOrdered, result);
    }

    [Theory]
    [InlineData("/RU", "/ru")]
    [InlineData(" /ru/msk ", "/ru/msk")]
    [InlineData("/ru//msk", "/ru/msk")]
    public void AddPlatforms_NormalizesLocationFormats(string inputLocation, string normalizedLocation)
    {
        var platform = new AdPlatform("Тест", new[] { inputLocation });

        _repository.AddPlatforms(new[] { platform });
        var result = _repository.FindPlatformsForLocation(normalizedLocation);

        Assert.Contains("Тест", result);
    }

    [Fact]
    public void FindPlatformsForLocation_NullLocation_ReturnsEmpty()
    {
        var result = _repository.FindPlatformsForLocation(null);
        Assert.Empty(result);
    }

    [Fact]
    public void AddPlatforms_MultipleLocationsForSinglePlatform_AddsAll()
    {
        var platform = new AdPlatform("МультиЛокация", new[] { "/ru", "/ru/msk", "/eu" });

        _repository.AddPlatforms(new[] { platform });

        Assert.Contains("МультиЛокация", _repository.FindPlatformsForLocation("/ru"));
        Assert.Contains("МультиЛокация", _repository.FindPlatformsForLocation("/ru/msk"));
        Assert.Contains("МультиЛокация", _repository.FindPlatformsForLocation("/eu"));
    }

    [Fact]
    public void FindPlatformsForLocation_LargeDataSet_PerformanceTest()
    {
        var platforms = Enumerable.Range(1, 10000)
            .Select(i => new AdPlatform($"Platform{i}", new[] { $"/loc/{i}", $"/parent/{i % 100}" }));

        _repository.AddPlatforms(platforms);

        var watch = Stopwatch.StartNew();
        var result = _repository.FindPlatformsForLocation("/parent/50").Count();
        watch.Stop();

        Assert.True(watch.ElapsedMilliseconds < 100,
            $"Поиск занял {watch.ElapsedMilliseconds} мс, ожидалось < 100 мс");
        Assert.True(result >= 100);
    }

    [Fact]
    public async Task AddPlatforms_ConcurrentAccess_NoRaceConditions()
    {
        var tasks = new List<Task>();
        var platformCount = 1000;

        for (int i = 0; i < platformCount; i++)
        {
            int index = i;
            tasks.Add(Task.Run(() =>
            {
                _repository.AddPlatforms(new[]
                {
                    new AdPlatform($"Platform{index}", new[] { $"/loc/{index}" })
                });
            }));
        }

        await Task.WhenAll(tasks);

        Assert.Equal(platformCount, _repository.FindPlatformsForLocation("/loc/0").Count() + platformCount - 1);
    }

    [Fact]
    public void AddPlatforms_DuplicateLocations_ProcessesCorrectly()
    {
        var platform = new AdPlatform("Дубль", new[] { "/dupe", "/dupe" });

        _repository.AddPlatforms(new[] { platform });
        var result = _repository.FindPlatformsForLocation("/dupe");

        Assert.Single(result);
    }

    [Fact]
    public void AddPlatforms_SpecialCharacters_InLocations()
    {
        var platform = new AdPlatform("СпецСимволы", new[] { "/ru/msk/кириллица", "/space in path" });

        _repository.AddPlatforms(new[] { platform });

        Assert.Contains("СпецСимволы", _repository.FindPlatformsForLocation("/ru/msk/кириллица"));
        Assert.Contains("СпецСимволы", _repository.FindPlatformsForLocation("/space in path"));
    }
    [Fact]
    public void AddPlatforms_ContainsNullPlatforms_ProcessesCorrectly()
    {
        // Arrange
        var platforms = new AdPlatform[]
        {
        new("Valid", new[] { "/valid" }),
        null
        };

        // Act
        _repository.AddPlatforms(platforms);
        var result = _repository.FindPlatformsForLocation("/valid");

        // Assert
        Assert.Single(result);
        Assert.Contains("Valid", result);
    }
   
}