using Atlas.Api.Application.Dto;
using Atlas.Api.Infrastructure;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Queries;

public record GetPointOfInterestRequest(Guid Guid) : IRequest<Result<PointOfInterestDto>>;

public class GetPointOfInterestRequestHandler(IPointOfInterestRepository PointOfInterestRepository, ILogger<GetPointOfInterestRequestHandler> logger) : IRequestHandler<GetPointOfInterestRequest, Result<PointOfInterestDto>>
{
    private readonly ILogger<GetPointOfInterestRequestHandler> _logger = logger;
    private readonly IPointOfInterestRepository _PointOfInterestRepository = PointOfInterestRepository;
    public async Task<Result<PointOfInterestDto>> Handle(GetPointOfInterestRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get PointOfInterest request received. {Guid}",request.Guid);
        var PointOfInterest = await _PointOfInterestRepository.GetAsync(request.Guid);
        if (PointOfInterest == null)
        {
            return Result.Fail("PointOfInterest not found");
        }
        _logger.LogInformation("PointOfInterest: {PointOfInterestname} found.", PointOfInterest.Name);
        return PointOfInterest.ToPointOfInterestDto();
    }
}