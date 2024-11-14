using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.Users.Models;

/// <summary>
/// Request to update the user profile.
/// </summary>
public sealed class UpdateUserRequest
{
    /// <summary>
    /// The new name of the user.
    /// </summary>
    [StringLength(User.MaxNameLength, MinimumLength = User.MinNameLength)]
    public string? Name { get; init; }

    /// <summary>
    /// The new E-Mail address of the user.
    /// </summary>
    [EmailAddress]
    public string? Email { get; init; }

    /// <summary>
    /// A new description for the user.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Whether the user is an admin.
    /// </summary>
    public bool? Admin { get; set; } = false;

    /// <summary>
    /// The new password of the user.
    /// </summary>
    [RegularExpression(User.PasswordRegex,
        ErrorMessage = "Password must contain at least six characters, at least one number and both lower and uppercase letters")]
    public string? Password { get; init; }
}