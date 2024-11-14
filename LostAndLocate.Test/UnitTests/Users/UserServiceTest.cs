using CSharpFunctionalExtensions;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Repositories;
using LostAndLocate.Users.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Users;

public class UserServiceTest
{
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly ISecurityService _securityService = Substitute.For<ISecurityService>();
    private readonly ILogger<UserService> _logger = Substitute.For<ILogger<UserService>>();

    private readonly UserService _service;

    public UserServiceTest()
    {
        _service = new UserService(_repository, _securityService, _logger);
    }

    #region GetUsersAsync

    [Fact]
    public async Task GetUsersAsync_ValidInput_ReturnsUsers()
    {
        // Arrange
        var users = new User[]
        {
            new()
            {
                Name = "user1",
                Credentials = new Credentials(),
                Email = "user1@user.de",
                Description = "description1",
            },
            new()
            {
                Name = "user2",
                Credentials = new Credentials(),
                Email = "user2@user.de",
                Description = "description2",
            }
        };

        _repository.GetAllAsync()
            .Returns(users);

        // Act
        var result = await _service.GetUsersAsync();

        // Assert
        Assert.Equal(users, result);
    }

    #endregion

    #region GetUserAsync

    [Fact]
    public async Task GetUserAsync_ValidInput_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Name = "user1",
            Credentials = new Credentials(),
            Email = "user1@user.de",
            Description = "description1",
        };

        _repository.GetAsync(user.Id)
            .Returns(user);

        // Act
        var result = await _service.GetUserAsync(user.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(user, result.Value);
    }

    [Fact]
    public async Task GetUserAsync_InvalidId_ReturnsNone()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetAsync(id)
            .Returns(Maybe<User>.None);

        // Act
        var result = await _service.GetUserAsync(id);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region GetUserByNameAsync

    [Fact]
    public async Task GetUserByNameAsync_ValidInput_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Name = "user1",
            Credentials = new Credentials(),
            Email = "user1@user.de",
            Description = "description1",
        };

        _repository.GetByNameAsync(user.Name)
            .Returns(user);

        // Act
        var result = await _service.GetUserByNameAsync(user.Name);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(user, result.Value);
    }

    [Fact]
    public async Task GetUserByNameAsync_InvalidName_ReturnsNone()
    {
        // Arrange
        const string name = "test";
        _repository.GetByNameAsync(name)
            .Returns(Maybe<User>.None);

        // Act
        var result = await _service.GetUserByNameAsync(name);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region GetUserByEmailAsync

    [Fact]
    public async Task GetUserByEmailAsync_ValidInput_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Name = "user1",
            Credentials = new Credentials(),
            Email = "user1@user.de",
            Description = "description1",
        };

        _repository.GetByEmailAsync(user.Email)
            .Returns(user);

        // Act
        var result = await _service.GetUserByEmailAsync(user.Email);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(user, result.Value);
    }

    [Fact]
    public async Task GetUserByEmailAsync_InvalidEmail_ReturnsNone()
    {
        // Arrange
        const string email = "test@test.de";
        _repository.GetByEmailAsync(email)
            .Returns(Maybe<User>.None);

        // Act
        var result = await _service.GetUserByNameAsync(email);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region CreateUserAsync

    [Fact]
    public async Task CreateUserAsync_ValidInput_ReturnsUser()
    {
        // Arrange
        const string name = "test";
        const string email = "test@test.de";
        const string password = "Password123";
        const string description = "description";

        _repository.GetByNameAsync(name)
            .Returns(Maybe<User>.None);
        _repository.GetByEmailAsync(email)
            .Returns(Maybe<User>.None);
        _securityService.HashPassword(password)
            .Returns(new Credentials());
        _repository.AddAsync(Arg.Any<User>())
            .Returns(x => x.Arg<User>());

        // Act
        var result = await _service.CreateUserAsync(name, email, password, description);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(description, result.Value.Description);
    }

    [Fact]
    public async Task CreateUserAsync_InvalidName_ReturnsInvalidName()
    {
        // Arrange
        const string name = "123";
        const string email = "test@test.de";
        const string password = "Password123";
        const string description = "description";

        // Act
        var result = await _service.CreateUserAsync(name, email, password, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.InvalidName, result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_InvalidEmail_ReturnsInvalidEmail()
    {
        // Arrange
        const string name = "test";
        const string email = "test.de";
        const string password = "Password123";
        const string description = "description";

        // Act
        var result = await _service.CreateUserAsync(name, email, password, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.InvalidEmail, result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_InvalidPassword_ReturnsInvalidPassword()
    {
        // Arrange
        const string name = "test";
        const string email = "test@test.de";
        const string password = "password";
        const string description = "description";

        // Act
        var result = await _service.CreateUserAsync(name, email, password, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.InvalidPassword, result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_DuplicateName_ReturnsDuplicateName()
    {
        // Arrange
        const string name = "test";
        const string email = "test@test.de";
        const string password = "Password123";
        const string description = "description";

        _repository.GetByNameAsync(name)
            .Returns(new User());
        _repository.GetByEmailAsync(email)
            .Returns(Maybe<User>.None);
        _securityService.HashPassword(password)
            .Returns(new Credentials());

        // Act
        var result = await _service.CreateUserAsync(name, email, password, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.DuplicateName, result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_DuplicateEmail_ReturnsDuplicateEmail()
    {
        // Arrange
        const string name = "test";
        const string email = "test@test.de";
        const string password = "Password123";
        const string description = "description";

        _repository.GetByNameAsync(name)
            .Returns(Maybe<User>.None);
        _repository.GetByEmailAsync(email)
            .Returns(new User());
        _securityService.HashPassword(password)
            .Returns(new Credentials());

        // Act
        var result = await _service.CreateUserAsync(name, email, password, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.DuplicateEmail, result.Error);
    }

    #endregion

    #region UpdateUserAsync

    [Fact]
    public async Task UpdateUserAsync_ValidInput_ReturnsUser()
    {
        // Arrange
        const string name = "test";
        const string email = "test@test.de";
        const string password = "Password123";
        const string description = "description";

        var user = new User
        {
            Name = "oldName",
            Credentials = new Credentials(),
            Email = "oldEmail",
            Description = "oldDescription",
        };

        _repository.GetAsync(user.Id)
            .Returns(user);
        _repository.GetByNameAsync(name)
            .Returns(Maybe<User>.None);
        _repository.GetByEmailAsync(email)
            .Returns(Maybe<User>.None);
        _securityService.HashPassword(password)
            .Returns(new Credentials());
        _repository.UpdateAsync(Arg.Any<User>())
            .Returns(x => x.Arg<User>());

        // Act
        var result = await _service.UpdateUserAsync(user.Id, name, email, password, description, true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(description, result.Value.Description);
        Assert.True(result.Value.Admin);
    }

    [Fact]
    public async Task UpdateUserAsync_InvalidId_ReturnsInvalidId()
    {
        // Arrange
        const string name = "test";
        const string email = "test@test.de";
        const string password = "Password123";
        const string description = "description";
        var id = Guid.NewGuid();

        _repository.GetAsync(id)
            .Returns(Maybe<User>.None);
        _repository.GetByNameAsync(name)
            .Returns(Maybe<User>.None);
        _repository.GetByEmailAsync(email)
            .Returns(Maybe<User>.None);
        _securityService.HashPassword(password)
            .Returns(new Credentials());

        // Act
        var result = await _service.UpdateUserAsync(id, name, email, password, description, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.InvalidId, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_InvalidName_ReturnsInvalidName()
    {
        // Arrange
        const string name = "tes";
        const string email = "test@test.de";
        const string password = "Password123";
        const string description = "description";

        var user = new User
        {
            Name = "oldName",
            Credentials = new Credentials(),
            Email = "oldEmail",
            Description = "oldDescription",
        };

        _repository.GetAsync(user.Id)
            .Returns(user);
        _repository.GetByNameAsync(name)
            .Returns(Maybe<User>.None);
        _repository.GetByEmailAsync(email)
            .Returns(Maybe<User>.None);
        _securityService.HashPassword(password)
            .Returns(new Credentials());

        // Act
        var result = await _service.UpdateUserAsync(user.Id, name, email, password, description, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.InvalidName, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_InvalidEmail_ReturnsInvalidEmail()
    {
        // Arrange
        const string name = "test";
        const string email = "test.de";
        const string password = "Password123";
        const string description = "description";

        var user = new User
        {
            Name = "oldName",
            Credentials = new Credentials(),
            Email = "oldEmail",
            Description = "oldDescription",
        };

        _repository.GetAsync(user.Id)
            .Returns(user);
        _repository.GetByNameAsync(name)
            .Returns(Maybe<User>.None);
        _repository.GetByEmailAsync(email)
            .Returns(Maybe<User>.None);
        _securityService.HashPassword(password)
            .Returns(new Credentials());

        // Act
        var result = await _service.UpdateUserAsync(user.Id, name, email, password, description, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.InvalidEmail, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_DuplicateName_ReturnsDuplicateName()
    {
        // Arrange
        const string name = "test";
        const string email = "test@test.de";
        const string password = "Password123";
        const string description = "description";

        var user = new User
        {
            Name = "oldName",
            Credentials = new Credentials(),
            Email = "oldEmail",
            Description = "oldDescription",
        };

        _repository.GetAsync(user.Id)
            .Returns(user);
        _repository.GetByNameAsync(name)
            .Returns(new User());
        _repository.GetByEmailAsync(email)
            .Returns(Maybe<User>.None);
        _securityService.HashPassword(password)
            .Returns(new Credentials());

        // Act
        var result = await _service.UpdateUserAsync(user.Id, name, email, password, description, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.DuplicateName, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_DuplicateEmail_ReturnsDuplicateEmail()
    {
        // Arrange
        const string name = "test";
        const string email = "test@test.de";
        const string password = "Password123";
        const string description = "description";

        var user = new User
        {
            Name = "oldName",
            Credentials = new Credentials(),
            Email = "oldEmail",
            Description = "oldDescription",
        };

        _repository.GetAsync(user.Id)
            .Returns(user);
        _repository.GetByNameAsync(name)
            .Returns(Maybe<User>.None);
        _repository.GetByEmailAsync(email)
            .Returns(new User());
        _securityService.HashPassword(password)
            .Returns(new Credentials());

        // Act
        var result = await _service.UpdateUserAsync(user.Id, name, email, password, description, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.DuplicateEmail, result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_InvalidPassword_ReturnsInvalidPassword()
    {
        // Arrange
        const string name = "test";
        const string email = "test@test.de";
        const string password = "password";
        const string description = "description";

        var user = new User
        {
            Name = "oldName",
            Credentials = new Credentials(),
            Email = "oldEmail",
            Description = "oldDescription",
        };

        _repository.GetAsync(user.Id)
            .Returns(user);
        _repository.GetByNameAsync(name)
            .Returns(Maybe<User>.None);
        _repository.GetByEmailAsync(email)
            .Returns(Maybe<User>.None);
        _securityService.HashPassword(password)
            .Returns(new Credentials());

        // Act
        var result = await _service.UpdateUserAsync(user.Id, name, email, password, description, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserError.InvalidPassword, result.Error);
    }

    #endregion

    #region DeleteUserAsync

    [Fact]
    public async Task DeleteUserAsync_ValidInput_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Name = "user1",
            Credentials = new Credentials(),
            Email = "user1@user.de",
            Description = "description1",
        };

        _repository.DeleteAsync(user.Id)
            .Returns(user);

        // Act
        var result = await _service.DeleteUserAsync(user.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(user, result.Value);
    }

    [Fact]
    public async Task DeleteUserAsync_ValidId_ReturnsNone()
    {
        var id = Guid.NewGuid();

        _repository.DeleteAsync(id)
            .Returns(Maybe<User>.None);

        // Act
        var result = await _service.DeleteUserAsync(id);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion
}