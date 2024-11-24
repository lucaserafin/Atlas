using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Factories;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Commands.Poi;

public record UpdatePointOfInterestRequest(Guid Guid, PointOfInterestDto PointOfInterestDto) : IRequest<Result<PointOfInterestDto>>;

public class UpdatePointOfInterestRequestHandler(IPointOfInterestRepository PointOfInterestRepository,
    ILogger<UpdatePointOfInterestRequestHandler> logger) : IRequestHandler<UpdatePointOfInterestRequest, Result<PointOfInterestDto>>
{
    private readonly IPointOfInterestRepository _PointOfInterestRepository = PointOfInterestRepository;
    private readonly ILogger<UpdatePointOfInterestRequestHandler> _logger = logger;

    public async Task<Result<PointOfInterestDto>> Handle(UpdatePointOfInterestRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating PointOfInterest with Guid: {Guid}", request.Guid);

        var pointOfInterest = await _PointOfInterestRepository.GetAsync(request.Guid);
        if (pointOfInterest == null)
        {
            _logger.LogWarning("PointOfInterest with Guid: {Guid} not found", request.Guid);
            return Result.Fail("PointOfInterest not found");
        }
        var PointOfInterestnameExist = await _PointOfInterestRepository.PointOfInterestNameExistAsync(request.PointOfInterestDto.Name);
        if (PointOfInterestnameExist)
        {
            _logger.LogWarning("PointOfInterestname already exist");
            return Result.Fail("PointOfInterestname already exist");
        }
        pointOfInterest.UpdateName(request.PointOfInterestDto.Name);
        pointOfInterest.UpdateDescription(request.PointOfInterestDto.Description);
        pointOfInterest.AssociateLocationData(PointFactory.CreatePoint(request.PointOfInterestDto.Latitude, request.PointOfInterestDto.Longitude));

        _PointOfInterestRepository.Update(pointOfInterest);
        await _PointOfInterestRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PointOfInterest with Guid: {Guid} updated successfully", request.Guid);
        return Result.Ok(pointOfInterest.ToPointOfInterestDto());
    }
}
