using LostAndLocate.Files.Repositories;
using LostAndLocate.Files.Services;
using LostAndLocate.Utils;

namespace LostAndLocate.Files;

public sealed class FileModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection builder)
    {
        builder.AddScoped<IFileService, FileDatabaseService>();

        builder.AddScoped<IFileRepository, FileRepository>();

        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}