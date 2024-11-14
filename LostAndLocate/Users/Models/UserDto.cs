using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.Users.Models;

/// <summary>
/// User profile for all API actions.
/// </summary>
public sealed class UserDto
{
    /// <summary>
    /// Unique Id of the User.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// The name of the user.
    /// </summary>
    [Required]
    [StringLength(User.MaxNameLength, MinimumLength = User.MinNameLength)]
    public string Name { get; init; } = null!;

    /// <summary>
    /// The E-Mail address of the user.
    /// </summary>
    [Required]
    public string Email { get; init; } = null!;

    /// <summary>
    /// The registration date of the user.
    /// </summary>
    [Required]
    public DateTime Registration { get; init; }

    /// <summary>
    /// A personal description of the user.
    /// </summary>
    [Required]
    public string Description { get; init; } = null!;

    /// <summary>
    /// Whether the user is an admin.
    /// </summary>
    public bool Admin { get; init; }
}