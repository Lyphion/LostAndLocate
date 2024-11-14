using LostAndLocate.Users.Repositories;
using LostAndLocate.Users.Services;
using LostAndLocate.Utils;

namespace LostAndLocate.Users;

public sealed class UserModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection builder)
    {
        builder.AddSingleton<ISecurityService, SecurityService>();
        builder.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.AddScoped<IUserService, UserService>();

        builder.AddScoped<IUserRepository, UserRepository>();

        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}