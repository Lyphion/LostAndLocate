using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.Chats.Models;

/// <summary>
/// Chat message between users with timestamp.
/// </summary>
public sealed class ChatMessageDto
{
    /// <summary>
    /// The time the message was wrote.
    /// </summary>
    [Required]
    public DateTime Time { get; init; }

    /// <summary>
    /// The Id of the user who sent the message.
    /// </summary>
    [Required]
    public Guid Sender { get; init; }

    /// <summary>
    /// The Id of the user the message was for.
    /// </summary>
    [Required]
    public Guid Target { get; init; }

    /// <summary>
    /// The actual message.
    /// </summary>
    [Required]
    public string Message { get; init; } = null!;
}