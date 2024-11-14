using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.Users.Models;

/// <summary>
/// Authentication response after login in.
/// </summary>
public readonly struct Authentication
{
    /// <summary>
    /// Type of the authentication token.
    /// </summary>
    [Required]
    public string Type { get; init; }

    /// <summary>
    /// The authentication token for API access.
    /// </summary>
    [Required]
    public string Token { get; init; }
}