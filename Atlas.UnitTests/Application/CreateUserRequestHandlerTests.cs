using Atlas.Api.Application.Commands.User;
using Atlas.Api.Application.Dto;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Atlas.UnitTests.Application;

public class CreateUserRequestHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserRequestHandler> _logger;
    private readonly CreateUserRequestHandler _handler;

    public CreateUserRequestHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<CreateUserRequestHandler>>();
        _handler = new CreateUserRequestHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", new CoordinateDto(10.0, 20.0));
        _userRepository.UsernameExistAsync(Arg.Any<string>()).Returns(false);
        _userRepository.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be("testuser");
        await _userRepository.Received(1).AddAsync(Arg.Any<User>());
        await _userRepository.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureWhenUsernameExists()
    {
        // Arrange
        var request = new CreateUserRequest("testuser", new CoordinateDto(10.0, 20.0));
        _userRepository.UsernameExistAsync(Arg.Any<string>()).Returns(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Username already exist");
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
        await _userRepository.UnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
