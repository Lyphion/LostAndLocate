using System.ComponentModel.DataAnnotations;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Chats.Models;

/// <summary>
/// Chat overview between users with last message in chat.
/// </summary>
public sealed class SimpleChatDto
{
    /// <summary>
    /// Involved users in the chat.
    /// </summary>
    [Required]
    [Range(2, 2)]
    public UserDto[] Users { get; init; } = null!;

    /// <summary>
    /// The last message in the chat.
    /// </summary>
    [Required]
    public ChatMessageDto LastMessage { get; init; } = null!;
}