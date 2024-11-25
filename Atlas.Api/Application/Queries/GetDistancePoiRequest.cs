using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Factories;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Queries;

public record GetDistancePoiRequest(Guid UserId, Guid PoiId) : IRequest<Result<double>>;

public class GetDistancePoiRequestHandler(ILogger<GetDistancePoiRequestHandler> logger,
    IUserRepository userRepository,
    IPointOfInterestRepository pointOfInterestRepository
    ) : IRequestHandler<GetDistancePoiRequest, Result<double>>
{
    private readonly ILogger<GetDistancePoiRequestHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPointOfInterestRepository _pointOfInterestRepository = pointOfInterestRepository;

    public async Task<Result<double>> Handle(GetDistancePoiRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all User in a region request received.");
        var user = await _userRepository.GetAsync(request.UserId);

        if (user is null)
        {
            _logger.LogInformation("No user found with id: {UserID}.", request.UserId);
            return Result.Fail("No user found");
        }

        var poi = await _pointOfInterestRepository.GetAsync(request.PoiId);
        if (poi is null)
        {
            _logger.LogInformation("No point of interest found with id: {PoiID}.", request.PoiId);
            return Result.Fail("No point of interest found");
        }

        var distance = Haversine.Calculate(user.Location.Y, user.Location.X, poi.Location.Y, poi.Location.X);

        return Result.Ok(distance);
    }
}