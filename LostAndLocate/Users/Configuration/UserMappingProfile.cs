using AutoMapper;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Users.Configuration;

/// <summary>
/// User Profile class for AutoMapper
/// </summary>
public sealed class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Setup AutoMapper Type conversion
        CreateMap<User, UserDto>();
    }
}