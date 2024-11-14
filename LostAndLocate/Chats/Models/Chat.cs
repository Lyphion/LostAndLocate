using LostAndLocate.Users.Models;

namespace LostAndLocate.Chats.Models;

/// <summary>
/// Collection of all <see cref="ChatMessage"/>s between <see cref="User"/>s.
/// </summary>
public sealed class Chat
{
    /// <summary>
    /// Involved <see cref="User"/>s in the chat.
    /// </summary>
    public User[] Users { get; init; } = null!;

    /// <summary>
    /// List of all messages in the chat.
    /// </summary>
    public IList<ChatMessage> Messages { get; init; } = null!;
}