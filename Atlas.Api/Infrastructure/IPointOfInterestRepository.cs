using Atlas.Api.Domain;

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
}
