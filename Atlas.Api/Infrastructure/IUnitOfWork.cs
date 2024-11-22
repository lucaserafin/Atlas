using Microsoft.EntityFrameworkCore.Storage;

namespace Atlas.Api.Infrastructure;

public interface IUnitOfWork
{
    public int SaveChanges();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
