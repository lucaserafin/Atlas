using Atlas.Api.Application.Dto;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Application.Queries;

public record GetUsersInRegionRequest(Polygon Polygon) : IRequest<Result<IEnumerable<UserDto>>>;

public class GetUsersInRegionRequestHandler(ILogger<GetUsersInRegionRequestHandler> logger,
    IUserRepository userRepository
    ) : IRequestHandler<GetUsersInRegionRequest, Result<IEnumerable<UserDto>>>
{
    private readonly ILogger<GetUsersInRegionRequestHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<Result<IEnumerable<UserDto>>> Handle(GetUsersInRegionRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all User in a region request received.");
        var users = await _userRepository.GetAllUsersInRegion(request.Polygon);

        if(users is null || !users.Any())
        {
            _logger.LogInformation("No users found in the region.");
            return Result.Fail("No users found");
        }

        return Result.Ok(users.Select(x => x.ToUserDto()));
    }
}