using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.Users.Models;

/// <summary>
/// Request to login in and get access to the API.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Name of the user.
    /// </summary>
    [Required]
    [StringLength(User.MaxNameLength, MinimumLength = User.MinNameLength)]
    public string Username { get; init; } = null!;

    /// <summary>
    /// Password of the user.
    /// </summary>
    [Required]
    public string Password { get; init; } = null!;
}