using LostAndLocate.Data;
using LostAndLocate.Reviews.Models;
using LostAndLocate.Reviews.Repositories;
using LostAndLocate.Users.Models;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Reviews;

public class ReviewRepositoryTest
{
    private readonly IDbContext _context = Substitute.For<IDbContext>();

    private readonly ReviewRepository _repository;

    public ReviewRepositoryTest()
    {
        _repository = new ReviewRepository(_context);

        var reviewsSet = DummyData.Reviews.ToDbSet();

        _context.Set<Review>()
            .Returns(reviewsSet);
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnReviews()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(DummyData.Reviews, result);
    }

    #endregion

    #region GetAsync

    [Theory]
    [MemberData(nameof(DummyData.GetReviews), MemberType = typeof(DummyData))]
    public async Task GetAsync_ValidId_ReturnReview(Review review)
    {
        // Act
        var result = await _repository.GetAsync(review.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(review, result.Value);
    }

    [Fact]
    public async Task GetAsync_InvalidReview_ReturnNone()
    {
        // Arrange
        var reviewId = Guid.Empty;

        // Act
        var result = await _repository.GetAsync(reviewId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    [Theory]
    [MemberData(nameof(DummyData.GetReviews), MemberType = typeof(DummyData))]
    public async Task GetAsync_ValidInput_ReturnReview(Review review)
    {
        // Act
        var result = await _repository.GetAsync(review.Sender.Id, review.Target.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(review, result.Value);
    }

    [Fact]
    public async Task GetAsync_InvalidGroupAndName_ReturnNone()
    {
        // Arrange
        var senderId = Guid.Empty;
        var targetId = Guid.Empty;

        // Act
        var result = await _repository.GetAsync(senderId, targetId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion

    #region GetSenderReviewsAsync

    [Theory]
    [MemberData(nameof(DummyData.GetUsers), MemberType = typeof(DummyData))]
    public async Task GetSenderReviewsAsync_ValidId_ReturnReviews(User user)
    {
        // Act
        var result = await _repository.GetSenderReviewsAsync(user.Id);

        // Assert
        Assert.All(result, r => Assert.Equal(user.Id, r.Sender.Id));
    }

    [Fact]
    public async Task GetSenderReviewsAsync_InvalidReview_ReturnEmptyList()
    {
        // Arrange
        var reviewId = Guid.Empty;

        // Act
        var result = await _repository.GetSenderReviewsAsync(reviewId);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetReceiverReviewsAsync

    [Theory]
    [MemberData(nameof(DummyData.GetUsers), MemberType = typeof(DummyData))]
    public async Task GetReceiverReviewsAsync_ValidId_ReturnReviews(User user)
    {
        // Act
        var result = await _repository.GetReceiverReviewsAsync(user.Id);

        // Assert
        Assert.All(result, r => Assert.Equal(user.Id, r.Target.Id));
    }

    [Fact]
    public async Task GetReceiverReviewsAsync_InvalidReview_ReturnEmptyList()
    {
        // Arrange
        var reviewId = Guid.Empty;

        // Act
        var result = await _repository.GetReceiverReviewsAsync(reviewId);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_ValidInput_ReturnReview()
    {
        // Arrange
        var review = new Review();

        // Act
        var result = await _repository.AddAsync(review);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ValidInput_ReturnReview()
    {
        // Arrange
        var review = new Review();

        // Act
        var result = await _repository.UpdateAsync(review);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region DeleteAsync

    [Theory]
    [MemberData(nameof(DummyData.GetReviews), MemberType = typeof(DummyData))]
    public async Task DeleteAsync_ValidId_ReturnReview(Review review)
    {
        // Act
        var result = await _repository.DeleteAsync(review.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(review, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_InvalidReview_ReturnNone()
    {
        // Arrange
        var reviewId = Guid.Empty;

        // Act
        var result = await _repository.DeleteAsync(reviewId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    [Theory]
    [MemberData(nameof(DummyData.GetReviews), MemberType = typeof(DummyData))]
    public async Task DeleteAsync_ValidInput_ReturnReview(Review review)
    {
        // Act
        var result = await _repository.DeleteAsync(review.Sender.Id, review.Target.Id);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(review, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_InvalidGroupAndName_ReturnNone()
    {
        // Arrange
        var senderId = Guid.Empty;
        var targetId = Guid.Empty;

        // Act
        var result = await _repository.DeleteAsync(senderId, targetId);

        // Assert
        Assert.True(result.HasNoValue);
    }

    #endregion
}