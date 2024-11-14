using System.Net.Mime;
using AutoMapper;
using CSharpFunctionalExtensions;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;
using LostAndLocate.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndLocate.Users.Controllers;

/// <summary>
/// REST Endpoint for user related requests.
/// </summary>
[ApiController]
[Route("api/user")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly IMapper _mapper;

    public UserController(IUserService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Receives a list of all users.
    /// </summary>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>List all users</returns>
    /// <response code="200">Ok: List of all users</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    /// <response code="403">Forbid: User is not an admin</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(
        CancellationToken token = default)
    {
        var userId = User.GetId();
        var option = await _service.GetUserAsync(userId, token);

        // Check if requester is admin
        if (!option.TryGetValue(out var cur) || !cur.Admin)
            return Forbid();

        // Get all users
        var users = await _service.GetUsersAsync(token);
        var target = users.Select(_mapper.Map<UserDto>);

        return Ok(target);
    }

    /// <summary>
    /// Receives a user.
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>User with the id</returns>
    /// <response code="200">Ok: User with id</response>
    /// <response code="404">NotFound: Invalid user</response>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        // Get user by id
        var option = await _service.GetUserAsync(id, token)
            .Map(_mapper.Map<UserDto>);

        // Handle failure
        if (!option.TryGetValue(out var user))
            return NotFound("Invalid user");

        return Ok(user);
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    /// <param name="data">Description of the user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Created user</returns>
    /// <response code="200">Ok: Created user</response>
    /// <response code="400">BadRequest: Invalid data</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser(
        [FromBody] CreateUserRequest data,
        CancellationToken token = default)
    {
        // Create user
        var result = await _service.CreateUserAsync(
                data.Name, data.Email,
                data.Password, data.Description,
                token)
            .Map(_mapper.Map<UserDto>);

        // Handle error
        if (result.TryGetError(out var error, out var user))
            return BadRequest(error.GetDescription());

        return Ok(user);
    }

    /// <summary>
    /// Update a user.
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="data">Description of the user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Updated user</returns>
    /// <response code="200">Ok: Updated user</response>
    /// <response code="400">BadRequest: Invalid user or data</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    /// <response code="403">Forbid: No permission</response>
    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto>> UpdateUser(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest data,
        CancellationToken token = default)
    {
        var userId = User.GetId();
        var option = await _service.GetUserAsync(userId, token);

        // Check if requester is admin or self
        if ((!option.TryGetValue(out var cur) || !cur.Admin) && id != userId)
            return Forbid();

        // Normal user can't change admin
        if (!cur.Admin)
            data.Admin = null;

        // Update user
        var result = await _service.UpdateUserAsync(
                id, data.Name, data.Email, data.Password,
                data.Description, data.Admin, token)
            .Map(_mapper.Map<UserDto>);

        // Handle error
        if (result.TryGetError(out var error, out var user))
            return BadRequest(error.GetDescription());

        return Ok(user);
    }

    /// <summary>
    /// Delete a user.
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Deleted user</returns>
    /// <response code="200">Ok: Deleted user</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    /// <response code="403">Forbid: No permission</response>
    /// <response code="404">NotFound: Invalid user</response>
    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> DeleteUser(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var userId = User.GetId();
        var curOption = await _service.GetUserAsync(userId, token);

        // Check if requester is admin or self
        if ((!curOption.TryGetValue(out var cur) || !cur.Admin) && id != userId)
            return Forbid();

        // Delete user
        var option = await _service.DeleteUserAsync(id, token)
            .Map(_mapper.Map<UserDto>);

        // Handle failure
        if (!option.TryGetValue(out var user))
            return NotFound("Invalid user");

        return Ok(user);
    }
}