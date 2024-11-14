using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.Reviews.Models;

/// <summary>
/// Request to create or update a review.
/// </summary>
public sealed class CreateReviewRequest
{
    /// <summary>
    /// Rating of the review.
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