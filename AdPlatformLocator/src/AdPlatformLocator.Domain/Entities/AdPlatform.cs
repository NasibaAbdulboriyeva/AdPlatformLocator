namespace AdPlatformLocator.Domain.Entities;

public class AdPlatform
{
    public string Name { get; }
    public IReadOnlyCollection<Location> Locations { get; }

    public AdPlatform(string name, IEnumerable<Location> locations)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (locations == null || !locations.Any()) throw new ArgumentException("At least one location is required");

        Name = name;
        Locations = locations.ToList().AsReadOnly();
    }

    public override string ToString() => Name;
}
