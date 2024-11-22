using NetTopologySuite.Geometries;

namespace Atlas.Api.Domain;

public class User : Entity
{
    public User(string username, Point location)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        Username = username;
        AssociateLocationData(location);
    }

    public string Username { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public Point Location { get; private set; }
    public void AssociateLocationData(Point locationData)
    {
        UpdatedAt = DateTime.Now.ToUniversalTime();
        Location = locationData;
    }
}
