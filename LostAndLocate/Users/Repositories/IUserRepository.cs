using CSharpFunctionalExtensions;
using LostAndLocate.Data;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Users.Repositories;

/// <summary>
/// Repository definitions for <see cref="User"/>s. 
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Receives a <see cref="User"/> by its <paramref name="name"/> from the database.
    /// </summary>
    /// <param name="name">The Name of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if name was not found</returns>
    Task<Maybe<User>> GetByNameAsync(string name, CancellationToken token = default);

    /// <summary>
    /// Receives a <see cref="User"/> by its <paramref name="email"/> from the database.
    /// </summary>
    /// <param name="email">The E-Mail of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if email was not found</returns>
    Task<Maybe<User>> GetByEmailAsync(string email, CancellationToken token = default);
}