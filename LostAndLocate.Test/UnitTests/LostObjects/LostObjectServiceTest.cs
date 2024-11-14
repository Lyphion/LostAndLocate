using CSharpFunctionalExtensions;
using LostAndLocate.LostObjects.Models;
using LostAndLocate.LostObjects.Repositories;
using LostAndLocate.LostObjects.Services;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.LostObjects;

public class LostObjectServiceTest
{
    private readonly ILostObjectRepository _repository = Substitute.For<ILostObjectRepository>();
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly ILogger<LostObjectService> _logger = Substitute.For<ILogger<LostObjectService>>();

    private readonly LostObjectService _service;

    public LostObjectServiceTest()
    {
        _service = new LostObjectService(_repository, _userService, _logger);
    }

    #region GetObjectsAsync

    [Fact]
    public async Task GetObjectsAsync_ValidInput_ReturnsObjects()
    {
        // Arrange
        var objects = new LostObject[]
        {
            new()
            {
                Name = "object1",
                Coordinates = new Coordinates()
                {
                    Latitude = 10,
                    Longitude = 20
                },
                User = new User(),
                Description = "description1",
            },
            new()
            {
                Name = "object2",
                Coordinates = new Coordinates()
                {
                    Latitude = 30,
                    Longitude = -10
                },
                User = new User(),
                Description = "description2",
            }
        };

        _repository.GetAllAsync()
            .Returns(objects);

        // Act
        var result = await _service.GetObjectsAsync();

        // Assert
        Assert.Equal(objects, result);
    }

    [Fact]
    public async Task GetObjectsAsync_ValidFilterInput_ReturnsObjects()
    {
        // Arrange
        var filter = new LostObjectFilter { MaxAmount = 10 };
        var objects = new LostObject[]
        {
            new()
            {
                Name = "object1",
                Coordinates = new Coordinates()
                {
                    Latitude = 10,
                    Longitude = 20
                },
                User = new User(),
                Description = "description1",
            },
            new()
            {
                Name = "object2",
                Coordinates = new Coordinates()
                {
                    Latitude = 30,
                    Longitude = -10
                },
                User = new User(),
                Description = "description2",
            }
        };

        _repository.GetAllAsync(filter)
            .Returns(objects);

        // Act
        var result = await _service.GetObjectsAsync(filter);

        // Assert
        Assert.Equal(objects, result);
    }

    #endregion

    #region GetObjectAsync

