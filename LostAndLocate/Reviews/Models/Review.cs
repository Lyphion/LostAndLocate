using System.ComponentModel.DataAnnotations;
using LostAndLocate.Data;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Reviews.Models;

/// <summary>
/// A review between <see cref="User"/>s with a rating and description.
/// </summary>
public sealed class Review : IEntity
{
    public const byte MinStars = 1;
    public const byte MaxStars = 5;

    /// <summary>
    /// Unique Id of the Review.
    /// </summary>
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// User who wrote the review.
    /// </summary>
    [Required]
    public User Sender { get; init; } = null!;

    /// <summary>
    /// The user the review is for.
    /// </summary>
    [Required]
    public User Target { get; init; } = null!;

    /// <summary>
    /// The rating of the review.
    /// </summary>
    [Required]
    [Range(MinStars, MaxStars)]
    public byte Stars { get; set; }

    /// <summary>
    /// A description of the review.
    /// </summary>
    [Required]
    public string Description { get; set; } = null!;
}