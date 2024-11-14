using CSharpFunctionalExtensions;
using LostAndLocate.Data;
using LostAndLocate.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.Users.Repositories;

/// <summary>
/// Entity Framework Core Repository for <see cref="User"/>s.
/// </summary>
public sealed class UserRepository : EfCoreRepository<User, IDbContext>, IUserRepository
{
    public UserRepository(IDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Receives a <see cref="User"/> by its <paramref name="name"/> from the database.
    /// </summary>
    /// <param name="name">The Name of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if name was not found</returns>
    public async Task<Maybe<User>> GetByNameAsync(string name, CancellationToken token = default)
    {
        return await Context.Set<User>()
            .Where(u => u.Name.ToLower().Equals(name.ToLower()))
            .FirstOrDefaultAsync(token) ?? Maybe<User>.None;
    }

    /// <summary>
    /// Receives a <see cref="User"/> by its <paramref name="email"/> from the database.
    /// </summary>
    /// <param name="email">The E-Mail of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if email was not found</returns>
    public async Task<Maybe<User>> GetByEmailAsync(string email, CancellationToken token = default)
    {
        return await Context.Set<User>()
            .Where(u => u.Email.ToLower().Equals(email.ToLower()))
            .FirstOrDefaultAsync(token) ?? Maybe<User>.None;
    }
}