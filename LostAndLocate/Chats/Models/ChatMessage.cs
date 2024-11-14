using System.ComponentModel.DataAnnotations;
using LostAndLocate.Data;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Chats.Models;

/// <summary>
/// Chat message between <see cref="User"/>s with timestamp.
/// </summary>
public sealed class ChatMessage : IEntity
{
    /// <summary>
    /// Unique Id of the Message.
    /// </summary>
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// The user who sent the message.
    /// </summary>
    [Required]
    public User Sender { get; init; } = null!;

    /// <summary>
    /// The user the message was for.
    /// </summary>
    [Required]
    public User Target { get; init; } = null!;

    /// <summary>
    /// The time the message was wrote.
    /// </summary>
    [Required]
    public DateTime Time { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// The actual message.
    /// </summary>
    [Required]
    public string Message { get; init; } = null!;
}