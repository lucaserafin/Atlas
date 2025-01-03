﻿using Atlas.Api.Application.Dto;
using Atlas.Api.Application.Factories;
using Atlas.Api.Domain;
using Atlas.Api.Infrastructure.Contracts;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Commands.Poi;

public record CreatePointOfInterestRequest(string Name, string Description, CoordinateDto Coordinate) : IRequest<Result<PointOfInterestDto>>;

public class CreatePointOfInterestRequestHandler(IPointOfInterestRepository PointOfInterestRepository,
    ILogger<CreatePointOfInterestRequestHandler> logger) : IRequestHandler<CreatePointOfInterestRequest, Result<PointOfInterestDto>>
{
    private readonly IPointOfInterestRepository _PointOfInterestRepository = PointOfInterestRepository;
    private readonly ILogger<CreatePointOfInterestRequestHandler> _logger = logger;

    public async Task<Result<PointOfInterestDto>> Handle(CreatePointOfInterestRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating PointOfInterest");
        try
        {

        var PointOfInterest = PointOfInterestFactory.CreatePointOfInterest(request.Name, request.Description, request.Coordinate.Latitude, request.Coordinate.Longitude);
        bool PointOfInterestAlreadyExist = await _PointOfInterestRepository.PointOfInterestNameExistAsync(PointOfInterest.Name);
        if (PointOfInterestAlreadyExist)
        {
            _logger.LogWarning("PointOfInterestname already exist");
            return Result.Fail("PointOfInterestname already exist");
        }

        await _PointOfInterestRepository.AddAsync(PointOfInterest);
        await _PointOfInterestRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("PointOfInterest created");
        return PointOfInterest.ToPointOfInterestDto();

        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error creating PointOfInterest");
            return Result.Fail("Error creating PointOfInterest");
        }
    }
}