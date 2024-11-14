using CSharpFunctionalExtensions;
using LostAndLocate.LostObjects.Models;
using LostAndLocate.Users.Models;

namespace LostAndLocate.LostObjects.Services;

/// <summary>
/// Service definitions for lost objects.
/// </summary>
public interface ILostObjectService
{
    /// <summary>
    /// Receives a list of <see cref="LostObject"/>s which match the criteria of the <see cref="LostObjectFilter"/>.
    /// </summary>
    /// <param name="filter">The filter for the search</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list with matching <see cref="LostObject"/>s</returns>
    Task<IEnumerable<LostObject>> GetObjectsAsync(
        LostObjectFilter filter,
        CancellationToken token = default);

    /// <summary>
    /// Receives the matching <see cref="LostObject"/> with the provided Id.
    /// </summary>
    /// <param name="objectId">Id of the object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="LostObject"/> or None if Id was not found</returns>
    Task<Maybe<LostObject>> GetObjectAsync(
        Guid objectId,
        CancellationToken token = default);

    /// <summary>
    /// Creates a new <see cref="LostObject"/> with the given properties.
    /// </summary>
    /// <param name="name">The name of the object</param>
    /// <param name="coordinates">The coordinates where the object was found</param>
    /// <param name="userId">The Id of the <see cref="User"/> who found the object</param>
    /// <param name="description">A description of the object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The created <see cref="LostObject"/> or <see cref="LostObjectError"/> if some property is invalid</returns>
    Task<Result<LostObject, LostObjectError>> CreateObjectAsync(
        string name, Coordinates coordinates, Guid userId, string description,
        CancellationToken token = default);

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
    Task<Result<LostObject, LostObjectError>> UpdateObjectAsync(
        Guid objectId, string? name = null, Coordinates? coordinates = null,
        string? description = null, bool? returned = null,
        CancellationToken token = default);

    /// <summary>
    /// Deletes and returns the matching <see cref="LostObject"/> with the provided Id.
    /// </summary>
    /// <param name="objectId">Id of the <see cref="LostObject"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="LostObject"/> or None if Id was not found</returns>
    Task<Maybe<LostObject>> DeleteObjectAsync(
        Guid objectId,
        CancellationToken token = default);
}