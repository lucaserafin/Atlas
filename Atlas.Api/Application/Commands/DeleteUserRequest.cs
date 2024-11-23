using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Factories;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Commands;

public record DeleteUserRequest(Guid Guid) : IRequest<Result>;

public class DeleteUserRequestHandler(IUserRepository userRepository, 
    ILogger<DeleteUserRequestHandler> logger) : IRequestHandler<DeleteUserRequest, Result>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<DeleteUserRequestHandler> _logger = logger;

    public async Task<Result> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete user request received. {Guid}", request.Guid);
        var user = await _userRepository.GetAsync(request.Guid);
        if (user == null)
        {
            _logger.LogWarning("User not found");
            return Result.Fail("User not found");
        }

        _userRepository.Remove(user);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User deleted");
        return Result.Ok();
    }
}