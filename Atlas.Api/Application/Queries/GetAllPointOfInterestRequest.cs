using Atlas.Api.Application.Dto;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Queries;

public record GetAllPointOfInterestRequest() : IRequest<Result<IEnumerable<PointOfInterestDto>>>;

public class GetAllPointOfInterestRequestHandler(IPointOfInterestRepository PointOfInterestRepository, ILogger<GetAllPointOfInterestRequestHandler> logger) : IRequestHandler<GetAllPointOfInterestRequest, Result<IEnumerable<PointOfInterestDto>>>
{
    private readonly ILogger<GetAllPointOfInterestRequestHandler> _logger = logger;
    private readonly IPointOfInterestRepository _PointOfInterestRepository = PointOfInterestRepository;
    public async Task<Result<IEnumerable<PointOfInterestDto>>> Handle(GetAllPointOfInterestRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all PointOfInterest request received.");
        var PointOfInterests = await _PointOfInterestRepository.GetAllAsync();
        if (PointOfInterests is null || !PointOfInterests.Any())
        {
            return Result.Fail("No PointOfInterests on DB");
        }
        _logger.LogInformation("PointOfInterests found: {Count}.", PointOfInterests.Count());
        return Result.Ok(PointOfInterests.Select(x => x.ToPointOfInterestDto()));
    }
}