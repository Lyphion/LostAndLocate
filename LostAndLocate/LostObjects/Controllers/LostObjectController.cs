using System.Net.Mime;
using AutoMapper;
using CSharpFunctionalExtensions;
using LostAndLocate.LostObjects.Models;
using LostAndLocate.LostObjects.Services;
using LostAndLocate.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndLocate.LostObjects.Controllers;

/// <summary>
/// REST Endpoint for lost object related requests.
/// </summary>
[ApiController]
[Route("api/object")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public sealed class LostObjectController : ControllerBase
{
    private readonly ILostObjectService _service;
    private readonly IMapper _mapper;

    public LostObjectController(ILostObjectService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Receives a list of lost object matching the filter.
    /// </summary>
    /// <param name="filter">Filter options</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>List of matching objects</returns>
    /// <response code="200">Ok: List of matching objects</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LostObjectDto>>> GetObjects(
        [FromQuery] LostObjectFilter filter,
        CancellationToken token = default)
    {
        // Check if only one coordinate is given
        if (filter.Location is not null 
            && (filter.Location.Longitude is null || filter.Location.Latitude is null))
            return BadRequest(LostObjectError.InvalidLocationParameter.GetDescription());
        
        // Get all objects matching the filter
        var objects = await _service.GetObjectsAsync(filter, token);
        var target = objects.Select(_mapper.Map<LostObjectDto>);

        return Ok(target);
    }

    /// <summary>
    /// Receives a lost object from <paramref name="id"/>.
    /// </summary>
    /// <param name="id">Id of the object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Lost object with the id</returns>
    /// <response code="200">Ok: Lost object with id</response>
    /// <response code="404">NotFound: Invalid object</response>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LostObjectDto>> GetObject(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        // Get object by id
        var option = await _service.GetObjectAsync(id, token)
            .Map(_mapper.Map<LostObjectDto>);

        // Handle failure
        if (!option.TryGetValue(out var obj))
            return NotFound("Invalid object");

        return Ok(obj);
    }

    /// <summary>
    /// Create a new lost object.
    /// </summary>
    /// <param name="data">Description of the lost object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Created lost object</returns>
    /// <response code="200">Ok: Created object</response>
    /// <response code="400">BadRequest: Invalid data</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LostObjectDto>> CreateObject(
        [FromBody] CreateLostObjectRequest data,
        CancellationToken token = default)
    {
        var userId = User.GetId();

        // Create new object
        var result = await _service.CreateObjectAsync(
                data.Name, data.Coordinates,
                userId, data.Description,
                token)
            .Map(_mapper.Map<LostObjectDto>);

        // Handle error
        if (result.TryGetError(out var error, out var obj))
            return BadRequest(error.GetDescription());

        return Ok(obj);
    }

    /// <summary>
    /// Update a lost object.
    /// </summary>
    /// <param name="id">Id of the lost object</param>
    /// <param name="data">Description of the lost object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Updated lost object</returns>
    /// <response code="200">Ok: Updated object</response>
    /// <response code="400">BadRequest: Invalid data</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    /// <response code="403">Forbid: Cannot modify</response>
    /// <response code="404">NotFound: Invalid object</response>
    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LostObjectDto>> UpdateObject(
        [FromRoute] Guid id,
        [FromBody] UpdateLostObjectRequest data,
        CancellationToken token = default)
    {
        var userId = User.GetId();
        var option = await _service.GetObjectAsync(id, token);

        // Check if object exists
        if (!option.TryGetValue(out var cur))
            return NotFound("Invalid object");

        // Check if user owns object
        if (cur.User.Id != userId)
            return Forbid();

        // Update object
        var result = await _service.UpdateObjectAsync(
                id, data.Name, data.Coordinates,
                data.Description, data.Returned,
                token)
            .Map(_mapper.Map<LostObjectDto>);

        // Handle error
        if (result.TryGetError(out var error, out var obj))
            return BadRequest(error.GetDescription());

        return Ok(obj);
    }

    /// <summary>
    /// Delete a lost object.
    /// </summary>
    /// <param name="id">Id of the lost object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Deleted lost object</returns>
    /// <response code="200">Ok: Deleted object</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    /// <response code="403">Forbid: No permission</response>
    /// <response code="404">NotFound: Invalid object</response>
    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LostObjectDto>> DeleteObject(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var userId = User.GetId();
        var option = await _service.GetObjectAsync(id, token);

        // Check if object exists
        if (!option.TryGetValue(out var cur))
            return NotFound("Invalid object");

        // Check if user owns object
        if (cur.User.Id != userId)
            return Forbid();

        // Delete object
        var newOption = await _service.DeleteObjectAsync(id, token)
            .Map(_mapper.Map<LostObjectDto>);

        // Handle failure
        if (!newOption.TryGetValue(out var obj))
            return NotFound("Invalid object");

        return Ok(obj);
    }
}