using CSharpFunctionalExtensions;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Users.Services;

/// <summary>
/// Service definitions for authentication.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Try to authenticate a <see cref="User"/> with <paramref name="name"/> and <paramref name="password"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="User"/></param>
    /// <param name="password">The password of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The <see cref="Authentication"/> of None if <paramref name="name"/> or <paramref name="password"/> is invalid</returns>
    Task<Maybe<Authentication>> AuthenticateAsync(
        string name, string password, 
        CancellationToken token = default);

    /// <summary>
    /// Checks if the provided <see cref="Authentication"/> token is valid.
    /// </summary>
    /// <param name="authentication">The authentication token</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Id if <see cref="Authentication"/> is valid else None</returns>
    Task<Maybe<Guid>> ValidateAsync(
        Authentication authentication, 
        CancellationToken token = default);
}