using CSharpFunctionalExtensions;
using LostAndLocate.Data;
using LostAndLocate.Files.Models;

namespace LostAndLocate.Files.Repositories;

/// <summary>
/// Repository definitions for <see cref="SavedFile"/>s. 
/// </summary>
public interface IFileRepository : IRepository<SavedFile>
{
    /// <summary>
    /// Receives the <see cref="SavedFile"/> with given <paramref name="group"/> and <paramref name="name"/> from the database.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="SavedFile"/> or None if group with name was not found</returns>
    Task<Maybe<SavedFile>> GetAsync(
        string group, string name,
        CancellationToken token = default);

    /// <summary>
    /// Deletes and returns the matching <see cref="SavedFile"/> with the provided <paramref name="group"/> and <paramref name="name"/> from the database.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="SavedFile"/> or None if group with name was not found</returns>
    Task<Maybe<SavedFile>> DeleteAsync(
        string group, string name,
        CancellationToken token = default);
}