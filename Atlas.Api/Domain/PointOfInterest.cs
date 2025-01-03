﻿using NetTopologySuite.Geometries;

namespace Atlas.Api.Domain;

public class PointOfInterest : Entity
{
    public PointOfInterest(string name, string description, Point location)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
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


    public void AssociateLocationData(Point locationData)
    {
        UpdatedAt = DateTime.Now.ToUniversalTime();
        Location = locationData;
    }

    public void UpdateDescription(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Description = description;
        UpdatedAt = DateTime.Now.ToUniversalTime();
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        UpdatedAt = DateTime.Now.ToUniversalTime();
    }
}
