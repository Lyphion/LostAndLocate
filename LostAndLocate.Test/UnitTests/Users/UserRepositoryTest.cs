using LostAndLocate.Data;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Repositories;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Users;

public class UserRepositoryTest
{
    private readonly IDbContext _context = Substitute.For<IDbContext>();

    private readonly UserRepository _repository;

    public UserRepositoryTest()
    {
        _repository = new UserRepository(_context);

        var usersSet = DummyData.Users.ToDbSet();

        _context.Set<User>()
            .Returns(usersSet);
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnUsers()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(DummyData.Users, result);
    }

    #endregion

    #region GetAsync

    [Theory]
    [MemberData(nameof(DummyData.GetUsers), MemberType = typeof(DummyData))]
    public async Task GetAsync_ValidId_ReturnUser(User user)
    {
        // Act
        var result = await _repository.GetAsync(user.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(user, result.Value);
    }

    [Fact]
    public async Task GetAsync_InvalidUser_ReturnNone()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act
        var result = await _repository.GetAsync(userId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region GetByNameAsync

    [Theory]
    [MemberData(nameof(DummyData.GetUsers), MemberType = typeof(DummyData))]
    public async Task GetByNameAsync_ValidName_ReturnUser(User user)
    {
        // Act
        var result = await _repository.GetByNameAsync(user.Name);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(user, result.Value);
    }

    [Fact]
    public async Task GetByNameAsync_InvalidName_ReturnNone()
    {
        // Arrange
        const string name = "RandomName";

        // Act
        var result = await _repository.GetByNameAsync(name);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region GetByEmailAsync

    [Theory]
    [MemberData(nameof(DummyData.GetUsers), MemberType = typeof(DummyData))]
    public async Task GetByEmailAsync_ValidEmail_ReturnUser(User user)
    {
        // Act
        var result = await _repository.GetByEmailAsync(user.Email);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(user, result.Value);
    }

    [Fact]
    public async Task GetByEmailAsync_InvalidEmail_ReturnNone()
    {
        // Arrange
        const string name = "random@email.de";

        // Act
        var result = await _repository.GetByEmailAsync(name);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_ValidInput_ReturnUser()
    {
        // Arrange
        var user = new User();

        // Act
        var result = await _repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ValidInput_ReturnUser()
    {
        // Arrange
        var user = new User();

        // Act
        var result = await _repository.UpdateAsync(user);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region DeleteAsync

    [Theory]
    [MemberData(nameof(DummyData.GetUsers), MemberType = typeof(DummyData))]
    public async Task DeleteAsync_ValidInput_ReturnUser(User user)
    {
        // Act
        var result = await _repository.DeleteAsync(user.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(user, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_InvalidUser_ReturnNone()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act
        var result = await _repository.DeleteAsync(userId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion
}