using Atlas.Api.Application.Commands.User;
using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using Atlas.Api.Infrastructure.Contracts;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Atlas.Api.Hubs;

public class AtlasHub(
    ILogger<AtlasHub> logger,
    IMediator mediator) : Hub<IAtlasHub>
{
    private readonly ILogger<AtlasHub> _logger = logger;
    private readonly IMediator _mediator = mediator;

    public async Task UpdateUserLocation(Guid userId, double latitudine, double longitudine)
    {
        var updateUserLocation = await _mediator.Send(new UpdateUserLocationRequest(userId, new CoordinateDto(Latitude: latitudine, Longitude: longitudine)));
        if(updateUserLocation.IsFailed)
        {
            _logger.LogWarning("Failed to update user location with Guid: {Guid}", userId);
            await Clients.Caller.UpdateUserLocationFailed(updateUserLocation.Errors);
            return;
        }

        const int distance = 1000;
        var nearPointOfInterest = await _mediator.Send(new GetNearPointOfInterestRequest(userId, distance));
        if (nearPointOfInterest.IsFailed)
        {
            _logger.LogWarning("Failed to get near point of interest for user with Guid: {Guid}", userId);
            await Clients.Caller.GetNearPointOfInterestFailed( nearPointOfInterest.Errors);
            return;
        }
        
        await Clients.Caller.UpdateUserLocationSuccess(nearPointOfInterest.Value);
    }
}
