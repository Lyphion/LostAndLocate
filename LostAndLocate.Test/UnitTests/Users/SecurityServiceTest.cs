using LostAndLocate.Users.Services;
using Microsoft.Extensions.Configuration;

namespace LostAndLocate.Test.UnitTests.Users;

public class SecurityServiceTest
{
    private readonly SecurityService _service;

    public SecurityServiceTest()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Security:Iterations", "8192" },
            { "Security:SaltSize", "64" },
            { "Security:HashSize", "64" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _service = new SecurityService(configuration);
    }

    #region HashPassword

    [Fact]
    public void HashPassword_ValidInput_ReturnCredentials()
    {
        // Arrange
        const string password = "RandomPassword";

        // Act
        var credentials = _service.HashPassword(password);

        // Assert
        Assert.Equal(8192, credentials.Iterations);
        Assert.Equal(64, credentials.Salt.Length);
        Assert.Equal(64, credentials.Hash.Length);
    }

    #endregion

    #region ValidatePassword

    [Fact]
    public void ValidatePassword_ValidInput_ReturnsTrue()
    {
        // Arrange
        const string password = "RandomPassword";
        var credentials = _service.HashPassword(password);

        // Act
        var result = _service.ValidatePassword(password, credentials);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidatePassword_InvalidInput_ReturnsFalse()
    {
        // Arrange
        const string password = "RandomPassword";
        const string wrong = "RandomWrong";
        var credentials = _service.HashPassword(password);

        // Act
        var result = _service.ValidatePassword(wrong, credentials);

        // Assert
        Assert.False(result);
    }

    #endregion
}