using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Factories;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Commands.User;

public record CreateUserRequest(string Username, CoordinateDto Coordinate) : IRequest<Result<UserDto>>;

public class CreateUserRequestHandler(IUserRepository userRepository,
    ILogger<CreateUserRequestHandler> logger) : IRequestHandler<CreateUserRequest, Result<UserDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<CreateUserRequestHandler> _logger = logger;

    public async Task<Result<UserDto>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user");

        var user = UserFactory.CreateUser(request.Username, request.Coordinate.Latitude, request.Coordinate.Longitude);
        bool userAlreadyExist = await _userRepository.UsernameExistAsync(user.Username);
        if (userAlreadyExist)
        {
            _logger.LogWarning("Username already exist");
            return Result.Fail("Username already exist");
        }

        await _userRepository.AddAsync(user);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User created");
        return user.ToUserDto();
    }
}