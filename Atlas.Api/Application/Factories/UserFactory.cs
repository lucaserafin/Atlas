using Atlas.Api.Domain;

namespace Atlas.Api.Application.Factories;

public static class UserFactory
{
    public static User CreateUser(string username, double latitude, double longitude)
    {
        var location = PointFactory.CreatePoint(latitude, longitude);
        return new User(username, location);
    }
}
