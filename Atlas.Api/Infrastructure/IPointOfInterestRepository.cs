using Atlas.Api.Domain;

namespace Atlas.Api.Infrastructure;

public interface IPointOfInterestRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task AddAsync(PointOfInterest PointOfInterest);
    Task<PointOfInterest?> GetAsync(Guid guid);
    Task<IEnumerable<PointOfInterest>> GetAllAsync();
    void Update(PointOfInterest PointOfInterest);
    void Remove(PointOfInterest PointOfInterest);
    Task<bool> PointOfInterestNameExistAsync(string PointOfInterestname);
}
