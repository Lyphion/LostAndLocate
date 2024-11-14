using LostAndLocate.Reviews.Repositories;
using LostAndLocate.Reviews.Services;
using LostAndLocate.Utils;

namespace LostAndLocate.Reviews;

public sealed class ReviewModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection builder)
    {
        builder.AddScoped<IReviewService, ReviewService>();

        builder.AddScoped<IReviewRepository, ReviewRepository>();

        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}