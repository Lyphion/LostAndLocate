using System.Net.WebSockets;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Chats.Repositories;

/// <summary>
/// Handler definitions for managing Chat WebSocket Connections.
/// </summary>
public interface IChatWebSocketHandler
{
    /// <summary>
    /// Sends a message to all connected clients.
    /// </summary>
    /// <param name="obj">The object which should be sent as Json</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <typeparam name="T">The type of the value to serialize</typeparam>
    Task SendAllMessageAsync<T>(T obj, CancellationToken token = default);

    /// <summary>
    /// Sends a message to all connected <see cref="User"/>s with the specific id.
    /// </summary>
    /// <param name="targetId">The id of the target <see cref="User"/></param>
    /// <param name="obj">The object which should be sent as Json</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <typeparam name="T">The type of the value to serialize</typeparam>
    /// <returns>True if a <see cref="User"/> with the id is connected</returns>
    Task<bool> SendMessageAsync<T>(Guid targetId, T obj, CancellationToken token = default);

    /// <summary>
    /// Registers a WebSocket of a <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/></param>
    /// <param name="socket">The connected Websocket of the <see cref="User"/></param>
    /// <returns>True if the WebSocket was registered successfully</returns>
    bool Register(Guid userId, WebSocket socket);
    
    /// <summary>
    /// Unregisters a WebSocket of a <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/></param>
    /// <param name="socket">The connected Websocket of the <see cref="User"/></param>
    /// <returns>True if the WebSocket was unregistered successfully</returns>
    bool Unregister(Guid userId, WebSocket socket);
}