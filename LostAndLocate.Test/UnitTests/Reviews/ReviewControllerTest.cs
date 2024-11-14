using AutoMapper;
using LostAndLocate.Reviews.Configuration;
using LostAndLocate.Reviews.Controllers;
using LostAndLocate.Reviews.Services;
using LostAndLocate.Users.Configuration;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Reviews;

public class ReviewControllerTest
{
    private readonly IReviewService _service = Substitute.For<IReviewService>();

    private readonly ReviewController _controller;

    public ReviewControllerTest()
    {
        var mapper = new MapperConfiguration(mc =>
        {
            mc.AddProfile<UserMappingProfile>();
            mc.AddProfile<ReviewMappingProfile>();
        }).CreateMapper();

        _controller = new ReviewController(_service, mapper);
    }

    #region GetSenderReviews

    #endregion

    #region GetReceiverReviews

    #endregion

    #region CreateReview

    #endregion

    #region DeleteReview

    #endregion
}