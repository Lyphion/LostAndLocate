using CSharpFunctionalExtensions;
using LostAndLocate.Chats.Models;
using LostAndLocate.Chats.Repositories;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;

namespace LostAndLocate.Chats.Services;

/// <summary>
/// Service implementation for chat.
/// </summary>
public sealed class ChatService : IChatService
{
    private readonly IChatRepository _repository;
    private readonly IUserService _userService;

    private readonly ILogger<ChatService> _logger;

    public ChatService(
        IChatRepository repository,
        IUserService userService,
        ILogger<ChatService> logger)
    {
        _repository = repository;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a <see cref="ChatMessage"/> between <see cref="User"/>s.
    /// </summary>
    /// <param name="senderId">Id of the sender <see cref="User"/></param>
    /// <param name="targetId">Id of the target <see cref="User"/></param>
    /// <param name="message">Message to be sent</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The created <see cref="ChatMessage"/> or <see cref="ChatError"/> if a <see cref="User"/> or the <paramref name="message"/> is invalid</returns>
    public async Task<Result<ChatMessage, ChatError>> SendMessageAsync(
        Guid senderId, Guid targetId, string message,
        CancellationToken token = default)
    {
        // Check if sender is target
        if (senderId == targetId)
            return ChatError.InvalidTarget;

        // Get users
        var senderOption = await _userService.GetUserAsync(senderId, token);
        var targetOption = await _userService.GetUserAsync(targetId, token);

        // Check if partners exist
        if (!senderOption.TryGetValue(out var sender)
            || !targetOption.TryGetValue(out var target))
            return ChatError.InvalidUser;

        // Check if message is valid
        if (message.Length == 0)
            return ChatError.InvalidMessage;

        // Create chat message
        var chatMessage = new ChatMessage
        {
            Sender = sender,
            Target = target,
            Message = message
        };

        _logger.LogInformation("Created chat message between {Sender} and {Target}",
            senderId, targetId);

        // Update database
        return await _repository.AddAsync(chatMessage, token);
    }

    /// <summary>
    /// Receives a list of <see cref="SimpleChat"/> where the <see cref="User"/> was involved.
    /// This <see cref="SimpleChat"/> contains the last <see cref="ChatMessage"/> and the involved <see cref="User"/>s.
    /// </summary>
    /// <param name="userId">Id of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of <see cref="SimpleChat"/>s</returns>
    public async Task<IEnumerable<SimpleChat>> GetChatsAsync(
        Guid userId, CancellationToken token = default)
    {
        // Get the list of last messages
        var messages = await _repository.GetAllLastAsync(userId, token);

        // Convert to SimpleChat
        return messages.Select(m => new SimpleChat
        {
            Users = new[] { m.Sender, m.Target },
            LastMessage = m
        });
    }

    /// <summary>
    /// Receives the <see cref="Chat"/> between the <see cref="User"/>s containing all <see cref="ChatMessage"/>s.
    /// The order of <paramref name="userId"/> and <paramref name="otherUserId"/> does not matter.
    /// </summary>
    /// <param name="userId">Id of first <see cref="User"/></param>
    /// <param name="otherUserId">Id of second <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The <see cref="Chat"/> between the <see cref="User"/>s</returns>
    public async Task<Result<Chat, ChatError>> GetChatAsync(
        Guid userId, Guid otherUserId,
        CancellationToken token = default)
    {
        // Get users
        var senderOption = await _userService.GetUserAsync(userId, token);
        var targetOption = await _userService.GetUserAsync(otherUserId, token);

        // Check if partners exist
        if (!senderOption.TryGetValue(out var user)
            || !targetOption.TryGetValue(out var otherUser))
            return ChatError.InvalidUser;

        // Get all messages
        var messages = await _repository.GetAllAsync(userId, otherUserId, token);

        // Convert to Chat
        return new Chat
        {
            Users = new[] { user, otherUser },
            Messages = messages.ToList()
        };
    }
}