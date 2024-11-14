using System.Net.WebSockets;
using System.Text.Json;
using LostAndLocate.Users.Models;
using LostAndLocate.Utils;

namespace LostAndLocate.Chats.Repositories;

/// <summary>
/// Handler for managing Chat WebSocket Connections.
/// </summary>
public sealed class ChatWebSocketHandler : IChatWebSocketHandler
{
    private readonly IDictionary<Guid, List<WebSocket>> _connections;
    private readonly JsonSerializerOptions _jsonOptions;

    public ChatWebSocketHandler()
    {
        _connections = new Dictionary<Guid, List<WebSocket>>();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    /// <summary>
    /// Sends a message to all connected clients.
    /// </summary>
    /// <param name="obj">The object which should be sent as Json</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <typeparam name="T">The type of the value to serialize</typeparam>
    public async Task SendAllMessageAsync<T>(T obj, CancellationToken token = default)
    {
        // Serialize object
        var data = JsonSerializer.Serialize(obj, _jsonOptions);

        // Send all users the message
        var tasks = _connections.Values
            .SelectMany(s => s)
            .Select(s => s.SendSaveAsync(data, token))
            .ToArray();

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Sends a message to all connected <see cref="User"/>s with the specific id.
    /// </summary>
    /// <param name="targetId">The id of the target <see cref="User"/></param>
    /// <param name="obj">The object which should be sent as Json</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <typeparam name="T">The type of the value to serialize</typeparam>
    /// <returns>True if a <see cref="User"/> with the id is connected</returns>
    public async Task<bool> SendMessageAsync<T>(Guid targetId, T obj, CancellationToken token = default)
    {
        // Check if a user with the id exists
        if (!_connections.TryGetValue(targetId, out var sockets))
            return false;

        // Serialize object
        var data = JsonSerializer.Serialize(obj, _jsonOptions);
        
        // Send the message to all users with the id
        var tasks = sockets
            .Select(s => s.SendSaveAsync(data, token))
            .ToArray();
        
        await Task.WhenAll(tasks);
        return true;
    }

    /// <summary>
    /// Registers a WebSocket of a <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/></param>
    /// <param name="socket">The connected Websocket of the <see cref="User"/></param>
    /// <returns>True if the WebSocket was registered successfully</returns>
    public bool Register(Guid userId, WebSocket socket)
    {
        // Check if no connection exists from the user
        if (!_connections.TryGetValue(userId, out var list))
        {
            // Save Socket
            _connections[userId] = new List<WebSocket> { socket };
            return true;
        }

        // Check if same WebSocket is already present
        if (list.Contains(socket))
            return false;

        // Save Socket
        list.Add(socket);
        return true;
    }

    /// <summary>
    /// Unregisters a WebSocket of a <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The id of the <see cref="User"/></param>
    /// <param name="socket">The connected Websocket of the <see cref="User"/></param>
    /// <returns>True if the WebSocket was unregistered successfully</returns>
    public bool Unregister(Guid userId, WebSocket socket)
    {
        // Check if no connection exists from the user
        if (!_connections.TryGetValue(userId, out var list))
            return false;

        // Remove WebSocket
        return list.Remove(socket);
    }
}