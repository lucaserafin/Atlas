using Atlas.Api.Domain;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Application.Dto;

public record UserDto(Guid Guid,string Username, double Latitude, double Longitude);

public static class UserDtoExtensions
{
    public static UserDto ToUserDto(this User user)
    {
        return new UserDto(user.Guid, user.Username, user.Location.Y, user.Location.X);
    }
}