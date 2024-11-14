using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.LostObjects.Models;

/// <summary>
/// Request to create a new lost object.
/// </summary>
public sealed class CreateLostObjectRequest
{
    /// <summary>
    /// The name of the object.
    /// </summary>
    [Required]
    [StringLength(LostObject.MaxNameLength, MinimumLength = LostObject.MinNameLength)]
    public string Name { get; init; } = null!;

    /// <summary>
    /// The coordinates with latitude and longitude where the object was found.
    /// </summary>
    [Required]
    public Coordinates Coordinates { get; init; } = null!;

    /// <summary>
    /// A description of the object.
    /// </summary>
    [Required]
    public string Description { get; init; } = null!;
}