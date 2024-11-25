using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Application.Factories;

public static class PointFactory
{
    public static Point CreatePoint(double latitude, double longitude)
    {
        const int srid = 4326;
        var gf = NtsGeometryServices.Instance.CreateGeometryFactory(srid);
        return gf.CreatePoint(CreateCoordinate(latitude, longitude));
    }

    public static Coordinate CreateCoordinate(double latitude, double longitude)
    {
        return new Coordinate(longitude, latitude);
    }
}
