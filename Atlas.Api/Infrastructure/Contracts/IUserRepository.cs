using Atlas.Api.Domain;
using NetTopologySuite.Geometries;

namespace Atlas.Api.Infrastructure.Contracts;

public interface IUserRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task AddAsync(User user);
    Task<User?> GetAsync(Guid guid);
    Task<IEnumerable<User>> GetAllAsync();
    void Update(User user);
    void Remove(User user);
    Task<bool> UsernameExistAsync(string username);
    Task<IEnumerable<User>> GetAllUsersInRegion(Polygon region);
    Task<IEnumerable<User>> GetAllUsersInRadius(Point center, double distanceInKm);
}