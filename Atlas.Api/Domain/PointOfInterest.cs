using Microsoft.AspNetCore.Http.HttpResults;
using NetTopologySuite.Geometries;
using System.Xml.Linq;

namespace Atlas.Api.Domain;

public class PointOfInterest : Entity
{
    public PointOfInterest(string name, string description, Point location)
    {
        Name = name;
        Description = description;
        Location = location;
        CreatedAt = DateTime.Now;
    }

    public string Name { get; }
    public string Description { get; }
    public Point Location { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
