using LostAndLocate.LostObjects.Repositories;
using LostAndLocate.LostObjects.Services;
using LostAndLocate.Utils;

namespace LostAndLocate.LostObjects;

public sealed class LostObjectModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection builder)
    {
        builder.AddScoped<ILostObjectService, LostObjectService>();

        builder.AddScoped<ILostObjectRepository, LostObjectRepository>();

        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}