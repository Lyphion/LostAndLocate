using LostAndLocate.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests;

public static class DbContextExtensions
{
    public static DbSet<T> ToDbSet<T>(this IEnumerable<T> entities)
        where T : class, IEntity
    {
        var data = entities.ToArray();

        var set = data.AsQueryable()
            .BuildMockDbSet();

        set.Add(Arg.Any<T>())
            .Returns(x =>
            {
                var mock = Substitute.For<EntityEntry<T>>(new object?[1]);
                mock.Entity.Returns(x.Arg<T>());
                return mock;
            });

        set.Update(Arg.Any<T>())
            .Returns(x =>
            {
                var mock = Substitute.For<EntityEntry<T>>(new object?[1]);
                mock.Entity.Returns(x.Arg<T>());
                return mock;
            });

        return set;
    }
}