using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using Atlas.Api.Domain;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlas.Api.Infrastructure.Contracts;

namespace Atlas.UnitTests.Application;

public class GetAllUserRequestHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllUserRequestHandler> _logger;
    private readonly GetAllUserRequestHandler _handler;

    public GetAllUserRequestHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<GetAllUserRequestHandler>>();
        _handler = new GetAllUserRequestHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDtos_WhenUsersAreFound()
    {
        // Arrange
        var users = new List<User>
        {
            new User("testuser1", new NetTopologySuite.Geometries.Point(10.0, 20.0)),
            new User("testuser2", new NetTopologySuite.Geometries.Point(30.0, 40.0))
        };
        _userRepository.GetAllAsync().Returns(users);

        var request = new GetAllUserRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.First().Username.Should().Be("testuser1");
        result.Value.Last().Username.Should().Be("testuser2");
        await _userRepository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoUsersAreFound()
    {
        // Arrange
        _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>());

        var request = new GetAllUserRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No users on DB");
        await _userRepository.Received(1).GetAllAsync();
    }
}
