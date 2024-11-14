using LostAndLocate.Chats.Models;
using LostAndLocate.Data;
using LostAndLocate.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.Chats.Repositories;

/// <summary>
/// Entity Framework Core Repository for <see cref="ChatMessage"/>s.
/// </summary>
public sealed class ChatRepository : EfCoreRepository<ChatMessage, IDbContext>, IChatRepository
{
    public ChatRepository(IDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Receives a list of <see cref="ChatMessage"/>s between <see cref="User"/>s from the database.
    /// The order of <paramref name="userId"/> and <paramref name="otherUserId"/> does not matter.
    /// </summary>
    /// <param name="userId">Id of the first partner <see cref="User"/></param>
    /// <param name="otherUserId">Id of the second partner <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="ChatMessage"/>s between the <see cref="User"/>s</returns>
    public async Task<IEnumerable<ChatMessage>> GetAllAsync(
        Guid userId, Guid otherUserId,
        CancellationToken token = default)
    {
        return await Context.Set<ChatMessage>()
            .Where(m => m.Sender.Id == userId && m.Target.Id == otherUserId
                        || m.Target.Id == userId && m.Sender.Id == otherUserId)
            .ToListAsync(token);
    }

    /// <summary>
    /// Receives a list the last <see cref="ChatMessage"/>s in a chat where the <see cref="User"/> was involved.
    /// </summary>
    /// <param name="userId">Id of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of last <see cref="ChatMessage"/>s</returns>
    public async Task<IEnumerable<ChatMessage>> GetAllLastAsync(
        Guid userId, CancellationToken token = default)
    {
        // Get all last messages from each partner in chat, sorted from newest to oldest
        var dbResult = await Context.Set<ChatMessage>()
            .Where(m => m.Sender.Id == userId || m.Target.Id == userId)
            .OrderByDescending(m => m.Time)
            .GroupBy(m => new { S = m.Sender.Id, T = m.Target.Id })
            .Select(g => g.OrderByDescending(m => m.Time).First())
            .ToListAsync(token);

        var result = new List<ChatMessage>();
        foreach (var message in dbResult)
        {
            var add = true;

            // Check already read messages
            foreach (var old in result)
            {
                // Check if messages have the same partners
                if (old.Sender.Id != message.Target.Id || old.Target.Id != message.Sender.Id)
                    continue;

                // Filter out older message between partners
                if (old.Time < message.Time)
                    result.Remove(old);
                else
                    add = false;
                break;
            }

            // Add message to list
            if (add)
                result.Add(message);
        }

        return result;
    }
}