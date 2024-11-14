using CSharpFunctionalExtensions;
using LostAndLocate.Reviews.Models;
using LostAndLocate.Reviews.Repositories;
using LostAndLocate.Reviews.Services;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Reviews;

public class ReviewServiceTest
{
    private readonly IReviewRepository _repository = Substitute.For<IReviewRepository>();
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly ILogger<ReviewService> _logger = Substitute.For<ILogger<ReviewService>>();

    private readonly ReviewService _service;

    public ReviewServiceTest()
    {
        _service = new ReviewService(_repository, _userService, _logger);
    }

    #region GetReviewAsync

    [Fact]
    public async Task GetReviewAsync_ValidInput_ReturnsReview()
    {
        // Arrange
        var sender = Guid.NewGuid();
        var target = Guid.NewGuid();

        var review = new Review
        {
            Sender = new User { Id = sender },
            Target = new User { Id = target },
            Stars = 4,
            Description = "description"
        };

        _repository.GetAsync(sender, target)
            .Returns(review);

        // Act
        var result = await _service.GetReviewAsync(sender, target);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(review, result.Value);
    }

    [Fact]
    public async Task GetReviewAsync_InvalidUser_ReturnsNone()
    {
        // Arrange
        var sender = Guid.NewGuid();
        var target = Guid.NewGuid();

        _repository.GetAsync(sender, target)
            .Returns(Maybe<Review>.None);

        // Act
        var result = await _service.GetReviewAsync(sender, target);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region GetSenderReviewsAsync

    [Fact]
    public async Task GetSenderReviewsAsync_ValidInput_ReturnsReviews()
    {
        // Arrange
        var sender = Guid.NewGuid();
        var target1 = Guid.NewGuid();
        var target2 = Guid.NewGuid();

        var reviews = new Review[]
        {
            new()
            {
                Sender = new User { Id = sender },
                Target = new User { Id = target1 },
                Stars = 4,
                Description = "description"
            },
            new()
            {
                Sender = new User { Id = sender },
                Target = new User { Id = target2 },
                Stars = 3,
                Description = "description2"
            }
        };

        _repository.GetSenderReviewsAsync(sender)
            .Returns(reviews);

        // Act
        var result = await _service.GetSenderReviewsAsync(sender);

        // Assert
        Assert.Equal(reviews, result);
    }

    [Fact]
    public async Task GetSenderReviewsAsync_InvalidUser_ReturnsEmptyList()
    {
        // Arrange
        var sender = Guid.NewGuid();

        _repository.GetSenderReviewsAsync(sender)
            .Returns(Array.Empty<Review>());

        // Act
        var result = await _service.GetSenderReviewsAsync(sender);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetReceiverReviewsAsync

    [Fact]
    public async Task GetReceiverReviewsAsync_ValidInput_ReturnsReviews()
    {
        // Arrange
        var sender1 = Guid.NewGuid();
        var sender2 = Guid.NewGuid();
        var target = Guid.NewGuid();

        var reviews = new Review[]
        {
            new()
            {
                Sender = new User { Id = sender1 },
                Target = new User { Id = target },
                Stars = 4,
                Description = "description"
            },
            new()
            {
                Sender = new User { Id = sender2 },
                Target = new User { Id = target },
                Stars = 3,
                Description = "description2"
            }
        };

        _repository.GetReceiverReviewsAsync(target)
            .Returns(reviews);

        // Act
        var result = await _service.GetReceiverReviewsAsync(target);

        // Assert
        Assert.Equal(reviews, result);
    }

    [Fact]
    public async Task GetReceiverReviewsAsync_InvalidUser_ReturnsEmptyList()
    {
        // Arrange
        var target = Guid.NewGuid();

        _repository.GetReceiverReviewsAsync(target)
            .Returns(Array.Empty<Review>());

        // Act
        var result = await _service.GetReceiverReviewsAsync(target);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region CreateReviewAsync

    [Fact]
    public async Task CreateReviewAsync_NewValidInput_ReturnsReview()
    {
        // Arrange
        var sender = new User();
        var target = new User();
        const byte stars = 4;
        const string description = "description";

        _userService.GetUserAsync(sender.Id)
            .Returns(sender);
        _userService.GetUserAsync(target.Id)
            .Returns(target);
        _repository.GetAsync(sender.Id, target.Id)
            .Returns(Maybe<Review>.None);
        _repository.AddAsync(Arg.Any<Review>())
            .Returns(x => x.Arg<Review>());

        // Act
        var result = await _service.CreateReviewAsync(sender.Id, target.Id, stars, description);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(sender, result.Value.Sender);
        Assert.Equal(target, result.Value.Target);
        Assert.Equal(stars, result.Value.Stars);
        Assert.Equal(description, result.Value.Description);
    }

    [Fact]
    public async Task CreateReviewAsync_UpdatedValidInput_ReturnsReview()
    {
        // Arrange
        var sender = new User();
        var target = new User();
        const byte stars = 4;
        const string description = "description";

        var old = new Review
        {
            Id = Guid.NewGuid(),
            Sender = sender,
            Target = target,
            Stars = 2,
            Description = "old Description"
        };

        _userService.GetUserAsync(sender.Id)
            .Returns(sender);
        _userService.GetUserAsync(target.Id)
            .Returns(target);
        _repository.GetAsync(sender.Id, target.Id)
            .Returns(old);
        _repository.UpdateAsync(Arg.Any<Review>())
            .Returns(x => x.Arg<Review>());

        // Act
        var result = await _service.CreateReviewAsync(sender.Id, target.Id, stars, description);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(old.Id, result.Value.Id);
        Assert.Equal(sender, result.Value.Sender);
        Assert.Equal(target, result.Value.Target);
        Assert.Equal(stars, result.Value.Stars);
        Assert.Equal(description, result.Value.Description);
    }

    [Fact]
    public async Task CreateReviewAsync_InvalidStars_ReturnsInvalidRating()
    {
        // Arrange
        var sender = new User();
        var target = new User();
        const byte stars = 6;
        const string description = "description";

        _userService.GetUserAsync(sender.Id)
            .Returns(sender);
        _userService.GetUserAsync(target.Id)
            .Returns(target);
        _repository.GetAsync(sender.Id, target.Id)
            .Returns(Maybe<Review>.None);

        // Act
        var result = await _service.CreateReviewAsync(sender.Id, target.Id, stars, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ReviewError.InvalidRating, result.Error);
    }

    [Fact]
    public async Task CreateReviewAsync_InvalidUser_ReturnsInvalidUser()
    {
        // Arrange
        var sender = new User();
        var target = new User();
        const byte stars = 6;
        const string description = "description";

        _userService.GetUserAsync(sender.Id)
            .Returns(Maybe<User>.None);
        _userService.GetUserAsync(target.Id)
            .Returns(target);
        _repository.GetAsync(sender.Id, target.Id)
            .Returns(Maybe<Review>.None);

        // Act
        var result = await _service.CreateReviewAsync(sender.Id, target.Id, stars, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ReviewError.InvalidUser, result.Error);
    }

    [Fact]
    public async Task CreateReviewAsync_SameUser_ReturnsInvalidTarget()
    {
        // Arrange
        var sender = new User();
        const byte stars = 6;
        const string description = "description";

        _userService.GetUserAsync(sender.Id)
            .Returns(sender);

        // Act
        var result = await _service.CreateReviewAsync(sender.Id, sender.Id, stars, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ReviewError.InvalidTarget, result.Error);
    }

    #endregion

    #region DeleteReviewAsync

    [Fact]
    public async Task DeleteReviewAsync_ValidInput_ReturnsReview()
    {
        // Arrange
        var sender = Guid.NewGuid();
        var target = Guid.NewGuid();

        var review = new Review
        {
            Sender = new User { Id = sender },
            Target = new User { Id = target },
            Stars = 4,
            Description = "description"
        };

        _repository.DeleteAsync(sender, target)
            .Returns(review);

        // Act
        var result = await _service.DeleteReviewAsync(sender, target);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(review, result.Value);
    }

    [Fact]
    public async Task DeleteReviewAsync_InvalidUser_ReturnsNone()
    {
        // Arrange
        var sender = Guid.NewGuid();
        var target = Guid.NewGuid();

        _repository.DeleteAsync(sender, target)
            .Returns(Maybe<Review>.None);

        // Act
        var result = await _service.DeleteReviewAsync(sender, target);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion
}