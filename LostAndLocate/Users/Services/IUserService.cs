using CSharpFunctionalExtensions;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Users.Services;

/// <summary>
/// Service definitions for users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Receives a list of all <see cref="User"/>s.
    /// </summary>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of all <see cref="User"/>s</returns>
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken token = default);

    /// <summary>
    /// Receives the matching <see cref="User"/> with the provided Id.
    /// </summary>
    /// <param name="userId">Id of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if Id was not found</returns>
    Task<Maybe<User>> GetUserAsync(
        Guid userId,
        CancellationToken token = default);

    /// <summary>
    /// Receives a <see cref="User"/> by its <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The Name of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if name was not found</returns>
    Task<Maybe<User>> GetUserByNameAsync(
        string name,
        CancellationToken token = default);

    /// <summary>
    /// Receives a <see cref="User"/> by its <paramref name="email"/>.
    /// </summary>
    /// <param name="email">The E-Mail of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if email was not found</returns>
    Task<Maybe<User>> GetUserByEmailAsync(
        string email,
        CancellationToken token = default);

    /// <summary>
    /// Creates a new <see cref="User"/> with the given properties.
    /// </summary>
    /// <param name="name">The name of the user</param>
    /// <param name="email">The E-Mail of the user</param>
    /// <param name="password">The password of the user</param>
    /// <param name="description">A description of the object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The created <see cref="User"/> or <see cref="UserError"/> if some property is invalid</returns>
    Task<Result<User, UserError>> CreateUserAsync(
        string name, string email, string password, string description,
        CancellationToken token = default);

    /// <summary>
    /// Updated a <see cref="User"/> with the given properties.
    /// </summary>
    /// <param name="userId">The name of the <see cref="User"/></param>
    /// <param name="name">The name of the user</param>
    /// <param name="email">The E-Mail of the user</param>
    /// <param name="password">The password of the user</param>
    /// <param name="description">A description of the object</param>
    /// <param name="admin">Whether the user is an admin</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The updated <see cref="User"/> or <see cref="UserError"/> if some property is invalid</returns>
    Task<Result<User, UserError>> UpdateUserAsync(
        Guid userId, string? name = null, string? email = null, string? password = null,
        string? description = null, bool? admin = null,
        CancellationToken token = default);

    /// <summary>
    /// Deletes and returns the matching <see cref="User"/> with the provided Id.
    /// </summary>
    /// <param name="userId">Id of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if Id was not found</returns>
    Task<Maybe<User>> DeleteUserAsync(
        Guid userId,
        CancellationToken token = default);
}