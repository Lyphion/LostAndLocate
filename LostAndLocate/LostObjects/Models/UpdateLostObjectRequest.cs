using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.LostObjects.Models;

/// <summary>
/// Request to update a lost object.
/// </summary>
public sealed class UpdateLostObjectRequest
{
    /// <summary>
    /// The new name of the object.
    /// </summary>
    [StringLength(LostObject.MaxNameLength, MinimumLength = LostObject.MinNameLength)]
    public string? Name { get; init; }

    /// <summary>
    /// The new coordinates with latitude and longitude where the object was found.
    /// </summary>
    public Coordinates? Coordinates { get; init; }

    /// <summary>
    /// A new description of the object.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Whether the object was returned to the original user.
    /// </summary>
    public bool? Returned { get; init; } = false;
}