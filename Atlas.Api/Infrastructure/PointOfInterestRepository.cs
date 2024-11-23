using Atlas.Api.Domain;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Infrastructure;

public class PointOfInterestRepository(AtlasDbContext dbContext) : IPointOfInterestRepository
{
    private readonly AtlasDbContext _dbContext = dbContext;

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task AddAsync(PointOfInterest poi)
    {
        await _dbContext.AddAsync(poi);
    }

    public async Task<IEnumerable<PointOfInterest>> GetAllAsync()
    {
        return await _dbContext.PointsOfInterest.ToListAsync();
    }

    public async Task<PointOfInterest?> GetAsync(Guid guid)
    {
        return await _dbContext.PointsOfInterest.FirstOrDefaultAsync(x => x.Guid == guid);
    }

    public void Remove(PointOfInterest poi)
    {
        _dbContext.PointsOfInterest.Remove(poi);  
    }

    public void Update(PointOfInterest poi)
    {
        _dbContext.Update(poi);
    }

    public async Task<bool> PointOfInterestNameExistAsync(string name)
    {
        return await _dbContext.PointsOfInterest.AnyAsync(x => x.Name == name);
    }

    public async Task<IEnumerable<PointOfInterest>> GetNearPointOfInterestAsync(Point location, double distance)
    {
       return await _dbContext.PointsOfInterest
            .Where(x => x.Location.Distance(location) <= distance)
            .OrderBy(x => x.Location.Distance(location))
            .ToListAsync();
    }
}
