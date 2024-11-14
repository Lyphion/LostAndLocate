using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.Data;

/// <summary>
/// Database context definition for generic operations. 
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// Creates a <see cref="DbSet{TEntity}" /> that can be used to query and save instances of <typeparamref name="TEntity" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned</typeparam>
    /// <returns>A set for the given entity type</returns>
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken token = default);
}