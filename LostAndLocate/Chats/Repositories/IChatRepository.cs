using LostAndLocate.Chats.Models;
using LostAndLocate.Data;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Chats.Repositories;

/// <summary>
/// Repository definitions for <see cref="ChatMessage"/>s. 
/// </summary>
public interface IChatRepository : IRepository<ChatMessage>
{
    /// <summary>
    /// Receives a list of <see cref="ChatMessage"/>s between <see cref="User"/>s from the database.
    /// The order of <paramref name="userId"/> and <paramref name="otherUserId"/> does not matter.
    /// </summary>
    /// <param name="userId">Id of the first partner <see cref="User"/></param>
    /// <param name="otherUserId">Id of the second partner <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="ChatMessage"/>s between the <see cref="User"/>s</returns>
    Task<IEnumerable<ChatMessage>> GetAllAsync(
        Guid userId, Guid otherUserId,
        CancellationToken token = default);

    /// <summary>
    /// Receives a list the last <see cref="ChatMessage"/>s in a chat where the <see cref="User"/> was involved.
    /// </summary>
    /// <param name="userId">Id of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of last <see cref="ChatMessage"/>s</returns>
    Task<IEnumerable<ChatMessage>> GetAllLastAsync(
        Guid userId, CancellationToken token = default);
}