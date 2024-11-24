using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Factories;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Commands.User;

public record UpdateUserRequest(Guid Guid, UserDto userDto) : IRequest<Result<UserDto>>;

public class UpdateUserRequestHandler(IUserRepository userRepository,
    ILogger<UpdateUserRequestHandler> logger) : IRequestHandler<UpdateUserRequest, Result<UserDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<UpdateUserRequestHandler> _logger = logger;

    public async Task<Result<UserDto>> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user with Guid: {Guid}", request.Guid);

        var user = await _userRepository.GetAsync(request.Guid);
        if (user == null)
        {
            _logger.LogWarning("User with Guid: {Guid} not found", request.Guid);
            return Result.Fail("User not found");
        }
        var usernameExist = await _userRepository.UsernameExistAsync(request.userDto.Username);
        if (usernameExist)
        {
            _logger.LogWarning("Username already exist");
            return Result.Fail("Username already exist");
        }
        user.UpdateUsername(request.userDto.Username);
        user.AssociateLocationData(PointFactory.CreatePoint(request.userDto.Latitude, request.userDto.Longitude));

        _userRepository.Update(user);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User with Guid: {Guid} updated successfully", request.Guid);
        return Result.Ok(user.ToUserDto());
    }
}
