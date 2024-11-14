using LostAndLocate.Data;
using LostAndLocate.LostObjects.Models;
using LostAndLocate.LostObjects.Repositories;
using LostAndLocate.Users.Models;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.LostObjects;

public class LostObjectRepositoryTest
{
    private readonly IDbContext _context = Substitute.For<IDbContext>();

    private readonly LostObjectRepository _repository;

    public LostObjectRepositoryTest()
    {
        _repository = new LostObjectRepository(_context);

        var objectsSet = DummyData.Objects.ToDbSet();

        _context.Set<LostObject>()
            .Returns(objectsSet);
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnLostObjects()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(DummyData.Objects, result);
    }

    [Theory]
    [MemberData(nameof(DummyData.GetObjects), MemberType = typeof(DummyData))]
    public async Task GetAllAsync_WithFilter_ReturnLostObjects(LostObject obj)
    {
        // Arrange
        var filter = new LostObjectFilter
        {
            Name = obj.Name,
            User = obj.User.Name,
            MaxAmount = 1,
            Before = obj.Created,
            After = obj.Created,
            Returned = obj.Returned,
            Location = new LostObjectFilterLocation
            {
                Latitude = obj.Coordinates.Latitude,
                Longitude = obj.Coordinates.Longitude,
                Radius = 10
            }
        };

        // Act
        var result = await _repository.GetAllAsync(filter);

        // Assert
        Assert.Single(result, obj);
    }

    [Theory]
    [MemberData(nameof(DummyData.GetObjects), MemberType = typeof(DummyData))]
    public async Task GetAllAsync_WithNameFilter_ReturnLostObjects(LostObject obj)
    {
        // Arrange
        var filter = new LostObjectFilter
        {
            Name = obj.Name
        };

        // Act
        var result = await _repository.GetAllAsync(filter);

        // Assert
        Assert.All(result, o => Assert.Contains(obj.Name, o.Name));
    }

    [Theory]
    [MemberData(nameof(DummyData.GetUsers), MemberType = typeof(DummyData))]
    public async Task GetAllAsync_WithUserFilter_ReturnLostObjects(User user)
    {
        // Arrange
        var filter = new LostObjectFilter
        {
            User = user.Name
        };

        // Act
        var result = await _repository.GetAllAsync(filter);

        // Assert
        Assert.All(result, o => Assert.Contains(user.Name, o.User.Name));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAllAsync_WithReturnedFilter_ReturnLostObjects(bool returned)
    {
        // Arrange
        var filter = new LostObjectFilter
        {
            Returned = returned
        };

        // Act
        var result = await _repository.GetAllAsync(filter);

        // Assert
        Assert.All(result, o => Assert.Equal(returned, o.Returned));
    }

    [Theory]
    [MemberData(nameof(DummyData.GetObjects), MemberType = typeof(DummyData))]
    public async Task GetAllAsync_WithBeforeFilter_ReturnLostObjects(LostObject obj)
    {
        // Arrange
        var filter = new LostObjectFilter
        {
            Before = obj.Created
        };

        // Act
        var result = await _repository.GetAllAsync(filter);

        // Assert
        Assert.All(result, o => Assert.True(obj.Created >= o.Created));
    }
    
    [Theory]
    [MemberData(nameof(DummyData.GetObjects), MemberType = typeof(DummyData))]
    public async Task GetAllAsync_WithAfterFilter_ReturnLostObjects(LostObject obj)
    {
        // Arrange
        var filter = new LostObjectFilter
        {
            After = obj.Created
        };

        // Act
        var result = await _repository.GetAllAsync(filter);

        // Assert
        Assert.All(result, o => Assert.True(obj.Created <= o.Created));
    }
    
    [Theory]
    [MemberData(nameof(DummyData.GetObjects), MemberType = typeof(DummyData))]
    public async Task GetAllAsync_WithLocationFilter_ReturnLostObjects(LostObject obj)
    {
        // Arrange
        var filter = new LostObjectFilter
        {
            Location = new LostObjectFilterLocation
            {
                Latitude = obj.Coordinates.Latitude,
                Longitude = obj.Coordinates.Longitude,
                Radius = 10
            }
        };

        // Act
        var result = await _repository.GetAllAsync(filter);

        // Assert
        Assert.All(result, o => Assert.True(obj.Coordinates.Distance(o.Coordinates) <= 10));
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetAllAsync_WithLimitFilter_ReturnLostObjects(int limit)
    {
        // Arrange
        var filter = new LostObjectFilter
        {
            MaxAmount = limit
        };

        // Act
        var result = await _repository.GetAllAsync(filter);

        // Assert
        Assert.True(limit >= result.Count());
    }

    #endregion

    #region GetAsync

    [Theory]
    [MemberData(nameof(DummyData.GetObjects), MemberType = typeof(DummyData))]
    public async Task GetAsync_ValidId_ReturnLostObject(LostObject obj)
    {
        // Act
        var result = await _repository.GetAsync(obj.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(obj, result.Value);
    }

    [Fact]
    public async Task GetAsync_InvalidLostObject_ReturnNone()
    {
        // Arrange
        var objId = Guid.Empty;

        // Act
        var result = await _repository.GetAsync(objId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_ValidInput_ReturnLostObject()
    {
        // Arrange
        var obj = new LostObject();

        // Act
        var result = await _repository.AddAsync(obj);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ValidInput_ReturnLostObject()
    {
        // Arrange
        var obj = new LostObject();

        // Act
        var result = await _repository.UpdateAsync(obj);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region DeleteAsync

    [Theory]
    [MemberData(nameof(DummyData.GetObjects), MemberType = typeof(DummyData))]
    public async Task DeleteAsync_ValidInput_ReturnLostObject(LostObject obj)
    {
        // Act
        var result = await _repository.DeleteAsync(obj.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(obj, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_InvalidLostObject_ReturnNone()
    {
        // Arrange
        var objId = Guid.Empty;

        // Act
        var result = await _repository.DeleteAsync(objId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion
}