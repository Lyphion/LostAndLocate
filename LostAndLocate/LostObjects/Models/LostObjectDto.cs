using System.ComponentModel.DataAnnotations;
using LostAndLocate.Users.Models;

namespace LostAndLocate.LostObjects.Models;

/// <summary>
/// Lost object with name, user, coordinates, description and timestamp.
/// </summary>
public sealed class LostObjectDto
{
    /// <summary>
    /// Unique Id of the LostObject.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// The name of the lost object.
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
    /// The user who found the object.
    /// </summary>
    [Required]
    public UserDto User { get; init; } = null!;

    /// <summary>
    /// Timestamp when the object was registered.
    /// </summary>
    [Required]
    public DateTime Created { get; init; }

    /// <summary>
    /// A description of the object.
    /// </summary>
    [Required]
    public string Description { get; init; } = null!;

    /// <summary>
    /// Whether the object was returned to the original user.
    /// </summary>
    [Required]
    public bool Returned { get; init; } = false;
}