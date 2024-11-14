using CSharpFunctionalExtensions;
using LostAndLocate.Reviews.Models;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Reviews.Services;

/// <summary>
/// Service definitions for reviews.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Receives a <see cref="Review"/> between <see cref="User"/>s.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="Review"/> or None if none was not found</returns>
    Task<Maybe<Review>> GetReviewAsync(
        Guid senderId, Guid targetId,
        CancellationToken token = default);

    /// <summary>
    /// Receives a list of <see cref="Review"/> the <see cref="User"/> created.
    /// </summary>
    /// <param name="userId">The Id of the sender <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="Review"/> from the <see cref="User"/></returns>
    Task<IEnumerable<Review>> GetSenderReviewsAsync(
        Guid userId,
        CancellationToken token = default);

    /// <summary>
    /// Receives a list of <see cref="Review"/> the <see cref="User"/> received.
    /// </summary>
    /// <param name="userId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="Review"/> for the <see cref="User"/></returns>
    Task<IEnumerable<Review>> GetReceiverReviewsAsync(
        Guid userId,
        CancellationToken token = default);

    /// <summary>
    /// Creates or updates a <see cref="Review"/> between <see cref="User"/>s.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <param name="stars">The rating of the review</param>
    /// <param name="description">A description of the review</param>
    /// <returns>The created or updated <see cref="Review"/> or <see cref="ReviewError"/> if some property is invalid</returns>
    Task<Result<Review, ReviewError>> CreateReviewAsync(
        Guid senderId, Guid targetId, byte stars, string description,
        CancellationToken token = default);

    /// <summary>
    /// Deletes and returns a <see cref="Review"/> between <see cref="User"/>s.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="Review"/> or None if none was not found</returns>
    Task<Maybe<Review>> DeleteReviewAsync(
        Guid senderId, Guid targetId,
        CancellationToken token = default);
}