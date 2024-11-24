using Microsoft.EntityFrameworkCore.Storage;

namespace Atlas.Api.Infrastructure.Contracts;

public interface IUnitOfWork
{
    public int SaveChanges();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
