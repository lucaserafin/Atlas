using Atlas.Api.Application.Commands.User;
using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Api.Api;

public static class UserApi
{
    public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/users", CreateUser);
        builder.MapGet("/api/users", GetAllUsers);
        builder.MapGet("/api/users/{id}", GetUser);
        builder.MapPut("/api/users/{id}", UpdateUser);
        builder.MapDelete("/api/users/{id}", DeleteUser);

        builder.MapPut("/api/users/{id}/location", UpdateUserLocation);

        builder.MapGet("/api/users/{id}/PointOfInterest", GetNearPointOfInterest);
        return builder;
    }


    public static async Task<IResult> CreateUser(CreateUserRequest request, IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result switch
        {
            { IsSuccess: true } => Results.Created($"/api/users/{result.Value.Guid}", result.Value),
            { IsSuccess: false } => Results.BadRequest(result.Errors),
            _ => Results.BadRequest()
        };
    }

    public static async Task<IResult> GetUser(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetUserRequest(id));
        return result switch
        {
            { IsSuccess: true } => Results.Ok(result.Value),
            { IsSuccess: false } => Results.NotFound(result.Errors),
            _ => Results.BadRequest()
        };
    }

    public static async Task<IResult> GetAllUsers(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllUserRequest());
        return result switch
        {
            { IsSuccess: true } => Results.Ok(result.Value),
            { IsSuccess: false } => Results.NotFound(result.Errors),
            _ => Results.BadRequest()
        };
    }
    public static async Task<IResult> UpdateUser(Guid id, UserDto user, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateUserRequest(id, user));
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }
        if (result.HasError(e => e.Message.Equals("User not found")))
        {
            return Results.NotFound(result.Errors);
        }
        if (result.HasError(e => e.Message.Equals("User already exists")))
        {
            return Results.BadRequest(result.Errors);
        }
        return Results.BadRequest();
    }

    public static async Task<IResult> DeleteUser(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new DeleteUserRequest(id));
        return result switch
        {
            { IsSuccess: true } => Results.NoContent(),
            { IsSuccess: false } => Results.NotFound(result.Errors),
            _ => Results.BadRequest()
        };
    }

    public static async Task<IResult> UpdateUserLocation(Guid id, CoordinateDto coordinate, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateUserLocationRequest(id, coordinate));
        return result switch
        {
            { IsSuccess: true } => Results.NoContent(),
            { IsSuccess: false } => Results.NotFound(result.Errors),
            _ => Results.BadRequest()
        };
    }

    private static async Task<IResult> GetNearPointOfInterest(Guid id, [FromQuery] double distance, IMediator mediator)
    {
        //Distance in km from API
        //To meters
        distance *= 1000;
        var result = await mediator.Send(new GetNearPointOfInterestRequest(id, distance));
        return result switch
        {
            { IsSuccess: true } => Results.Ok(result.Value),
            { IsSuccess: false } => Results.NotFound(result.Errors),
            _ => Results.BadRequest()
        };
    }

}
