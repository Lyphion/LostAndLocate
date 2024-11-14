using System.Net.Mime;
using AutoMapper;
using CSharpFunctionalExtensions;
using LostAndLocate.Reviews.Models;
using LostAndLocate.Reviews.Services;
using LostAndLocate.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndLocate.Reviews.Controllers;

/// <summary>
/// REST Endpoint for review related requests.
/// </summary>
[ApiController]
[Route("api/review")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public sealed class ReviewController : ControllerBase
{
    private readonly IReviewService _service;
    private readonly IMapper _mapper;

    public ReviewController(IReviewService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Receives a list of all sent reviews of user.
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>List of reviews of user</returns>
    /// <response code="200">Ok: List of reviews</response>
    [AllowAnonymous]
    [HttpGet("{id:guid}/sent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetSenderReviews(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        // Get all review from sender
        var reviews = await _service.GetSenderReviewsAsync(id, token);
        var target = reviews.Select(_mapper.Map<ReviewDto>);

        return Ok(target);
    }

    /// <summary>
    /// Receives a list of received reviews for user.
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>List of reviews for user</returns>
    /// <response code="200">Ok: List of reviews</response>
    [AllowAnonymous]
    [HttpGet("{id:guid}/received")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReceiverReviews(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        // Get all review from receiver
        var reviews = await _service.GetReceiverReviewsAsync(id, token);
        var target = reviews.Select(_mapper.Map<ReviewDto>);

        return Ok(target);
    }

    /// <summary>
    /// Create or update a review.
    /// </summary>
    /// <param name="id">Id of the user to be reviewed</param>
    /// <param name="data">Description of the review</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Created or updated review</returns>
    /// <response code="200">Ok: Created or updated review</response>
    /// <response code="400">BadRequest: Invalid user or data</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReviewDto>> CreateReview(
        [FromRoute] Guid id,
        [FromBody] CreateReviewRequest data,
        CancellationToken token = default)
    {
        var userId = User.GetId();

        // Create or update review for user
        var result = await _service.CreateReviewAsync(
                userId, id,
                data.Stars, data.Description,
                token)
            .Map(_mapper.Map<ReviewDto>);

        // Handle error
        if (result.TryGetError(out var error, out var review))
            return BadRequest(error.GetDescription());

        return Ok(review);
    }

    /// <summary>
    /// Delete a review.
    /// </summary>
    /// <param name="id">Id of the user from which to delete the review</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Deleted review</returns>
    /// <response code="200">Ok: Deleted review</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    /// <response code="404">Notfound: Invalid user</response>
    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> DeleteReview(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var userId = User.GetId();

        // Delete review for user
        var option = await _service.DeleteReviewAsync(
                userId, id,
                token)
            .Map(_mapper.Map<ReviewDto>);

        // Handle failure
        if (!option.TryGetValue(out var review))
            return NotFound("Invalid review");

        return Ok(review);
    }
}