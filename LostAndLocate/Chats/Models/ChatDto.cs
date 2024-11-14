using System.ComponentModel.DataAnnotations;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Chats.Models;

/// <summary>
/// Collection all of chat messages between users.
/// </summary>
public sealed class ChatDto
{
    /// <summary>
    /// Involved users in the chat.
    /// </summary>
    [Required]
    [Range(2, 2)]
    public UserDto[] Users { get; init; } = null!;

    /// <summary>
    /// List of all messages in the chat.
    /// </summary>
    [Required]
    public IList<ChatMessageDto> Messages { get; init; } = null!;
}