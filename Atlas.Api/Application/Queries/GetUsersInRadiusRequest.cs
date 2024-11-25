using Atlas.Api.Application.Dto;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Application.Queries;

public record GetUsersInRadiusRequest(double Latitude, double Longitude, double DistanceInKm) : IRequest<Result<IEnumerable<UserDto>>>;

public class GetUsersInRadiusRequestHandler(ILogger<GetUsersInRadiusRequestHandler> logger,
    IUserRepository userRepository
    ) : IRequestHandler<GetUsersInRadiusRequest, Result<IEnumerable<UserDto>>>
{
    private readonly ILogger<GetUsersInRadiusRequestHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<Result<IEnumerable<UserDto>>> Handle(GetUsersInRadiusRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all User in a radius request received.");
        var point = Factories.PointFactory.CreatePoint(request.Latitude, request.Longitude);
        var users = await _userRepository.GetAllUsersInRadius(point, request.DistanceInKm);

        if (users is null || !users.Any())
        {
            _logger.LogInformation("No users found in the radius.");
            return Result.Fail("No users found");
        }

        return Result.Ok(users.Select(x => x.ToUserDto()));
    }
}