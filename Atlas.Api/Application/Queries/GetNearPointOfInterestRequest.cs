using Atlas.Api.Application.Dto;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Application.Queries;

public record GetNearPointOfInterestRequest(Guid guid, double distance) : IRequest<Result<IEnumerable<PointOfInterestDto>>>;

public class GetNearPointOfInterestRequestHandler(ILogger<GetNearPointOfInterestRequestHandler> logger,
    IUserRepository userRepository,
    IPointOfInterestRepository pointOfInterestRepository) : IRequestHandler<GetNearPointOfInterestRequest, Result<IEnumerable<PointOfInterestDto>>>
{
    private readonly ILogger<GetNearPointOfInterestRequestHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPointOfInterestRepository _pointOfInterestRepository = pointOfInterestRepository;

    public async Task<Result<IEnumerable<PointOfInterestDto>>> Handle(GetNearPointOfInterestRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting near point of interest for user with Guid: {Guid} Distance: {Distance}", request.guid, request.distance);

        var user = await _userRepository.GetAsync(request.guid);
        if (user is null)
        {
            _logger.LogWarning("User with Guid: {Guid} not found", request.guid);
            return Result.Fail("User not found");
        }
        var pointOfInterest = await _pointOfInterestRepository.GetNearPointOfInterestAsync(user.Location, request.distance);
        if (pointOfInterest is null)
        {
            _logger.LogWarning("No point of interest found near user with Guid: {Guid}", request.guid);
            return Result.Fail("No point of interest found near user");
        }

        _logger.LogInformation("Point of interest found near user with Guid: {Guid} Distance: {Distance}", request.guid, request.distance);
        return Result.Ok(pointOfInterest.Select(x => x.ToPointOfInterestDto()));
    }
}