using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using AdPlatformLocator.Infrastructure.Persistence;
using System.Text;

public class TextFileAdParserTests
{
    private readonly Mock<ILogger<TextFileAdParser>> _loggerMock = new();
    private readonly TextFileAdParser _parser;

    public TextFileAdParserTests()
    {
        _parser = new TextFileAdParser(_loggerMock.Object);
    }

    [Fact]
    public async Task ParseAsync_ValidFile_ReturnsCorrectPlatforms()
    {
        // Arrange
        var content = @"Яндекс.Директ:/ru
Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";
        using var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(content);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var result = (await _parser.ParseAsync(stream)).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        var yandex = result.First();
        Assert.Equal("Яндекс.Директ", yandex.Name);
        Assert.Equal("/ru", yandex.Locations.Single());

        var revda = result.Last();
        Assert.Equal("Ревдинский рабочий", revda.Name);
        Assert.Equal(2, revda.Locations.Count);
        Assert.Contains("/ru/svrd/revda", revda.Locations);
    }

    [Fact]
    public async Task ParseAsync_EmptyFile_ThrowsArgumentException()
    {
        // Arrange
        using var emptyStream = new MemoryStream();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _parser.ParseAsync(emptyStream));
    }

    [Fact]
    public async Task ParseAsync_InvalidFormat_ThrowsFormatException()
    {
        // Arrange
        var invalidContent = "Площадка без разделителя";
        using var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(invalidContent);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() => _parser.ParseAsync(stream));
    }

    [Fact]
    public async Task ParseAsync_LineWithEmptyLocations_ThrowsFormatException()
    {
        // Arrange
        var content = "Площадка без локаций:";
        using var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(content);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() => _parser.ParseAsync(stream));
    }

    [Fact]
    public async Task ParseAsync_PlatformWithoutLocations_SkipsInvalidLine()
    {
        // Arrange
        var content = @"Яндекс.Директ:/ru
Невалидная площадка:
Ревдинский рабочий:/ru/svrd";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<FormatException>(() => _parser.ParseAsync(stream));
        Assert.Contains("No locations found", ex.Message); 
    }

    [Fact]
    public async Task ParseAsync_MultiplePlatforms_HandlesCommasCorrectly()
    {
        // Arrange
        var content = "Площадка:/loc1,/loc2, /loc3";
        using var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(content);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var result = await _parser.ParseAsync(stream);

        // Assert
        var platform = result.Single();
        Assert.Equal(3, platform.Locations.Count);
        Assert.Contains("/loc1", platform.Locations);
        Assert.Contains("/loc3", platform.Locations);
    }
}