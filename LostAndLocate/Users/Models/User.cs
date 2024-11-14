using System.ComponentModel.DataAnnotations;
using LostAndLocate.Data;
using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.Users.Models;

/// <summary>
/// User profile for all API actions.
/// </summary>
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public sealed class User : IEntity
{
    public const byte MinNameLength = 4;
    public const byte MaxNameLength = 16;

    public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{6,}$";
    public const string EmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

    /// <summary>
    /// Unique Id of the User.
    /// </summary>
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// The name of the user.
    /// </summary>
    [Required]
    [StringLength(MaxNameLength, MinimumLength = MinNameLength)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The credentials of the user stored as salt and hash.
    /// </summary>
    [Required]
    public Credentials Credentials { get; set; } = null!;

    /// <summary>
    /// The E-Mail address of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    /// <summary>
    /// The registration date of the user.
    /// </summary>
    [Required]
    public DateTime Registration { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// A personal description of the user.
    /// </summary>
    [Required]
    public string Description { get; set; } = null!;

    /// <summary>
    /// Whether the user is an admin.
    /// </summary>
    [Required]
    public bool Admin { get; set; }
}