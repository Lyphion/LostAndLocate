using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.Chats.Models;

/// <summary>
/// Request to send chat message to target.
/// </summary>
public sealed class SendChatMessageRequest
{
    /// <summary>
    /// The Id of the user the message is for.
    /// </summary>
    [Required]
    public Guid Target { get; init; }

    /// <summary>
    /// The actual message.
    /// </summary>
    [Required]
    public string Message { get; init; } = null!;
}