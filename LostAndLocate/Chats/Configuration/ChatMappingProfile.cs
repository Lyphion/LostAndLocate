using AutoMapper;
using LostAndLocate.Chats.Models;

namespace LostAndLocate.Chats.Configuration;

/// <summary>
/// Chat Profile class for AutoMapper
/// </summary>
public sealed class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        // Setup AutoMapper Type conversion
        CreateMap<Chat, ChatDto>();
        CreateMap<SimpleChat, SimpleChatDto>();
        CreateMap<ChatMessage, ChatMessageDto>()
            .ForMember(
                c => c.Sender,
                option => option.MapFrom(c => c.Sender.Id))
            .ForMember(
                c => c.Target,
                option => option.MapFrom(c => c.Target.Id));
    }
}