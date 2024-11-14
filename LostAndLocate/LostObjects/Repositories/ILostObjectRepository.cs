using LostAndLocate.Data;
using LostAndLocate.LostObjects.Models;

namespace LostAndLocate.LostObjects.Repositories;

/// <summary>
/// Repository definitions for <see cref="LostObject"/>s. 
/// </summary>
public interface ILostObjectRepository : IRepository<LostObject>
{
    /// <summary>
    /// Receives a list of <see cref="LostObject"/>s which match the criteria of the <see cref="LostObjectFilter"/> from the database.
    /// </summary>
    /// <param name="filter">The filter for the search</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list with matching <see cref="LostObject"/>s</returns>
    Task<IEnumerable<LostObject>> GetAllAsync(
        LostObjectFilter filter,
        CancellationToken token = default);
}