    [Fact]
    public async Task GetObjectAsync_ValidInput_ReturnsObject()
    {
        // Arrange
        var obj = new LostObject
        {
            Name = "object",
            Coordinates = new Coordinates
            {
                Latitude = 90,
                Longitude = 0
            },
            User = new User(),
            Description = "description",
        };

        _repository.GetAsync(obj.Id)
            .Returns(obj);

        // Act
        var result = await _service.GetObjectAsync(obj.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(obj, result.Value);
    }

    [Fact]
    public async Task GetObjectAsync_InvalidId_ReturnsNone()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetAsync(id)
            .Returns(Maybe<LostObject>.None);

        // Act
        var result = await _service.GetObjectAsync(id);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region CreateObjectAsync

    [Fact]
    public async Task CreateObjectAsync_ValidInput_ReturnsObject()
    {
        // Arrange
        const string name = "test";
        const string description = "description";
        var user = new User();
        var coordinates = new Coordinates
        {
            Latitude = 10,
            Longitude = -20
        };

        _userService.GetUserAsync(user.Id)
            .Returns(user);
        _repository.AddAsync(Arg.Any<LostObject>())
            .Returns(x => x.Arg<LostObject>());

        // Act
        var result = await _service.CreateObjectAsync(name, coordinates, user.Id, description);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(description, result.Value.Description);
        Assert.Equal(coordinates, result.Value.Coordinates);
        Assert.Equal(user, result.Value.User);
    }

    [Fact]
    public async Task CreateObjectAsync_InvalidName_ReturnsInvalidName()
    {
        // Arrange
        const string name = "123";
        const string description = "description";
        var user = new User();
        var coordinates = new Coordinates
        {
            Latitude = 10,
            Longitude = -20
        };

        _userService.GetUserAsync(user.Id)
            .Returns(user);

        // Act
        var result = await _service.CreateObjectAsync(name, coordinates, user.Id, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(LostObjectError.InvalidName, result.Error);
    }

    [Fact]
    public async Task CreateObjectAsync_InvalidUser_ReturnsInvalidUser()
    {
        // Arrange
        const string name = "test";
        const string description = "description";
        var user = new User();
        var coordinates = new Coordinates
        {
            Latitude = 10,
            Longitude = -20
        };

        _userService.GetUserAsync(user.Id)
            .Returns(Maybe<User>.None);

        // Act
        var result = await _service.CreateObjectAsync(name, coordinates, user.Id, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(LostObjectError.InvalidUser, result.Error);
    }

    [Fact]
    public async Task CreateObjectAsync_InvalidCoordinates_ReturnsInvalidCoordinates()
    {
        // Arrange
        const string name = "test";
        const string description = "description";
        var user = new User();
        var coordinates = new Coordinates
        {
            Latitude = -100,
            Longitude = 190
        };

        _userService.GetUserAsync(user.Id)
            .Returns(user);

        // Act
        var result = await _service.CreateObjectAsync(name, coordinates, user.Id, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(LostObjectError.InvalidCoordinates, result.Error);
    }

    #endregion

    #region UpdateObjectAsync

    [Fact]
    public async Task UpdateObjectAsync_ValidInput_ReturnsObject()
    {
        // Arrange
        const string name = "test";
        const string description = "description";
        var user = new User();
        var coordinates = new Coordinates
        {
            Latitude = 10,
            Longitude = -20
        };

        var old = new LostObject
        {
            Name = "oldName",
            Coordinates = new Coordinates(),
            User = user,
            Description = "oldDescription",
        };

        _userService.GetUserAsync(user.Id)
            .Returns(user);
        _repository.GetAsync(old.Id)
            .Returns(old);
        _repository.UpdateAsync(Arg.Any<LostObject>())
            .Returns(x => x.Arg<LostObject>());

        // Act
        var result = await _service.UpdateObjectAsync(old.Id, name, coordinates, description, true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(old.Id, result.Value.Id);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(description, result.Value.Description);
        Assert.Equal(coordinates, result.Value.Coordinates);
        Assert.Equal(user, result.Value.User);
        Assert.True(result.Value.Returned);
    }

    [Fact]
    public async Task UpdateObjectAsync_InvalidName_ReturnsInvalidName()
    {
        // Arrange
        const string name = "123";
        const string description = "description";
        var user = new User();
        var coordinates = new Coordinates
        {
            Latitude = 10,
            Longitude = -20
        };

        var old = new LostObject
        {
            Name = "oldName",
            Coordinates = new Coordinates(),
            User = user,
            Description = "oldDescription",
        };

        _userService.GetUserAsync(user.Id)
            .Returns(user);
        _repository.GetAsync(old.Id)
            .Returns(old);

        // Act
        var result = await _service.UpdateObjectAsync(old.Id, name, coordinates, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(LostObjectError.InvalidName, result.Error);
    }

    [Fact]
    public async Task UpdateObjectAsync_InvalidId_ReturnsInvalidId()
    {
        // Arrange
        const string name = "test";
        const string description = "description";
        var user = new User();
        var coordinates = new Coordinates
        {
            Latitude = 10,
            Longitude = -20
        };

        var old = new LostObject();

        _userService.GetUserAsync(user.Id)
            .Returns(user);
        _repository.GetAsync(old.Id)
            .Returns(Maybe<LostObject>.None);

        // Act
        var result = await _service.UpdateObjectAsync(old.Id, name, coordinates, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(LostObjectError.InvalidId, result.Error);
    }

    [Fact]
    public async Task UpdateObjectAsync_InvalidCoordinates_ReturnsInvalidCoordinates()
    {
        // Arrange
        const string name = "test";
        const string description = "description";
        var user = new User();
        var coordinates = new Coordinates
        {
            Latitude = 100,
            Longitude = -320
        };

        var old = new LostObject
        {
            Name = "oldName",
            Coordinates = new Coordinates(),
            User = user,
            Description = "oldDescription",
        };

        _userService.GetUserAsync(user.Id)
            .Returns(user);
        _repository.GetAsync(old.Id)
            .Returns(old);

        // Act
        var result = await _service.UpdateObjectAsync(old.Id, name, coordinates, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(LostObjectError.InvalidCoordinates, result.Error);
    }

    #endregion

    #region DeleteObjectAsync

    [Fact]
    public async Task DeleteObjectAsync_ValidInput_ReturnsObject()
    {
        // Arrange
        var obj = new LostObject
        {
            Name = "object",
            Coordinates = new Coordinates
            {
                Latitude = 10,
                Longitude = -20
            },
            User = new User(),
            Description = "description",
        };

        _repository.DeleteAsync(obj.Id)
            .Returns(obj);

        // Act
        var result = await _service.DeleteObjectAsync(obj.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(obj, result.Value);
    }

    [Fact]
    public async Task DeleteObjectAsync_ValidId_ReturnsNone()
    {
        var id = Guid.NewGuid();

        _repository.DeleteAsync(id)
            .Returns(Maybe<LostObject>.None);

        // Act
        var result = await _service.DeleteObjectAsync(id);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion
}