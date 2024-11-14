using AutoMapper;
using LostAndLocate.Users.Configuration;
using LostAndLocate.Users.Controllers;
using LostAndLocate.Users.Services;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Users;

public class UserControllerTest
{
    private readonly IUserService _service = Substitute.For<IUserService>();

    private readonly UserController _controller;

    public UserControllerTest()
    {
        var mapper = new MapperConfiguration(mc =>
        {
            mc.AddProfile<UserMappingProfile>();
        }).CreateMapper();

        _controller = new UserController(_service, mapper);
    }

    #region GetUsers

    #endregion

    #region GetUser

    #endregion

    #region CreateUser

    #endregion

    #region UpdateUser

    #endregion

    #region DeleteUser

    #endregion
}