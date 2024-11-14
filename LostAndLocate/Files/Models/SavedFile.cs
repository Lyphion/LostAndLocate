using System.ComponentModel.DataAnnotations;
using LostAndLocate.Data;

namespace LostAndLocate.Files.Models;

/// <summary>
/// Saved file with group, name and data.
/// </summary>
public sealed class SavedFile : IEntity
{
    /// <summary>
    /// Unique Id of the File.
    /// </summary>
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Group of the file.
    /// </summary>
    [Required]
    [StringLength(16)]
    public string Group { get; init; } = null!;

    /// <summary>
    /// Name of the file.
    /// </summary>
    [Required]
    [StringLength(64)]
    public string Name { get; init; } = null!;

    /// <summary>
    /// The main data of the file.
    /// </summary>
    [Required]
    public byte[] Data { get; init; } = null!;
}