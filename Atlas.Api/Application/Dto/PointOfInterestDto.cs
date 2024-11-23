using Atlas.Api.Domain;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Application.Dto;

public record PointOfInterestDto(Guid Guid,string Name, string Description , double Latitude, double Longitude);

public static class PointOfInterestExtensions
{
    public static PointOfInterestDto ToPointOfInterestDto(this PointOfInterest poi)
    {
        return new PointOfInterestDto(poi.Guid, poi.Name, poi.Description ,poi.Location.Y, poi.Location.X);
    }
}