using LostAndLocate.Data;
using LostAndLocate.Files.Models;
using LostAndLocate.Files.Repositories;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Files;

public class FileRepositoryTest
{
    private readonly IDbContext _context = Substitute.For<IDbContext>();

    private readonly FileRepository _repository;

    public FileRepositoryTest()
    {
        _repository = new FileRepository(_context);

        var filesSet = DummyData.Files.ToDbSet();

        _context.Set<SavedFile>()
            .Returns(filesSet);
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnFiles()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(DummyData.Files, result);
    }

    #endregion

    #region GetAsync

    [Theory]
    [MemberData(nameof(DummyData.GetFiles), MemberType = typeof(DummyData))]
    public async Task GetAsync_ValidId_ReturnFile(SavedFile file)
    {
        // Act
        var result = await _repository.GetAsync(file.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(file, result.Value);
    }

    [Fact]
    public async Task GetAsync_InvalidFile_ReturnNone()
    {
        // Arrange
        var fileId = Guid.Empty;

        // Act
        var result = await _repository.GetAsync(fileId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    [Theory]
    [MemberData(nameof(DummyData.GetFiles), MemberType = typeof(DummyData))]
    public async Task GetAsync_ValidGroupAndName_ReturnFile(SavedFile file)
    {
        // Act
        var result = await _repository.GetAsync(file.Group, file.Name);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(file, result.Value);
    }

    [Fact]
    public async Task GetAsync_InvalidGroupAndName_ReturnNone()
    {
        // Arrange
        const string group = "invalid";
        const string name = "name";

        // Act
        var result = await _repository.GetAsync(group, name);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_ValidInput_ReturnFile()
    {
        // Arrange
        var file = new SavedFile();

        // Act
        var result = await _repository.AddAsync(file);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ValidInput_ReturnFile()
    {
        // Arrange
        var file = new SavedFile();

        // Act
        var result = await _repository.UpdateAsync(file);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region DeleteAsync

    [Theory]
    [MemberData(nameof(DummyData.GetFiles), MemberType = typeof(DummyData))]
    public async Task DeleteAsync_ValidInput_ReturnFile(SavedFile file)
    {
        // Act
        var result = await _repository.DeleteAsync(file.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(file, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_InvalidFile_ReturnNone()
    {
        // Arrange
        var fileId = Guid.Empty;

        // Act
        var result = await _repository.DeleteAsync(fileId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    [Theory]
    [MemberData(nameof(DummyData.GetFiles), MemberType = typeof(DummyData))]
    public async Task DeleteAsync_ValidGroupAndName_ReturnFile(SavedFile file)
    {
        // Act
        var result = await _repository.DeleteAsync(file.Group, file.Name);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(file, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_InvalidGroupAndName_ReturnNone()
    {
        // Arrange
        const string group = "invalid";
        const string name = "name";

        // Act
        var result = await _repository.DeleteAsync(group, name);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion
}