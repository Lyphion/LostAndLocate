using LostAndLocate.Data;
using LostAndLocate.LostObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.LostObjects.Repositories;

/// <summary>
/// Entity Framework Core Repository for <see cref="LostObject"/>s.
/// </summary>
public sealed class LostObjectRepository : EfCoreRepository<LostObject, IDbContext>, ILostObjectRepository
{
    public LostObjectRepository(IDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Receives a list of <see cref="LostObject"/>s which match the criteria of the <see cref="LostObjectFilter"/>.
    /// </summary>
    /// <param name="filter">The filter for the search</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list with matching <see cref="LostObject"/>s</returns>
    public async Task<IEnumerable<LostObject>> GetAllAsync(
        LostObjectFilter filter, CancellationToken token = default)
    {
        var query = Context.Set<LostObject>() as IQueryable<LostObject>;

        // Check if name is present, filter for name
        if (filter.Name is not null)
            query = query.Where(l => l.Name.ToLower().Contains(filter.Name.ToLower()));

        // Check if user is present, filter for user
        if (filter.User is not null)
            query = query.Where(l => l.User.Name.ToLower().Contains(filter.User.ToLower()));

        // Check if date is present, filter for date
        if (filter.Before is not null)
            query = query.Where(l => l.Created <= filter.Before);
        if (filter.After is not null)
            query = query.Where(l => l.Created >= filter.After);

        // Check if returned is present, filter for returned
        if (filter.Returned is not null)
            query = query.Where(l => l.Returned == filter.Returned);

        // Check if amount is present, limit amount
        if (filter.MaxAmount is not null)
            query = query.Take((int) filter.MaxAmount);

        var result = await query.ToListAsync(token);

        // Check if location is present, limit radius
        if (filter.Location is null)
            return result;
        
        var center = new Coordinates
        {
            Latitude = filter.Location.Latitude ?? 0,
            Longitude = filter.Location.Longitude ?? 0
        };
        return result.Where(l => l.Coordinates.Distance(center) <= filter.Location.Radius);
    }
}