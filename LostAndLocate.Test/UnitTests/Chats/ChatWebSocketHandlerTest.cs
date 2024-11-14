using System.Net.WebSockets;
using LostAndLocate.Chats.Repositories;

namespace LostAndLocate.Test.UnitTests.Chats;

public class ChatWebSocketHandlerTest
{
    private readonly ChatWebSocketHandler _handler;

    public ChatWebSocketHandlerTest()
    {
        _handler = new ChatWebSocketHandler();
    }

    #region SendAllMessageAsync

    [Fact]
    public async Task SendAllMessageAsync()
    {
        // Arrange
        _handler.Register(Guid.NewGuid(), new ClientWebSocket());

        // Act
        await _handler.SendAllMessageAsync("test");
    }

    #endregion

    #region SendMessageAsync

    [Fact]
    public async Task SendMessageAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        _handler.Register(id, new ClientWebSocket());

        // Act
        var result = await _handler.SendMessageAsync(id, "test");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SendMessageAsync_InvalidId_ReturnsFalse()
    {
        // Act
        var result = await _handler.SendMessageAsync(Guid.Empty, "test");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Register

    [Fact]
    public void Register_NewSocket_ReturnsTrue()
    {
        // Act
        var result = _handler.Register(Guid.NewGuid(), new ClientWebSocket());

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Register_ExistingIdNewSocket_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var socket = new ClientWebSocket();
        _handler.Register(id, socket);
        
        // Act
        var result = _handler.Register(id, new ClientWebSocket());

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Register_ExistingSocket_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var socket = new ClientWebSocket();
        _handler.Register(id, socket);
        
        // Act
        var result = _handler.Register(id, socket);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Unregister

    [Fact]
    public void Unregister_ValidIdAndSocket_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var socket = new ClientWebSocket();
        _handler.Register(id, socket);
        
        // Act
        var result = _handler.Unregister(id, socket);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Unregister_InvalidId_ReturnsFalse()
    {
        // Act
        var result = _handler.Unregister(Guid.NewGuid(), new ClientWebSocket());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Unregister_InvalidSocket_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var socket = new ClientWebSocket();
        _handler.Register(id, socket);
        
        // Act
        var result = _handler.Unregister(id, new ClientWebSocket());

        // Assert
        Assert.False(result);
    }

    #endregion
}