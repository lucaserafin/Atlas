using NetTopologySuite.Geometries;

namespace Atlas.Api.Domain;

public class PointOfInterest : Entity
{
    public PointOfInterest(string name, string description, Point location)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Description = description;
        Location = location;
        CreatedAt = DateTime.Now.ToUniversalTime();
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Point Location { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    internal void AssociateLocationData(Point locationData)
    {
        UpdatedAt = DateTime.Now.ToUniversalTime();
        Location = locationData;
    }

    internal void UpdateDescription(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Description = description;
    }

    internal void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}
