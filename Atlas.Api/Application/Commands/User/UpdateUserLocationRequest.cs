using Atlas.Api.Application.Dto;
using FluentResults;
using MediatR;

namespace Atlas.Api.Application.Commands.User;

public record UpdateUserLocationRequest(Guid Guid, CoordinateDto Coordinate) : IRequest<Result>;

public class UpdateUserLocationRequestHandler : IRequestHandler<UpdateUserLocationRequest, Result>
{
    public Task<Result> Handle(UpdateUserLocationRequest request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}