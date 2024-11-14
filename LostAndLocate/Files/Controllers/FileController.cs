using System.Net.Mime;
using LostAndLocate.Files.Services;
using LostAndLocate.LostObjects.Services;
using LostAndLocate.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace LostAndLocate.Files.Controllers;

/// <summary>
/// REST Endpoint for file/content related requests.
/// </summary>
[ApiController]
[Route("content")]
public sealed class FileController : ControllerBase
{
    private static readonly JpegEncoder JpegEncoder = new();

    private readonly IFileService _service;

    public FileController(IFileService service)
    {
        _service = service;
    }

    /// <summary>
    /// Receives the picture of user.
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Picture for user</returns>
    /// <response code="200">Ok: Picture found</response>
    /// <response code="404">NotFound: Unknown id or picture not found</response>
    [AllowAnonymous]
    [HttpGet("user/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserPicture(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        // Get image from Blob Storage
        var outputStream = new MemoryStream();
        var success = await _service.GetContentAsync("user", $"{id}.jpg",
            outputStream, token);

        // Check if image was received
        if (!success)
        {
            await outputStream.DisposeAsync();
            return NotFound("Image not found");
        }

        // Send image
        outputStream.Seek(0, SeekOrigin.Begin);
        return File(outputStream, MediaTypeNames.Image.Jpeg, $"{id}.jpg");
    }

    /// <summary>
    /// Receives the picture of the lost object.
    /// </summary>
    /// <param name="id">Id of the object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Picture of lost object</returns>
    /// <response code="200">Ok: Picture found</response>
    /// <response code="404">NotFound: Unknown id</response>
    [AllowAnonymous]
    [HttpGet("object/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetObjectPicture(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        // Get image from Blob Storage
        var outputStream = new MemoryStream();
        var success = await _service.GetContentAsync("object", $"{id}.jpg",
            outputStream, token);

        // Check if image was received
        if (!success)
        {
            await outputStream.DisposeAsync();
            return NotFound("Image not found");
        }

        // Send image
        outputStream.Seek(0, SeekOrigin.Begin);
        return File(outputStream, MediaTypeNames.Image.Jpeg, $"{id}.jpg");
    }

    /// <summary>
    /// Uploads a picture for a user.
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="file">Picture of the user</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Success of upload</returns>
    /// <response code="200">Ok: Picture uploaded</response>
    /// <response code="400">BadRequest: Invalid or unsupported picture</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    /// <response code="403">Forbid: No permission</response>
    [Authorize]
    [HttpPost("user/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadUserPicture(
        [FromRoute] Guid id, IFormFile file,
        CancellationToken token = default)
    {
        var userId = User.GetId();

        // Check if user is self
        if (id != userId)
            return Forbid();

        // Converting and resizing image
        await using var inputStream = file.OpenReadStream();
        using var tempStream = new MemoryStream();

        var success = await ConvertImageAsync(
            inputStream, tempStream,
            new Size(512), token);

        // Handle failure
        if (!success)
            return BadRequest("Invalid image");

        // Save image to Blob storage
        tempStream.Seek(0, SeekOrigin.Begin);
        var res = await _service.SaveContentAsync("user", $"{id}.jpg",
            tempStream, token);

        return res ? Ok() : Problem();
    }

    /// <summary>
    /// Uploads a picture for a lost object.
    /// </summary>
    /// <param name="id">Id of the object</param>
    /// <param name="file">Picture of the object</param>
    /// <param name="objectService">Service managing lost objects</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Success of upload</returns>
    /// <response code="200">Ok: Picture uploaded</response>
    /// <response code="400">BadRequest: Invalid or unsupported picture</response>
    /// <response code="401">Unauthorized: Not authorized</response>
    /// <response code="403">Forbid: No permission</response>
    [Authorize]
    [HttpPost("object/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UploadObjectPicture(
        [FromRoute] Guid id, IFormFile file,
        [FromServices] ILostObjectService objectService,
        CancellationToken token = default)
    {
        var userId = User.GetId();

        // Check if user owns object
        var option = await objectService.GetObjectAsync(id, token);
        if (!option.TryGetValue(out var obj) || obj.User.Id != userId)
            return Forbid();

        // Converting and resizing image
        await using var inputStream = file.OpenReadStream();
        using var tempStream = new MemoryStream();

        var success = await ConvertImageAsync(
            inputStream, tempStream,
            new Size(512), token);

        // Handle failure
        if (!success)
            return BadRequest("Invalid image");

        // Save image to Blob storage
        tempStream.Seek(0, SeekOrigin.Begin);
        var res = await _service.SaveContentAsync("object", $"{id}.jpg",
            tempStream, token);

        return res ? Ok() : Problem();
    }

    /// <summary>
    /// Converts to JPG and scales down the image.
    /// </summary>
    /// <param name="inputStream">Stream with the original image</param>
    /// <param name="outputStream">Stream of the converted images</param>
    /// <param name="maxSize">Maximum size of the converted image</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>True if the conversion was successful</returns>
    private static async Task<bool> ConvertImageAsync(
        Stream inputStream, Stream outputStream,
        Size maxSize, CancellationToken token = default)
    {
        Image image;
        try
        {
            // Load image from stream
            image = await Image.LoadAsync(inputStream, token);
        }
        catch
        {
            return false;
        }

        // Resize image if to large
        if (image.Height > maxSize.Height || image.Width > maxSize.Width)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = maxSize,
                Mode = ResizeMode.Max
            }));
        }

        // Convert image to Jpeg
        await image.SaveAsync(outputStream, JpegEncoder, token);
        return true;
    }
}