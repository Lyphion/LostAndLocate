using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.LostObjects.Models;

/// <summary>
/// A filter to limit the returned list of lost objects.
/// </summary>
public sealed class LostObjectFilter
{
    /// <summary>
    /// Part of the lost object name.
    /// </summary>
    [StringLength(LostObject.MaxNameLength)]
    public string? Name { get; init; }

    /// <summary>
    /// Part of the user name who found the object.
    /// </summary>
    public string? User { get; init; }

    /// <summary>
    /// The maximum amount of received objects.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? MaxAmount { get; init; }

    /// <summary>
    /// Maximum timestamp where the object was found.
    /// </summary>
    public DateTime? Before { get; init; }

    /// <summary>
    /// Minimum timestamp where the object was found.
    /// </summary>
    public DateTime? After { get; init; }

    /// <summary>
    /// Whether the object was returned to the original user.
    /// </summary>
    public bool? Returned { get; init; }
    
    /// <summary>
    /// Central coordinate and radius to search.
    /// </summary>
    public LostObjectFilterLocation? Location { get; init; }
}