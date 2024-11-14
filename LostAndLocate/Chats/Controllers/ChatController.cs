using System.Net.Mime;
using System.Net.WebSockets;
using System.Text.Json;
using AutoMapper;
using CSharpFunctionalExtensions;
using LostAndLocate.Chats.Models;
using LostAndLocate.Chats.Repositories;
using LostAndLocate.Chats.Services;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;
using LostAndLocate.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndLocate.Chats.Controllers;

/// <summary>
/// REST Endpoint for chat related requests.
/// </summary>
[ApiController]
[Route("api/chat")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public sealed class ChatController : ControllerBase
{
    private readonly IChatService _service;
    private readonly IChatWebSocketHandler _socketHandler;
    private readonly IMapper _mapper;

    public ChatController(
        IChatService service,
        IChatWebSocketHandler socketHandler,
        IMapper mapper)
    {
        _service = service;
        _socketHandler = socketHandler;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates and opens a WebSocket connection for Chat messages.
    /// </summary>
    /// <param name="authService">Service managing authentication</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <response code="200">Ok: Connection established</response>
    /// <response code="400">BadRequest: Not a websocket request</response>
    [AllowAnonymous]
    [HttpGet("websocket")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConnectWebSocket(
        [FromServices] IAuthenticationService authService,
        CancellationToken token = default)
    {
        // Check if connection is WebSocket connection
        if (!HttpContext.WebSockets.IsWebSocketRequest)
            return BadRequest("Not a Websocket request");

        // Serialization options
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        // Format error preset
        var formatError = JsonSerializer.Serialize(new { Error = "Invalid format" }, options);

        // Accepting socket connection
        using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        // Receive authentication token
        var jwt = await socket.ReceiveStringAsync(token);
        var auth = JsonSerializer.Deserialize<Authentication>(jwt, options);

        // Check if token is valid
        var option = await authService.ValidateAsync(auth, token);
        if (!option.TryGetValue(out var id))
            return BadRequest("Invalid token");

        // Register client
        _socketHandler.Register(id, socket);

        // Keep socket alive and receive chat messages
        while (!token.IsCancellationRequested && socket.State == WebSocketState.Open)
        {
            try
            {
                // Read message
                var data = await socket.ReceiveStringAsync(token);
                if (data.Length == 0)
                {
                    await socket.SendAsync(formatError, token);
                    continue;
                }

                // Parse message
                var message = JsonSerializer.Deserialize<SendChatMessageRequest>(data, options);
                if (message?.Message is null)
                {
                    await socket.SendAsync(formatError, token);
                    continue;
                }

                // Send message to chat partner
                var result = await _service.SendMessageAsync(id, message.Target, message.Message, token)
                    .Map(_mapper.Map<ChatMessageDto>);

                // Handle errors
                if (result.TryGetError(out var error, out var chat))
                {
                    // Send message back
                    var obj = new { Error = error.GetDescription() };
                    await _socketHandler.SendMessageAsync(id, obj, token);
                    continue;
                }

                await Task.WhenAll(
                    // Send to partner
                    _socketHandler.SendMessageAsync(message.Target, chat, token),
                    // Send message back
                    _socketHandler.SendMessageAsync(id, chat, token)
                );
            }
            catch (JsonException)
            {
                await socket.SendAsync(formatError, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                await socket.SendAsync("Internal error", token);
            }
        }

        // Unregister and close connection
        _socketHandler.Unregister(id, socket);
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", token);

        return Ok();
    }

    /// <summary>
    /// Returns list of all chats with others.
    /// </summary>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A List of chats</returns>
    /// <response code="200">Ok: All chats of user</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<SimpleChatDto>>> GetChats(
        CancellationToken token = default)
    {
        var userId = User.GetId();

        // Get chats of user
        var chats = await _service.GetChatsAsync(userId, token);
        var target = chats.Select(_mapper.Map<SimpleChatDto>);

        return Ok(target);
    }

    /// <summary>
    /// Returns the chat with partner user.
    /// </summary>
    /// <param name="id">Id of partner user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The Chat with partner</returns>
    /// <response code="200">Ok: Chat with partner user</response>
    /// <response code="400">BadRequest: Invalid user</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ChatDto>> GetChat(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var userId = User.GetId();

        // Get chat between users
        var result = await _service.GetChatAsync(userId, id, token)
            .Map(_mapper.Map<ChatDto>);

        // Handle error
        if (result.TryGetError(out var error, out var chat))
            return BadRequest(error.GetDescription());

        return Ok(chat);
    }

    /// <summary>
    /// Send chat message to another user.
    /// </summary>
    /// <param name="data">Message which should be send</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The created chat message</returns>
    /// <response code="200">Ok: Chat message for partner user</response>
    /// <response code="400">BadRequest: Invalid user</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ChatMessageDto>> SendMessage(
        [FromBody] SendChatMessageRequest data,
        CancellationToken token = default)
    {
        var userId = User.GetId();

        // Send message to chat partner
        var result = await _service.SendMessageAsync(
                userId, data.Target, data.Message, token)
            .Map(_mapper.Map<ChatMessageDto>);

        // Handle error
        if (result.TryGetError(out var error, out var chat))
            return BadRequest(error.GetDescription());

        await Task.WhenAll(
            // Send to partner
            _socketHandler.SendMessageAsync(data.Target, chat, token),
            // Send message back
            _socketHandler.SendMessageAsync(userId, chat, token)
        );

        return Ok(chat);
    }
}