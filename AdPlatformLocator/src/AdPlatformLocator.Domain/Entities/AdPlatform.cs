namespace AdPlatformLocator.Domain.Entities;

// Models/AdPlatform.cs
public class AdPlatform
{
    public string Name { get; }
    public IReadOnlyList<string> Locations { get; }

    public AdPlatform(string name, IEnumerable<string> locations)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Locations = locations?.ToList() ?? throw new ArgumentNullException(nameof(locations));
    }
}
