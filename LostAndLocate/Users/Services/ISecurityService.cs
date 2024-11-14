using LostAndLocate.Users.Models;

namespace LostAndLocate.Users.Services;

/// <summary>
/// Service definitions for security.
/// </summary>
public interface ISecurityService
{
    /// <summary>
    /// Creates <see cref="Credentials"/> from <paramref name="password"/>.
    /// </summary>
    /// <param name="password">The password which should be converted</param>
    /// <returns>The <see cref="Credentials"/> of the <paramref name="password"/></returns>
    Credentials HashPassword(string password);

    /// <summary>
    /// Checks if the <see cref="Credentials"/> and <paramref name="password"/> match.
    /// </summary>
    /// <param name="password">The password to check</param>
    /// <param name="credentials">The credentials to be used</param>
    /// <returns>True if <see cref="Credentials"/> and <paramref name="password"/> match</returns>
    bool ValidatePassword(string password, Credentials credentials);
}