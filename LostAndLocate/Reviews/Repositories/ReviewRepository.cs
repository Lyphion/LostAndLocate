using CSharpFunctionalExtensions;
using LostAndLocate.Data;
using LostAndLocate.Reviews.Models;
using LostAndLocate.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.Reviews.Repositories;

/// <summary>
/// Entity Framework Core Repository for <see cref="Review"/>s.
/// </summary>
public sealed class ReviewRepository : EfCoreRepository<Review, IDbContext>, IReviewRepository
{
    public ReviewRepository(IDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Receives a <see cref="Review"/> between <see cref="User"/>s from the database.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="Review"/> or None if none was not found</returns>
    public async Task<Maybe<Review>> GetAsync(
        Guid senderId, Guid targetId,
        CancellationToken token = default)
    {
        return await Context.Set<Review>()
            .Where(r => r.Sender.Id == senderId && r.Target.Id == targetId)
            .FirstOrDefaultAsync(token) ?? Maybe<Review>.None;
    }

    /// <summary>
    /// Receives a list of <see cref="Review"/> the <see cref="User"/> created from the database.
    /// </summary>
    /// <param name="userId">The Id of the sender <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="Review"/> from the <see cref="User"/></returns>
    public async Task<IEnumerable<Review>> GetSenderReviewsAsync(
        Guid userId, CancellationToken token = default)
    {
        return await Context.Set<Review>()
            .Where(r => r.Sender.Id == userId)
            .ToListAsync(token);
    }

    /// <summary>
    /// Receives a list of <see cref="Review"/> the <see cref="User"/> received from the database.
    /// </summary>
    /// <param name="userId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="Review"/> for the <see cref="User"/></returns>
    public async Task<IEnumerable<Review>> GetReceiverReviewsAsync(
        Guid userId, CancellationToken token = default)
    {
        return await Context.Set<Review>()
            .Where(r => r.Target.Id == userId)
            .ToListAsync(token);
    }

    /// <summary>
    /// Deletes and returns a <see cref="Review"/> between <see cref="User"/>s from the database.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="Review"/> or None if none was not found</returns>
    public async Task<Maybe<Review>> DeleteAsync(
        Guid senderId, Guid targetId,
        CancellationToken token = default)
    {
        var option = await GetAsync(senderId, targetId, token);

        if (!option.TryGetValue(out var entity))
            return Maybe<Review>.None;

        Context.Set<Review>().Remove(entity);
        await Context.SaveChangesAsync(token);

        return entity;
    }
}