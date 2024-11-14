using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.Users.Models;

/// <summary>
/// Object storing the credentials of a user as salt and hash.
/// </summary>
[Owned]
public sealed class Credentials
{
    /// <summary>
    /// Amount of iterations should be performed when testing.
    /// </summary>
    public int Iterations { get; init; }

    /// <summary>
    /// Random salt of the credentials.
    /// </summary>
    public byte[] Salt { get; init; } = null!;

    /// <summary>
    /// Hashed password of a user.
    /// </summary>
    public byte[] Hash { get; init; } = null!;
}