using Atlas.Api.Domain;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Infrastructure;

public interface IPointOfInterestRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task AddAsync(PointOfInterest poi);
    Task<PointOfInterest?> GetAsync(Guid guid);
    Task<IEnumerable<PointOfInterest>> GetAllAsync();
    void Update(PointOfInterest poi);
    void Remove(PointOfInterest poi);
    Task<bool> PointOfInterestNameExistAsync(string poi);
    Task<IEnumerable<PointOfInterest>> GetNearPointOfInterestAsync(Point location, double distance);
}
