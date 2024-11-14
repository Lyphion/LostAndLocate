using System.Security.Cryptography;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Users.Services;

/// <summary>
/// Service implementation for security.
/// </summary>
public sealed class SecurityService : ISecurityService
{
    private readonly IConfiguration _configuration;

    public SecurityService(IConfiguration configuration)
    {
        _configuration = configuration.GetSection("Security");
    }

    /// <summary>
    /// Creates <see cref="Credentials"/> from <paramref name="password"/>.
    /// </summary>
    /// <param name="password">The password which should be converted</param>
    /// <returns>The <see cref="Credentials"/> of the <paramref name="password"/></returns>
    public Credentials HashPassword(string password)
    {
        // Setting up algorithm
        using var algorithm = new Rfc2898DeriveBytes(
            password,
            _configuration.GetValue<int>("SaltSize"),
            _configuration.GetValue<int>("Iterations"),
            HashAlgorithmName.SHA256);

        // Hashing password
        var hash = algorithm.GetBytes(_configuration.GetValue<int>("HashSize"));

        // Creating credentials
        var credentials = new Credentials
        {
            Iterations = algorithm.IterationCount,
            Hash = hash,
            Salt = algorithm.Salt
        };

        return credentials;
    }

    /// <summary>
    /// Checks if the <see cref="Credentials"/> and <paramref name="password"/> match.
    /// </summary>
    /// <param name="password">The password to check</param>
    /// <param name="credentials">The credentials to be used</param>
    /// <returns>True if <see cref="Credentials"/> and <paramref name="password"/> match</returns>
    public bool ValidatePassword(string password, Credentials credentials)
    {
        // Setting up algorithm
        using var algorithm = new Rfc2898DeriveBytes(
            password,
            credentials.Salt,
            credentials.Iterations,
            HashAlgorithmName.SHA256);

        // Checking hash of credentials with provided password
        var hash = algorithm.GetBytes(_configuration.GetValue<int>("HashSize"));
        return hash.SequenceEqual(credentials.Hash);
    }
}