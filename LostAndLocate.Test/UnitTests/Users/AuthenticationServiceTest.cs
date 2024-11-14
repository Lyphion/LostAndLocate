using CSharpFunctionalExtensions;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Users;

public class AuthenticationServiceTest
{
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly ISecurityService _securityService = Substitute.For<ISecurityService>();
    private readonly ILogger<AuthenticationService> _logger = Substitute.For<ILogger<AuthenticationService>>();

    private readonly AuthenticationService _service;

    public AuthenticationServiceTest()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Issuer", "LostAndLocate" },
            { "Jwt:Audience", "LostAndLocate" },
            { "Jwt:Key", "ThisIsARandomKey" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _service = new AuthenticationService(
            _userService, _securityService,
            configuration, _logger);
    }

    #region AuthenticateAsync

    [Fact]
    public async Task AuthenticateAsync_ValidInput_ReturnsAuthentication()
    {
        // Arrange
        const string password = "password";
        var credentials = new Credentials();

        var user = new User
        {
            Name = "test",
            Credentials = credentials,
            Email = "test@test.de",
            Description = "description",
        };

        _userService.GetUserByNameAsync(user.Name)
            .Returns(user);
        _securityService.ValidatePassword(password, credentials)
            .Returns(true);

        // Act
        var result = await _service.AuthenticateAsync(user.Name, password);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal("Bearer", result.Value.Type);
    }

    [Fact]
    public async Task AuthenticateAsync_UnknownUser_ReturnsNone()
    {
        // Arrange
        const string password = "password";
        var credentials = new Credentials();

        var user = new User
        {
            Name = "test",
            Credentials = credentials,
            Email = "test@test.de",
            Description = "description",
        };

        _userService.GetUserByNameAsync(user.Name)
            .Returns(Maybe<User>.None);
        _securityService.ValidatePassword(password, credentials)
            .Returns(false);

        // Act
        var result = await _service.AuthenticateAsync(user.Name, password);

        // Assert
        Assert.True(result.HasNoValue);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidPassword_ReturnsNone()
    {
        // Arrange
        const string password = "password";
        var credentials = new Credentials();

        var user = new User
        {
            Name = "test",
            Credentials = credentials,
            Email = "test@test.de",
            Description = "description",
        };

        _userService.GetUserByNameAsync(user.Name)
            .Returns(user);
        _securityService.ValidatePassword(password, credentials)
            .Returns(false);

        // Act
        var result = await _service.AuthenticateAsync(user.Name, password);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region ValidateAsync

    [Fact]
    public async Task ValidateAsync_ValidInput_ReturnsId()
    {
        // Arrange
        const string password = "password";
        var credentials = new Credentials();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "test",
            Credentials = credentials,
            Email = "test@test.de",
            Description = "description",
        };

        _userService.GetUserByNameAsync(user.Name)
            .Returns(user);
        _userService.GetUserAsync(user.Id)
            .Returns(user);
        _securityService.ValidatePassword(password, credentials)
            .Returns(true);

        var auth = await _service.AuthenticateAsync(user.Name, password);

        // Act
        var result = await _service.ValidateAsync(auth.Value);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(user.Id, result.Value);
    }

    [Fact]
    public async Task ValidateAsync_InvalidTokenType_ReturnsNone()
    {
        // Arrange
        var auth = new Authentication
        {
            Type = "test",
            Token = "Unknown"
        };

        // Act
        var result = await _service.ValidateAsync(auth);

        // Assert
        Assert.True(result.HasNoValue);
    }

    [Fact]
    public async Task ValidateAsync_InvalidToken_ReturnsNone()
    {
        // Arrange
        var auth = new Authentication
        {
            Type = "Bearer",
            Token = "Unknown"
        };

        // Act
        var result = await _service.ValidateAsync(auth);

        // Assert
        Assert.True(result.HasNoValue);
    }

    [Fact]
    public async Task ValidateAsync_InvalidId_ReturnsNone()
    {
        // Arrange
        const string password = "password";
        var credentials = new Credentials();
        var id = Guid.NewGuid();

        var user = new User
        {
            Id = id,
            Name = "test",
            Credentials = credentials,
            Email = "test@test.de",
            Description = "description",
        };

        _userService.GetUserByNameAsync(user.Name)
            .Returns(user);
        _userService.GetUserAsync(user.Id)
            .Returns(Maybe<User>.None);
        _securityService.ValidatePassword(password, credentials)
            .Returns(true);

        var auth = await _service.AuthenticateAsync(user.Name, password);

        // Act
        var result = await _service.ValidateAsync(auth.Value);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion
}