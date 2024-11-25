namespace Atlas.Api.Application.Factories;

public static class Haversine
{
    //https://rosettacode.org/wiki/Haversine_formula#C#
    public static double Calculate(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6372.8; // In kilometers
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        lat1 = ToRadians(lat1);
        lat2 = ToRadians(lat2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2 * Math.Asin(Math.Sqrt(a));
        return R * 2 * Math.Asin(Math.Sqrt(a));
    }

    public static double ToRadians(double angle)
    {
        return Math.PI * angle / 180.0;
    }
}
