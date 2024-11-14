using CSharpFunctionalExtensions;
using LostAndLocate.LostObjects.Models;
using LostAndLocate.LostObjects.Repositories;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Services;

namespace LostAndLocate.LostObjects.Services;

/// <summary>
/// Service implementation for lost objects.
/// </summary>
public sealed class LostObjectService : ILostObjectService
{
    private readonly ILostObjectRepository _repository;
    private readonly IUserService _userService;

    private readonly ILogger<LostObjectService> _logger;

    public LostObjectService(
        ILostObjectRepository repository,
        IUserService userService,
        ILogger<LostObjectService> logger)
    {
        _repository = repository;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Receives a list of <see cref="LostObject"/>s which match the criteria of the <see cref="LostObjectFilter"/>.
    /// </summary>
    /// <param name="filter">The filter for the search</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list with matching <see cref="LostObject"/>s</returns>
    public async Task<IEnumerable<LostObject>> GetObjectsAsync(
        LostObjectFilter? filter = null,
        CancellationToken token = default)
    {
        if (filter is null)
            return await _repository.GetAllAsync(token);
        return await _repository.GetAllAsync(filter, token);
    }

    /// <summary>
    /// Receives the matching <see cref="LostObject"/> with the provided Id.
    /// </summary>
    /// <param name="objectId">Id of the object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="LostObject"/> or None if Id was not found</returns>
    public async Task<Maybe<LostObject>> GetObjectAsync(
        Guid objectId,
        CancellationToken token = default)
    {
        return await _repository.GetAsync(objectId, token);
    }

    /// <summary>
    /// Creates a new <see cref="LostObject"/> with the given properties.
    /// </summary>
    /// <param name="name">The name of the object</param>
    /// <param name="coordinates">The coordinates where the object was found</param>
    /// <param name="userId">The Id of the <see cref="User"/> who found the object</param>
    /// <param name="description">A description of the object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The created <see cref="LostObject"/> or <see cref="LostObjectError"/> if some property is invalid</returns>
    public async Task<Result<LostObject, LostObjectError>> CreateObjectAsync(
        string name, Coordinates coordinates, Guid userId, string description,
        CancellationToken token = default)
    {
        // Check if name is valid
        if (name.Length is < LostObject.MinNameLength or > LostObject.MaxNameLength)
            return LostObjectError.InvalidName;

        // Get user
        var userOption = await _userService.GetUserAsync(userId, token);

        // Check if user exists
        if (!userOption.TryGetValue(out var user))
            return LostObjectError.InvalidUser;

        // Check if coordinates are valid
        if (!coordinates.Valid())
            return LostObjectError.InvalidCoordinates;

        // Create object
        var obj = new LostObject
        {
            Name = name,
            Coordinates = coordinates,
            User = user,
            Description = description,
        };

        _logger.LogInformation("Created object {Id}", obj.Id);

        // Update database
        return await _repository.AddAsync(obj, token);
    }

    /// <summary>
    /// Updates a <see cref="LostObject"/> with the given properties.
    /// </summary>
    /// <param name="objectId">The Id of the <see cref="LostObject"/></param>
    /// <param name="name">The name of the object</param>
    /// <param name="coordinates">The coordinates where the object was found</param>
    /// <param name="description">A description of the object</param>
    /// <param name="returned">Whether the object was returned</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The updated <see cref="LostObject"/> or <see cref="LostObjectError"/> if some property is invalid</returns>
    public async Task<Result<LostObject, LostObjectError>> UpdateObjectAsync(
        Guid objectId, string? name = null, Coordinates? coordinates = null,
        string? description = null, bool? returned = null,
        CancellationToken token = default)
    {
        // Get object
        var option = await GetObjectAsync(objectId, token);

        // Check if object exists
        if (!option.TryGetValue(out var obj))
            return LostObjectError.InvalidId;

        // Save name if provided and changed
        if (name is not null && !name.Equals(obj.Name, StringComparison.InvariantCulture))
        {
            // Check if name is valid
            if (name.Length is < LostObject.MinNameLength or > LostObject.MaxNameLength)
                return LostObjectError.InvalidName;

            obj.Name = name;
        }

        // Save coordinates if provided
        if (coordinates is not null)
        {
            // Check if coordinates are valid
            if (!coordinates.Valid())
                return LostObjectError.InvalidCoordinates;

            obj.Coordinates = coordinates;
        }

        // Save description if provided
        if (description is not null)
            obj.Description = description;

        // Save returned if provided
        if (returned is not null)
            obj.Returned = (bool) returned;

        _logger.LogInformation("Updated object {Id}", obj.Id);

        // Update database
        return await _repository.UpdateAsync(obj, token);
    }

    /// <summary>
    /// Deletes and returns the matching <see cref="LostObject"/> with the provided Id.
    /// </summary>
    /// <param name="objectId">Id of the <see cref="LostObject"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="LostObject"/> or None if Id was not found</returns>
    public async Task<Maybe<LostObject>> DeleteObjectAsync(
        Guid objectId,
        CancellationToken token = default)
    {
        // Update database
        var option = await _repository.DeleteAsync(objectId, token);

        if (option.HasValue)
            _logger.LogInformation("Deleted object {Id}", objectId);

        return option;
    }
}