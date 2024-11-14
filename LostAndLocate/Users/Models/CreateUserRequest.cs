using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.Users.Models;

/// <summary>
/// Request to create a new user/register a new user.
/// </summary>
public sealed class CreateUserRequest
{
    /// <summary>
    /// Name of the user.
    /// </summary>
    [Required]
    [StringLength(User.MaxNameLength, MinimumLength = User.MinNameLength)]
    public string Name { get; init; } = null!;

    /// <summary>
    /// E-Mail address of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;

    /// <summary>
    /// A personal description of the user.
    /// </summary>
    [Required]
    public string Description { get; init; } = null!;

    /// <summary>
    /// The account password of the user.
    /// </summary>
    [Required]
    [RegularExpression(User.PasswordRegex,
        ErrorMessage = "Password must contain at least six characters, at least one number and both lower and uppercase letters")]
    public string Password { get; init; } = null!;
}