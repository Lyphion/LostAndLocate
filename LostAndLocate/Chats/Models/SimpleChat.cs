using LostAndLocate.Users.Models;

namespace LostAndLocate.Chats.Models;

/// <summary>
/// Chat overview between <see cref="User"/>s with last message in chat.
/// </summary>
public sealed class SimpleChat
{
    /// <summary>
    /// Involved users in the chat.
    /// </summary>
    public User[] Users { get; init; } = null!;

    /// <summary>
    /// The last message in the chat.
    /// </summary>
    public ChatMessage LastMessage { get; init; } = null!;
}