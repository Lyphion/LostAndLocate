using LostAndLocate.Chats.Models;
using LostAndLocate.Chats.Repositories;
using LostAndLocate.Data;
using LostAndLocate.Users.Models;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Chats;

public class ChatRepositoryTest
{
    private readonly IDbContext _context = Substitute.For<IDbContext>();

    private readonly ChatRepository _repository;

    public ChatRepositoryTest()
    {
        _repository = new ChatRepository(_context);

        var messagesSet = DummyData.Messages.ToDbSet();

        _context.Set<ChatMessage>()
            .Returns(messagesSet);
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnMessages()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(DummyData.Messages, result);
    }

    [Theory]
    [MemberData(nameof(DummyData.GetMessages), MemberType = typeof(DummyData))]
    public async Task GetAllAsync_ValidId_ReturnMessages(ChatMessage message)
    {
        // Act
        var result = await _repository.GetAllAsync(message.Sender.Id, message.Target.Id);

        // Assert
        Assert.All(result, m => Assert.True(
            m.Sender.Id == message.Sender.Id && m.Target.Id == message.Target.Id
            || m.Sender.Id == message.Target.Id && m.Target.Id == message.Sender.Id));
    }

    [Fact]
    public async Task GetAllAsync_InvalidId_ReturnEmptyList()
    {
        // Arrange
        var senderId = Guid.Empty;
        var targetId = Guid.Empty;

        // Act
        var result = await _repository.GetAllAsync(senderId, targetId);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetAllLastAsync

    [Theory]
    [MemberData(nameof(DummyData.GetUsers), MemberType = typeof(DummyData))]
    public async Task GetAllLastAsync_ValidId_ReturnMessages(User user)
    {
        // Act
        var result = await _repository.GetAllLastAsync(user.Id);

        // Assert
        var data = result.ToArray();

        Assert.All(data, m => Assert.True(m.Sender.Id == user.Id || m.Target.Id == user.Id));

        var values = new HashSet<Guid>();
        foreach (var message in data)
            values.Add(message.Sender.Id == user.Id ? message.Target.Id : message.Sender.Id);
        
        Assert.Equal(data.Length, values.Count);
    }

    [Fact]
    public async Task GetAllLastAsync_InvalidId_ReturnEmptyList()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act
        var result = await _repository.GetAllLastAsync(userId);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetAsync

    [Theory]
    [MemberData(nameof(DummyData.GetMessages), MemberType = typeof(DummyData))]
    public async Task GetAsync_ValidId_ReturnMessage(ChatMessage message)
    {
        // Act
        var result = await _repository.GetAsync(message.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(message, result.Value);
    }

    [Fact]
    public async Task GetAsync_InvalidMessage_ReturnNone()
    {
        // Arrange
        var messageId = Guid.Empty;

        // Act
        var result = await _repository.GetAsync(messageId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_ValidInput_ReturnMessage()
    {
        // Arrange
        var message = new ChatMessage();

        // Act
        var result = await _repository.AddAsync(message);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ValidInput_ReturnMessage()
    {
        // Arrange
        var message = new ChatMessage();

        // Act
        var result = await _repository.UpdateAsync(message);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region DeleteAsync

    [Theory]
    [MemberData(nameof(DummyData.GetMessages), MemberType = typeof(DummyData))]
    public async Task DeleteAsync_ValidInput_ReturnMessage(ChatMessage message)
    {
        // Act
        var result = await _repository.DeleteAsync(message.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(message, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_InvalidMessage_ReturnNone()
    {
        // Arrange
        var messageId = Guid.Empty;

        // Act
        var result = await _repository.DeleteAsync(messageId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion
}