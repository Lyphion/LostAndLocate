namespace LostAndLocate.Utils;

/// <summary>
/// Class definitions for Module registration.
/// </summary>
public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection builder);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}

/// <summary>
/// Extension class for registering module.
/// </summary>
public static class ModuleExtensions
{
    private static readonly List<IModule> RegisteredModules = new();

    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        var modules = DiscoverModules();
        foreach (var module in modules)
        {
            module.RegisterModule(services);
            RegisteredModules.Add(module);
        }

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (var module in RegisteredModules)
            module.MapEndpoints(app);

        return app;
    }

    private static IEnumerable<IModule> DiscoverModules()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }
}