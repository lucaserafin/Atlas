using Atlas.Api.Domain;

namespace Atlas.Api.Infrastructure;

public interface IUserRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task Add(User user);
    Task<User?> GetAsync(Guid guid);
    Task<IEnumerable<User>> GetAllAsync();
    void Update(User user);
    void Remove(User user);
}
