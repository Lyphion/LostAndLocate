using CSharpFunctionalExtensions;
using LostAndLocate.Data;
using LostAndLocate.Reviews.Models;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Reviews.Repositories;

/// <summary>
/// Repository definitions for <see cref="Review"/>s. 
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Receives a <see cref="Review"/> between <see cref="User"/>s from the database.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="Review"/> or None if none was not found</returns>
    Task<Maybe<Review>> GetAsync(
        Guid senderId, Guid targetId,
        CancellationToken token = default);

    /// <summary>
    /// Receives a list of <see cref="Review"/> the <see cref="User"/> created from the database.
    /// </summary>
    /// <param name="userId">The Id of the sender <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="Review"/> from the <see cref="User"/></returns>
    Task<IEnumerable<Review>> GetSenderReviewsAsync(
        Guid userId,
        CancellationToken token = default);

    /// <summary>
    /// Receives a list of <see cref="Review"/> the <see cref="User"/> received from the database.
    /// </summary>
    /// <param name="userId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="Review"/> for the <see cref="User"/></returns>
    Task<IEnumerable<Review>> GetReceiverReviewsAsync(
        Guid userId,
        CancellationToken token = default);

    /// <summary>
    /// Deletes and returns a <see cref="Review"/> between <see cref="User"/>s from the database.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="Review"/> or None if none was not found</returns>
    Task<Maybe<Review>> DeleteAsync(
        Guid senderId, Guid targetId,
        CancellationToken token = default);
}