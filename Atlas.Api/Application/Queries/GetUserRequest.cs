using Atlas.Api.Application.Dto;
using Atlas.Api.Infrastructure;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Queries;

public record GetUserRequest(Guid Guid) : IRequest<Result<UserDto>>;

public class GetUserRequestHandler(IUserRepository userRepository, ILogger<GetUserRequestHandler> logger) : IRequestHandler<GetUserRequest, Result<UserDto>>
{
    private readonly ILogger<GetUserRequestHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<Result<UserDto>> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get user request received. {Guid}",request.Guid);
        var user = await _userRepository.GetAsync(request.Guid);
        if (user == null)
        {
            return Result.Fail("User not found");
        }
        _logger.LogInformation("User: {Username} found.", user.Username);
        return user.ToUserDto();
    }
}