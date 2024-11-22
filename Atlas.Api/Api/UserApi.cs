using Atlas.Api.Application.Dto;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Api;

public static class UserApi
{
    public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/users", async (UserRepository repo) =>
        {
            return (await repo.GetAllAsync()).Select( x => x.ToUserDto());
        });

        builder.MapGet("/api/users/{id}", async (UserRepository repo, Guid id) =>
        {
            return (await repo.GetAsync(id)).ToUserDto();
        });

        builder.MapPost("/api/users", async (UserRepository repo, UserDto userDto) =>
        {
            var user = new User(userDto.Username, new Point(userDto.Longitude,userDto.Latitude));
            await repo.Add(user);
            await repo.UnitOfWork.SaveChangesAsync();
            return Results.Created($"/api/users/{user.Id}", user.ToUserDto());
        });

        builder.MapPut("/api/users/{id}", async (UserRepository repo, Guid id, UserDto userDto) =>
        {
            //if (id != userDto.Id)
            //{
            //    return Results.BadRequest();
            //}
            var user = await repo.GetAsync(id);
            if (user == null)
            {
                return Results.NotFound();
            }
            //repo.Update(user with { Username = userDto.Username, Location = new Point(userDto.Longitude, userDto.Latitude) });
            await repo.UnitOfWork.SaveChangesAsync();
            return Results.NoContent();
        });

        builder.MapDelete("/api/users/{id}", async (UserRepository repo, Guid id) =>
        {
            var user = await repo.GetAsync(id);
            if (user == null)
            {
                return Results.NotFound();
            }

            repo.Remove(user);
            await repo.UnitOfWork.SaveChangesAsync();
            return Results.NoContent();
        });

        return builder;
    }
}
