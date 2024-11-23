using Atlas.Api.Application.Commands;
using Atlas.Api.Application.Commands.Poi;
using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure;
using MediatR;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Api;

public static class PointOfInterestApi
{
    public static IEndpointRouteBuilder MapPointOfInterestApi(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/pointofinterests", CreatePointOfInterest);
        builder.MapGet("/api/pointofinterests", GetAllPointOfInterests);
        builder.MapGet("/api/pointofinterests/{id}", GetPointOfInterest);
        builder.MapPut("/api/pointofinterests/{id}", UpdatePointOfInterest);
        builder.MapDelete("/api/pointofinterests/{id}", DeletePointOfInterest);

        return builder;
    }


    public static async Task<IResult> CreatePointOfInterest(CreatePointOfInterestRequest request, IMediator mediator)
     {
        var result = await mediator.Send(request);
        return result switch
        {
            { IsSuccess: true} => Results.Created($"/api/pointofinterests/{result.Value.Guid}", result.Value),
            { IsSuccess: false } => Results.BadRequest(result.Errors),
            _ => Results.BadRequest()
        };
    }

    public static async Task<IResult> GetPointOfInterest(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetPointOfInterestRequest(id));
        return result switch
        {
            { IsSuccess: true } => Results.Ok(result.Value),
            { IsSuccess: false } => Results.NotFound(result.Errors),
            _ => Results.BadRequest()
        };
    }

    public static async Task<IResult> GetAllPointOfInterests(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllPointOfInterestRequest());
        return result switch
        {
            { IsSuccess: true } => Results.Ok(result.Value),
            { IsSuccess: false } => Results.NotFound(result.Errors),
            _ => Results.BadRequest()
        };
    }
    public static async Task<IResult> UpdatePointOfInterest(Guid id, PointOfInterestDto PointOfInterest,IMediator mediator)
    {
        var result = await mediator.Send(new UpdatePointOfInterestRequest(id,PointOfInterest));
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }
        if(result.HasError(e => e.Message.Equals("PointOfInterest not found")))
        {
            return Results.NotFound(result.Errors);
        }
        if (result.HasError(e => e.Message.Equals("PointOfInterest already exists")))
        {
            return Results.BadRequest(result.Errors);
        }
        return Results.BadRequest();
    }

    public static async Task<IResult> DeletePointOfInterest(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new DeletePointOfInterestRequest(id));
        return result switch
        {
            { IsSuccess: true } => Results.NoContent(),
            { IsSuccess: false } => Results.NotFound(result.Errors),
            _ => Results.BadRequest()
        };
    }

}
