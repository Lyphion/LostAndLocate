using System.Security.Claims;
using AutoMapper;
using CSharpFunctionalExtensions;
using LostAndLocate.Chats.Configuration;
using LostAndLocate.Chats.Controllers;
using LostAndLocate.Chats.Models;
using LostAndLocate.Chats.Repositories;
using LostAndLocate.Chats.Services;
using LostAndLocate.Users.Configuration;
using LostAndLocate.Users.Models;
using LostAndLocate.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Chats;

public class ChatControllerTest
{
    private readonly IChatService _service = Substitute.For<IChatService>();
    private readonly IChatWebSocketHandler _socketHandler = Substitute.For<IChatWebSocketHandler>();

    private readonly User _user;
    private readonly ChatController _controller;

    public ChatControllerTest()
    {
        var mapper = new MapperConfiguration(mc =>
        {
            mc.AddProfile<UserMappingProfile>();
            mc.AddProfile<ChatMappingProfile>();
        }).CreateMapper();

        _controller = new ChatController(_service, _socketHandler, mapper);
        _user = DummyData.Users.First();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, _user.Id.ToString()),
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var user = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    #region GetChats

    [Fact]
    public async Task GetChats_ValidUser_ReturnsSimpleChats()
    {
        // Arrange
        var chats = new SimpleChat[]
        {
            new()
            {
                Users = new[]
                {
                    DummyData.Messages[1].Sender,
                    DummyData.Messages[1].Target
                },
                LastMessage = DummyData.Messages[1]
            },
            new()
            {
                Users = new[]
                {
                    DummyData.Messages[2].Sender,
                    DummyData.Messages[2].Target
                },
                LastMessage = DummyData.Messages[2]
            }
        };

        _service.GetChatsAsync(_user.Id)
            .Returns(chats);

        // Act
        var result = await _controller.GetChats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<SimpleChatDto>>(okResult.Value).ToArray();

        Assert.Equal(chats.Length, response.Length);
        Assert.All(response, (c, i) =>
        {
            Assert.Equal(2, c.Users.Length);
            Assert.All(c.Users, (u, j) => Assert.Equal(chats[i].Users[j].Id, u.Id));

            Assert.Equal(chats[i].LastMessage.Message, c.LastMessage.Message);
            Assert.Equal(chats[i].LastMessage.Sender.Id, c.LastMessage.Sender);
            Assert.Equal(chats[i].LastMessage.Time, c.LastMessage.Time);
        });
    }

    [Fact]
    public async Task GetChats_InvalidUser_ReturnsEmptyList()
    {
        // Arrange
        _service.GetChatsAsync(_user.Id)
            .Returns(Array.Empty<SimpleChat>());

        // Act
        var result = await _controller.GetChats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<SimpleChatDto>>(okResult.Value);
        Assert.Empty(response);
    }

    #endregion

    #region GetChat

    [Fact]
    public async Task GetChat_ValidUser_ReturnsChat()
    {
        // Arrange
        var target = DummyData.Users[1];

        var messages = DummyData.Messages
            .Where(m => m.Sender == _user && m.Target == target
                        || m.Target == _user && m.Sender == target)
            .ToArray();

        var chat = new Chat
        {
            Users = new[] { _user, target },
            Messages = messages
        };

        _service.GetChatAsync(_user.Id, target.Id)
            .Returns(chat);

        // Act
        var result = await _controller.GetChat(target.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<ChatDto>(okResult.Value);

        Assert.Equal(2, response.Users.Length);
        Assert.All(response.Users, (u, i) => Assert.Equal(chat.Users[i].Id, u.Id));

        Assert.Equal(messages.Length, response.Messages.Count);
        Assert.All(response.Messages, (m, i) =>
        {
            var other = messages[i];
            Assert.Equal(other.Message, m.Message);
            Assert.Equal(other.Sender.Id, m.Sender);
            Assert.Equal(other.Time, m.Time);
        });
    }

    [Fact]
    public async Task GetChat_InvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var target = Guid.Empty;

        _service.GetChatAsync(_user.Id, target)
            .Returns(Result.Failure<Chat, ChatError>(ChatError.InvalidUser));

        // Act
        var result = await _controller.GetChat(target);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var message = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal(ChatError.InvalidUser.GetDescription(), message);
    }

    #endregion

    #region SendMessage
    
    [Fact]
    public async Task SendMessage_ValidUser_ReturnsMessage()
    {
        // Arrange
        var target = DummyData.Users[1];
        const string chatMessage = "message";

        var chatMessageRequest = new SendChatMessageRequest
        {
            Target = target.Id,
            Message = chatMessage
        };
        
        var message = new ChatMessage
        {
            Sender = _user,
            Target = target,
            Message = chatMessage
        };
        
        _service.SendMessageAsync(_user.Id, target.Id, chatMessage)
            .Returns(message);

        // Act
        var result = await _controller.SendMessage(chatMessageRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ChatMessageDto>(okResult.Value);
        
        Assert.Equal(chatMessage, response.Message);
        Assert.Equal(message.Sender.Id, response.Sender);
        Assert.Equal(message.Time, response.Time);
    }

    [Fact]
    public async Task SendMessage_InvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var target = Guid.Empty;
        const string chatMessage = "message";

        var chatMessageRequest = new SendChatMessageRequest
        {
            Target = target,
            Message = chatMessage
        };
        
        _service.SendMessageAsync(_user.Id, target, chatMessage)
            .Returns(Result.Failure<ChatMessage, ChatError>(ChatError.InvalidUser));
        
        // Act
        var result = await _controller.SendMessage(chatMessageRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var message = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal(ChatError.InvalidUser.GetDescription(), message);
    }

    #endregion
}