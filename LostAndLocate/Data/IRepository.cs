using CSharpFunctionalExtensions;

namespace LostAndLocate.Data;

/// <summary>
/// Generic Repository definitions for basic operations. 
/// </summary>
/// <typeparam name="TEntity">Entity to be managed</typeparam>
public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    /// <summary>
    /// Receives a list of all entities from the database.
    /// </summary>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of all entities</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default);

    /// <summary>
    /// Receives the matching entity with the provided Id from the database.
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching entity or None if Id was not found</returns>
    Task<Maybe<TEntity>> GetAsync(Guid id, CancellationToken token = default);

    /// <summary>
    /// Adds the provided entity to the database
    /// </summary>
    /// <param name="entity">Entity to be added</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The added entity from the database</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    /// Updates the provided entity to the database
    /// </summary>
    /// <param name="entity">Entity to be updated</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The updated entity from the database</returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    /// Deletes and returns the matching entity with the provided Id from the database.
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching entity or None if Id was not found</returns>
    Task<Maybe<TEntity>> DeleteAsync(Guid id, CancellationToken token = default);
}