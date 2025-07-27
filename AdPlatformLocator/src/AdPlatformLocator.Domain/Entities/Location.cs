namespace AdPlatformLocator.Domain.Entities;

public class Location : IEquatable<Location>
{
    public string Path { get; }

    public Location(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("/"))
            throw new Exception("Location path must start with '/' and not be empty.");

        Path = path.TrimEnd('/').ToLowerInvariant();
    }

   
    public bool IsNestedUnder(Location other)
    {
        return Path.StartsWith(other.Path + "/") || Path == other.Path;
    }

    public override string ToString() => Path;
    public override int GetHashCode() => Path.GetHashCode();

    public bool Equals(Location? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Path == other.Path;
    }
}
