using Atlas.Api.Domain;
using Atlas.Api.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Infrastructure;

public class UserRepository(AtlasDbContext dbContext) : IUserRepository
{
    private readonly AtlasDbContext _dbContext = dbContext;

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task AddAsync(User user)
    {
        await _dbContext.AddAsync(user);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersInRadius(Point center, double distanceInKm)
    {
        return await _dbContext.Users
            .Where(x => x.Location.IsWithinDistance(center, distanceInKm))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersInRegion(Polygon region)
    {
        return await _dbContext.Users
            .Where(x => region.Intersects(x.Location))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetAsync(Guid guid)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Guid == guid);
    }

    public void Remove(User user)
    {
        _dbContext.Users.Remove(user);
    }

    public void Update(User user)
    {
        _dbContext.Update(user);
    }

    public async Task<bool> UsernameExistAsync(string username)
    {
        return await _dbContext.Users.AnyAsync(x => x.Username == username);
    }
}
