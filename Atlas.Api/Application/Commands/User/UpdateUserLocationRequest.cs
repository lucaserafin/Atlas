using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Factories;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Commands.User;

public record UpdateUserLocationRequest(Guid Guid, CoordinateDto Coordinate) : IRequest<Result>;

public class UpdateUserLocationRequestHandler(IUserRepository userRepository,
     ILogger<UpdateUserLocationRequestHandler> logger) : IRequestHandler<UpdateUserLocationRequest, Result>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<UpdateUserLocationRequestHandler> _logger = logger;
    public async Task<Result> Handle(UpdateUserLocationRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user location with Guid: {Guid}", request.Guid);
        var user = await _userRepository.GetAsync(request.Guid);
        if(user is null)
        {
            return Result.Fail("User not found");
        }

        user.AssociateLocationData(PointFactory.CreatePoint(request.Coordinate.Latitude, request.Coordinate.Longitude));
        _logger.LogInformation("User location with Guid: {Guid} updated successfully", request.Guid);
        _userRepository.Update(user);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}