using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Factories;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Commands.Poi;

public record DeletePointOfInterestRequest(Guid Guid) : IRequest<Result>;

public class DeletePointOfInterestRequestHandler(IPointOfInterestRepository PointOfInterestRepository,
    ILogger<DeletePointOfInterestRequestHandler> logger) : IRequestHandler<DeletePointOfInterestRequest, Result>
{
    private readonly IPointOfInterestRepository _PointOfInterestRepository = PointOfInterestRepository;
    private readonly ILogger<DeletePointOfInterestRequestHandler> _logger = logger;

    public async Task<Result> Handle(DeletePointOfInterestRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete PointOfInterest request received. {Guid}", request.Guid);
        var PointOfInterest = await _PointOfInterestRepository.GetAsync(request.Guid);
        if (PointOfInterest == null)
        {
            _logger.LogWarning("PointOfInterest not found");
            return Result.Fail("PointOfInterest not found");
        }

        _PointOfInterestRepository.Remove(PointOfInterest);
        await _PointOfInterestRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("PointOfInterest deleted");
        return Result.Ok();
    }
}