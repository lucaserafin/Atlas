using Atlas.Api.Application.Dto;
using FluentResults;
using Microsoft.AspNetCore.SignalR;

namespace Atlas.Api.Hubs;

public interface IAtlasHub 
{
    Task UpdateUserLocationSuccess(IEnumerable<PointOfInterestDto> nearestPois);
    Task UpdateUserLocationFailed(IEnumerable<IError> Errors);
    Task GetNearPointOfInterestFailed(IEnumerable<IError> Errors);
}
