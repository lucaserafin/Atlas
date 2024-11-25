using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Queries;
using Atlas.Api.Infrastructure.Contracts;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Atlas.Api.Domain;

namespace Atlas.UnitTests.Application;

public class GetUsersInRadiusRequestHandlerTests
{
    private readonly ILogger<GetUsersInRadiusRequestHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly GetUsersInRadiusRequestHandler _handler;

    public GetUsersInRadiusRequestHandlerTests()
    {
        _logger = Substitute.For<ILogger<GetUsersInRadiusRequestHandler>>();
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new GetUsersInRadiusRequestHandler(_logger, _userRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnUsersSuccessfully()
    {
        // Arrange
        var request = new GetUsersInRadiusRequest(10.0, 20.0, 5.0);
        var users = new List<User>
        {
            new User("User1", new Point(10.1, 20.1)),
            new User("User2", new Point(10.2, 20.2))
        };
        _userRepository.GetAllUsersInRadius(Arg.Any<Point>(), Arg.Any<double>())
            .Returns(users);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnNoUsersFound()
    {
        // Arrange
        var request = new GetUsersInRadiusRequest(10.0, 20.0, 5.0);
        _userRepository.GetAllUsersInRadius(Arg.Any<Point>(), Arg.Any<double>())
            .Returns((IEnumerable<User>)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("No users found");
    }

    [Fact]
    public async Task Handle_ShouldReturnNoUsersFoundWhenListIsEmpty()
    {
        // Arrange
        var request = new GetUsersInRadiusRequest(10.0, 20.0, 5.0);
        _userRepository.GetAllUsersInRadius(Arg.Any<Point>(), Arg.Any<double>())
            .Returns(new List<User>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("No users found");
    }
}
