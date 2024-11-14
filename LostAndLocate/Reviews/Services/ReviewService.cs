using CSharpFunctionalExtensions;
using LostAndLocate.Reviews.Models;
using LostAndLocate.Reviews.Repositories;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;

namespace LostAndLocate.Reviews.Services;

/// <summary>
/// Service implementation for reviews.
/// </summary>
public sealed class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;
    private readonly IUserService _userService;

    private readonly ILogger<ReviewService> _logger;

    public ReviewService(
        IReviewRepository repository,
        IUserService userService,
        ILogger<ReviewService> logger)
    {
        _repository = repository;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Receives a <see cref="Review"/> between <see cref="User"/>s.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="Review"/> or None if none was not found</returns>
    public async Task<Maybe<Review>> GetReviewAsync(
        Guid senderId, Guid targetId,
        CancellationToken token = default)
    {
        return await _repository.GetAsync(senderId, targetId, token);
    }

    /// <summary>
    /// Receives a list of <see cref="Review"/> the <see cref="User"/> created.
    /// </summary>
    /// <param name="userId">The Id of the sender <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="Review"/> from the <see cref="User"/></returns>
    public async Task<IEnumerable<Review>> GetSenderReviewsAsync(
        Guid userId,
        CancellationToken token = default)
    {
        return await _repository.GetSenderReviewsAsync(userId, token);
    }

    /// <summary>
    /// Receives a list of <see cref="Review"/> the <see cref="User"/> received.
    /// </summary>
    /// <param name="userId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="Review"/> for the <see cref="User"/></returns>
    public async Task<IEnumerable<Review>> GetReceiverReviewsAsync(
        Guid userId,
        CancellationToken token = default)
    {
        return await _repository.GetReceiverReviewsAsync(userId, token);
    }

    /// <summary>
    /// Creates or updates a <see cref="Review"/> between <see cref="User"/>s.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <param name="stars">The rating of the review</param>
    /// <param name="description">A description of the review</param>
    /// <returns>The created or updated <see cref="Review"/> or <see cref="ReviewError"/> if some property is invalid</returns>
    public async Task<Result<Review, ReviewError>> CreateReviewAsync(
        Guid senderId, Guid targetId, byte stars, string description,
        CancellationToken token = default)
    {
        // Check if sender is target
        if (senderId == targetId)
            return ReviewError.InvalidTarget;

        // Get users
        var senderOption = await _userService.GetUserAsync(senderId, token);
        var targetOption = await _userService.GetUserAsync(targetId, token);

        // Check if partners exist
        if (!senderOption.TryGetValue(out var sender)
            || !targetOption.TryGetValue(out var target))
            return ReviewError.InvalidUser;

        // Check if stars is valid
        if (stars is < Review.MinStars or > Review.MaxStars)
            return ReviewError.InvalidRating;

        // Check if old review exists
        var oldOption = await GetReviewAsync(senderId, targetId, token);
        if (oldOption.TryGetValue(out var review))
        {
            // Override old
            review.Stars = stars;
            review.Description = description;

            _logger.LogInformation("Updated review between {Sender} and {Target}",
                senderId, targetId);

            // Update database
            return await _repository.UpdateAsync(review, token);
        }

        // Create review
        review = new Review
        {
            Sender = sender,
            Target = target,
            Stars = stars,
            Description = description
        };

        _logger.LogInformation("Created review between {Sender} and {Target}",
            senderId, targetId);

        // Update database
        return await _repository.AddAsync(review, token);
    }

    /// <summary>
    /// Deletes and returns a <see cref="Review"/> between <see cref="User"/>s.
    /// </summary>
    /// <param name="senderId">The Id of the sender <see cref="User"/></param>
    /// <param name="targetId">The Id of the target <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="Review"/> or None if none was not found</returns>
    public async Task<Maybe<Review>> DeleteReviewAsync(
        Guid senderId, Guid targetId,
        CancellationToken token = default)
    {
        // Update database
        var option = await _repository.DeleteAsync(senderId, targetId, token);

        if (option.HasValue)
            _logger.LogInformation("Deleted review between {Sender} and {Target}",
                senderId, targetId);

        return option;
    }
}