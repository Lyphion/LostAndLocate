using System.ComponentModel.DataAnnotations;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Reviews.Models;

/// <summary>
/// A review between users with a rating and description.
/// </summary>
public sealed class ReviewDto
{
    /// <summary>
    /// User who wrote the review.
    /// </summary>
    [Required]
    public UserDto Sender { get; init; } = null!;

    /// <summary>
    /// The user the review is for.
    /// </summary>
    [Required]
    public UserDto Target { get; init; } = null!;

    /// <summary>
    /// The rating of the review.
    /// </summary>
    [Required]
    [Range(Review.MinStars, Review.MaxStars)]
    public byte Stars { get; init; }

    /// <summary>
    /// A description of the review.
    /// </summary>
    [Required]
    public string Description { get; init; } = null!;
}