using CSharpFunctionalExtensions;
using LostAndLocate.Files.Models;
using LostAndLocate.Files.Repositories;
using LostAndLocate.Files.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Files;

public class FileDatabaseServiceTest
{
    private readonly IFileRepository _repository = Substitute.For<IFileRepository>();
    private readonly ILogger<FileDatabaseService> _logger = Substitute.For<ILogger<FileDatabaseService>>();

    private readonly FileDatabaseService _service;

    public FileDatabaseServiceTest()
    {
        _service = new FileDatabaseService(_repository, _logger);
    }

    #region GetContentAsync

    [Fact]
    public async Task GetContentAsync_ValidInput_ReturnsTrue()
    {
        // Arrange
        const string group = "group";
        const string name = "name.jpg";
        var data = new byte[64];

        var stream = new MemoryStream(64);

        Random.Shared.NextBytes(data);

        var file = new SavedFile
        {
            Group = group,
            Name = name,
            Data = data
        };

        _repository.GetAsync(group, name)
            .Returns(file);

        // Act
        var result = await _service.GetContentAsync(group, name, stream);

        // Assert
        Assert.True(result);
        Assert.Equal(data, stream.ToArray());
    }

    [Fact]
    public async Task GetContentAsync_UnknownInput_ReturnsFalse()
    {
        // Arrange
        const string group = "group";
        const string name = "name.jpg";

        var stream = new MemoryStream(64);

        _repository.GetAsync(group, name)
            .Returns(Maybe<SavedFile>.None);

        // Act
        var result = await _service.GetContentAsync(group, name, stream);

        // Assert
        Assert.False(result);
        Assert.Equal(0, stream.Length);
    }

    #endregion

    #region SaveContentAsync

    [Fact]
    public async Task SaveContentAsync_ValidInput_ReturnsTrue()
    {
        // Arrange
        const string group = "group";
        const string name = "name.jpg";

        var stream = new MemoryStream(64);

        // Act
        var result = await _service.SaveContentAsync(group, name, stream);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region DeleteContentAsync

    [Fact]
    public async Task DeleteContentAsync_ValidInput_ReturnsTrue()
    {
        // Arrange
        const string group = "group";
        const string name = "name.jpg";
        var data = new byte[64];

        Random.Shared.NextBytes(data);

        var file = new SavedFile
        {
            Group = group,
            Name = name,
            Data = data
        };

        _repository.DeleteAsync(group, name)
            .Returns(file);

        // Act
        var result = await _service.DeleteContentAsync(group, name);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteContentAsync_UnknownInput_ReturnsFalse()
    {
        // Arrange
        const string group = "group";
        const string name = "name.jpg";

        _repository.DeleteAsync(group, name)
            .Returns(Maybe<SavedFile>.None);

        // Act
        var result = await _service.DeleteContentAsync(group, name);

        // Assert
        Assert.False(result);
    }

    #endregion
}