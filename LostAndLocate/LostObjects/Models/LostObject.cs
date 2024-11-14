using System.ComponentModel.DataAnnotations;
using LostAndLocate.Data;
using LostAndLocate.Users.Models;

namespace LostAndLocate.LostObjects.Models;

/// <summary>
/// Lost object with name, user, coordinates, description and timestamp.
/// </summary>
public sealed class LostObject : IEntity
{
    public const byte MinNameLength = 4;
    public const byte MaxNameLength = 64;

    /// <summary>
    /// Unique Id of the LostObject.
    /// </summary>
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// The name of the lost object.
    /// </summary>
    [Required]
    [StringLength(MaxNameLength, MinimumLength = MinNameLength)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The coordinates with latitude and longitude where the object was found.
    /// </summary>
    [Required]
    public Coordinates Coordinates { get; set; } = null!;

    /// <summary>
    /// The <see cref="User"/> who found the object.
    /// </summary>
    [Required]
    public User User { get; init; } = null!;

    /// <summary>
    /// Timestamp when the object was registered.
    /// </summary>
    [Required]
    public DateTime Created { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// A description of the object.
    /// </summary>
    [Required]
    public string Description { get; set; } = null!;

    /// <summary>
    /// Whether the object was returned to the original user.
    /// </summary>
    [Required]
    public bool Returned { get; set; }
}