using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.Data;

/// <summary>
/// Generic Entity Framework Core Repository for basic operations.
/// </summary>
/// <typeparam name="TEntity">Entity to be managed</typeparam>
/// <typeparam name="TContext">Context to be used</typeparam>
public abstract class EfCoreRepository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : class, IEntity
    where TContext : IDbContext
{
    protected readonly TContext Context;

    protected EfCoreRepository(TContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Receives a list of all entities from the database.
    /// </summary>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of all entities</returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(
        CancellationToken token = default)
    {
        return await Context.Set<TEntity>().ToListAsync(token);
    }

    /// <summary>
    /// Receives the matching entity with the provided Id from the database.
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching entity or None if Id was not found</returns>
    public async Task<Maybe<TEntity>> GetAsync(
        Guid id, CancellationToken token = default)
    {
        return await Context.Set<TEntity>()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync(token) ?? Maybe<TEntity>.None;
    }

    /// <summary>
    /// Adds the provided entity to the database
    /// </summary>
    /// <param name="entity">Entity to be added</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The added entity from the database</returns>
    public async Task<TEntity> AddAsync(
        TEntity entity, CancellationToken token = default)
    {
        var result = Context.Set<TEntity>().Add(entity);
        await Context.SaveChangesAsync(token);

        return result.Entity;
    }

    /// <summary>
    /// Updates the provided entity to the database
    /// </summary>
    /// <param name="entity">Entity to be updated</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The updated entity from the database</returns>
    public async Task<TEntity> UpdateAsync(
        TEntity entity, CancellationToken token = default)
    {
        var result = Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync(token);

        return result.Entity;
    }

    /// <summary>
    /// Deletes and returns the matching entity with the provided Id from the database.
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching entity or None if Id was not found</returns>
    public async Task<Maybe<TEntity>> DeleteAsync(
        Guid id, CancellationToken token = default)
    {
        var option = await GetAsync(id, token);

        if (!option.TryGetValue(out var entity))
            return Maybe<TEntity>.None;

        Context.Set<TEntity>().Remove(entity);
        await Context.SaveChangesAsync(token);

        return entity;
    }
}