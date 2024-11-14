using CSharpFunctionalExtensions;
using LostAndLocate.Chats.Models;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Chats.Services;

/// <summary>
/// Service definitions for chat.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Creates a <see cref="ChatMessage"/> between <see cref="User"/>s.
    /// </summary>
    /// <param name="senderId">Id of the sender <see cref="User"/></param>
    /// <param name="targetId">Id of the target <see cref="User"/></param>
    /// <param name="message">Message to be sent</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The created <see cref="ChatMessage"/> or <see cref="ChatError"/> if a <see cref="User"/> or the <paramref name="message"/> is invalid</returns>
    Task<Result<ChatMessage, ChatError>> SendMessageAsync(
        Guid senderId, Guid targetId, string message,
        CancellationToken token = default);

    /// <summary>
    /// Receives a list of <see cref="SimpleChat"/> where the <see cref="User"/> was involved.
    /// This <see cref="SimpleChat"/> contains the last <see cref="ChatMessage"/> and the involved <see cref="User"/>s.
    /// </summary>
    /// <param name="userId">Id of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="SimpleChat"/>s</returns>
    Task<IEnumerable<SimpleChat>> GetChatsAsync(
        Guid userId,
        CancellationToken token = default);

    /// <summary>
    /// Receives the <see cref="Chat"/> between the <see cref="User"/>s containing all <see cref="ChatMessage"/>s.
    /// The order of <paramref name="userId"/> and <paramref name="otherUserId"/> does not matter.
    /// </summary>
    /// <param name="userId">Id of first <see cref="User"/></param>
    /// <param name="otherUserId">Id of second <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The <see cref="Chat"/> between the <see cref="User"/>s</returns>
    Task<Result<Chat, ChatError>> GetChatAsync(
        Guid userId, Guid otherUserId,
        CancellationToken token = default);
}