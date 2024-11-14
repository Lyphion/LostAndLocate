using CSharpFunctionalExtensions;
using LostAndLocate.Users.Controllers;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Users;

public class AuthenticationControllerTest
{
    private readonly IAuthenticationService _service = Substitute.For<IAuthenticationService>();

    private readonly AuthenticationController _controller;

    public AuthenticationControllerTest()
    {
        _controller = new AuthenticationController(_service);
    }

    #region Auth

    [Fact]
    public async Task Auth_ValidLogin_ReturnsAuthentication()
    {
        // Arrange
        const string username = "username";
        const string password = "password";

        var login = new LoginRequest
        {
            Username = username,
            Password = password
        };

        var auth = new Authentication
        {
            Type = "Bearer",
            Token = "TokenString"
        };

        _service.AuthenticateAsync(username, password)
            .Returns(auth);

        // Act
        var result = await _controller.Auth(login);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<Authentication>(okResult.Value);
        Assert.Equal(auth, response);
    }

    [Fact]
    public async Task Auth_InvalidLogin_ReturnsForbid()
    {
        // Arrange
        const string username = "username";
        const string password = "password";

        var login = new LoginRequest
        {
            Username = username,
            Password = password
        };

        _service.AuthenticateAsync(username, password)
            .Returns(Maybe<Authentication>.None);

        // Act
        var result = await _controller.Auth(login);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    #endregion
}