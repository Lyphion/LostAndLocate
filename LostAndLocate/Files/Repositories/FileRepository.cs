using CSharpFunctionalExtensions;
using LostAndLocate.Data;
using LostAndLocate.Files.Models;
using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.Files.Repositories;

/// <summary>
/// Entity Framework Core Repository for <see cref="SavedFile"/>s.
/// </summary>
public sealed class FileRepository : EfCoreRepository<SavedFile, IDbContext>, IFileRepository
{
    public FileRepository(IDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Receives the <see cref="SavedFile"/> with given <paramref name="group"/> and <paramref name="name"/> from the database.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="SavedFile"/> or None if group with name was not found</returns>
    public async Task<Maybe<SavedFile>> GetAsync(
        string group, string name,
        CancellationToken token = default)
    {
        return await Context.Set<SavedFile>()
            .Where(f => f.Group.ToLower().Equals(group.ToLower())
                        && f.Name.ToLower().Equals(name.ToLower()))
            .FirstOrDefaultAsync(token) ?? Maybe<SavedFile>.None;
    }

    /// <summary>
    /// Deletes and returns the matching <see cref="SavedFile"/> with the provided <paramref name="group"/> and <paramref name="name"/> from the database.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="SavedFile"/> or None if group with name was not found</returns>
    public async Task<Maybe<SavedFile>> DeleteAsync(
        string group, string name,
        CancellationToken token = default)
    {
        var option = await GetAsync(group, name, token);

        if (!option.TryGetValue(out var entity))
            return Maybe<SavedFile>.None;

        Context.Set<SavedFile>().Remove(entity);
        await Context.SaveChangesAsync(token);

        return entity;
    }
}