using CSharpFunctionalExtensions;
using LostAndLocate.Chats.Models;
using LostAndLocate.Chats.Repositories;
using LostAndLocate.Chats.Services;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Chats;

public class ChatServiceTest
{
    private readonly IChatRepository _repository = Substitute.For<IChatRepository>();
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly ILogger<ChatService> _logger = Substitute.For<ILogger<ChatService>>();

    private readonly ChatService _service;

    public ChatServiceTest()
    {
        _service = new ChatService(_repository, _userService, _logger);
    }

    #region SendMessageAsync

    [Fact]
    public async Task SendMessageAsync_ValidInput_ReturnsChatMessage()
    {
        // Arrange
        var sender = new User();
        var target = new User();
        const string message = "message";

        _userService.GetUserAsync(sender.Id)
            .Returns(sender);
        _userService.GetUserAsync(target.Id)
            .Returns(target);
        _repository.AddAsync(Arg.Any<ChatMessage>())
            .Returns(x => x.Arg<ChatMessage>());

        // Act
        var result = await _service.SendMessageAsync(sender.Id, target.Id, message);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(sender, result.Value.Sender);
        Assert.Equal(target, result.Value.Target);
        Assert.Equal(message, result.Value.Message);
    }

    [Fact]
    public async Task SendMessageAsync_InvalidUser_ReturnsInvalidUser()
    {
        // Arrange
        var sender = new User();
        var target = new User();
        const string message = "message";

        _userService.GetUserAsync(sender.Id)
            .Returns(sender);
        _userService.GetUserAsync(target.Id)
            .Returns(Maybe<User>.None);

        // Act
        var result = await _service.SendMessageAsync(sender.Id, target.Id, message);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ChatError.InvalidUser, result.Error);
    }

    [Fact]
    public async Task SendMessageAsync_InvalidMessage_ReturnsInvalidMessage()
    {
        // Arrange
        var sender = new User();
        var target = new User();
        const string message = "";

        _userService.GetUserAsync(sender.Id)
            .Returns(sender);
        _userService.GetUserAsync(target.Id)
            .Returns(target);

        // Act
        var result = await _service.SendMessageAsync(sender.Id, target.Id, message);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ChatError.InvalidMessage, result.Error);
    }

    [Fact]
    public async Task SendMessageAsync_SameUser_ReturnsInvalidTarget()
    {
        // Arrange
        var sender = new User();
        const string message = "";

        _userService.GetUserAsync(sender.Id)
            .Returns(sender);

        // Act
        var result = await _service.SendMessageAsync(sender.Id, sender.Id, message);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ChatError.InvalidTarget, result.Error);
    }

    #endregion

    #region GetChatsAsync

    [Fact]
    public async Task GetChatsAsync_ValidInput_ReturnsSimpleChats()
    {
        // Arrange
        var user1 = new User();
        var user2 = new User();
        var user3 = new User();

        var messages = new ChatMessage[]
        {
            new()
            {
                Sender = user1,
                Target = user2,
                Message = "message1"
            },
            new()
            {
                Sender = user3,
                Target = user1,
                Message = "message2"
            }
        };

        _repository.GetAllLastAsync(user1.Id)
            .Returns(messages);

        // Act
        var result = await _service.GetChatsAsync(user1.Id);

        // Assert
        var simpleChats = result as SimpleChat[] ?? result.ToArray();
        Assert.Equal(messages, simpleChats.Select(c => c.LastMessage));
    }

    #endregion

    #region GetChatAsync

    [Fact]
    public async Task GetChatAsync_ValidInput_ReturnsChat()
    {
        // Arrange
        var user1 = new User();
        var user2 = new User();

        var messages = new ChatMessage[]
        {
            new()
            {
                Sender = user1,
                Target = user2,
                Message = "message1"
            },
            new()
            {
                Sender = user2,
                Target = user1,
                Message = "message2"
            }
        };

        _userService.GetUserAsync(user1.Id)
            .Returns(user1);
        _userService.GetUserAsync(user2.Id)
            .Returns(user2);
        _repository.GetAllAsync(user1.Id, user2.Id)
            .Returns(messages);

        // Act
        var result = await _service.GetChatAsync(user1.Id, user2.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(messages, result.Value.Messages);
        Assert.Equal(new[] { user1, user2 }, result.Value.Users);
    }

    [Fact]
    public async Task GetChatAsync_InvalidUser_ReturnsInvalidUser()
    {
        // Arrange
        var user1 = new User();
        var user2 = new User();

        _userService.GetUserAsync(user1.Id)
            .Returns(user1);
        _userService.GetUserAsync(user2.Id)
            .Returns(Maybe<User>.None);
        _repository.GetAllAsync(user1.Id, user2.Id)
            .Returns(Array.Empty<ChatMessage>());

        // Act
        var result = await _service.GetChatAsync(user1.Id, user2.Id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ChatError.InvalidUser, result.Error);
    }

    #endregion
}