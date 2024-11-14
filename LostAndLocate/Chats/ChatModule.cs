using LostAndLocate.Chats.Repositories;
using LostAndLocate.Chats.Services;
using LostAndLocate.Utils;

namespace LostAndLocate.Chats;

public sealed class ChatModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection builder)
    {
        builder.AddScoped<IChatService, ChatService>();

        builder.AddScoped<IChatRepository, ChatRepository>();
        builder.AddSingleton<IChatWebSocketHandler, ChatWebSocketHandler>();

        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}