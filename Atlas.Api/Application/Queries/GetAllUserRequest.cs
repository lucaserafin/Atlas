using Atlas.Api.Application.Dto;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Queries;

public record GetAllUserRequest() : IRequest<Result<IEnumerable<UserDto>>>;

public class GetAllUserRequestHandler(IUserRepository userRepository, ILogger<GetAllUserRequestHandler> logger) : IRequestHandler<GetAllUserRequest, Result<IEnumerable<UserDto>>>
{
    private readonly ILogger<GetAllUserRequestHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<Result<IEnumerable<UserDto>>> Handle(GetAllUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all user request received.");
        var users = await _userRepository.GetAllAsync();
        if (users is null || !users.Any())
        {
            return Result.Fail("No users on DB");
        }
        _logger.LogInformation("Users found: {Count}.", users.Count());
        return Result.Ok(users.Select(x => x.ToUserDto()));
    }
}