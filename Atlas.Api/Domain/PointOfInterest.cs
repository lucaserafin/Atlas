using Microsoft.AspNetCore.Http.HttpResults;
using NetTopologySuite.Geometries;
using System.Xml.Linq;

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

    public string Name { get; }
    public string Description { get; }
    public Point Location { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
