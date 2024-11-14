using System.Net.Mime;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndLocate.Users.Controllers;

/// <summary>
/// REST Endpoint for authentication related requests.
/// </summary>
[ApiController]
[Route("api/auth")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _service;

    public AuthenticationController(IAuthenticationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Creates an authentication token from valid user credentials.
    /// </summary>
    /// <param name="data">Credentials of user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns><see cref="Authentication"/> token</returns>
    /// <response code="200">Ok: Valid credentials</response>
    /// <response code="403">Forbidden: Invalid credentials</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Authentication>> Auth(
        [FromBody] LoginRequest data,
        CancellationToken token = default)
    {
        // Authenticate with login userdata
        var option = await _service.AuthenticateAsync(data.Username, data.Password, token);

        // Handle failure
        if (!option.TryGetValue(out var auth))
            return Forbid();

        return Ok(auth);
    }
}