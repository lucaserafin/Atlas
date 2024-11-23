using Atlas.Api.Domain;

namespace Atlas.Api.Application.Factories;

public static class PointOfInterestFactory
{
    public static PointOfInterest CreatePointOfInterest(string name, string description, double latitude, double longitude)
    {
        var location = PointFactory.CreatePoint(latitude, longitude);
        return new PointOfInterest(name, description, location);
    }
}
