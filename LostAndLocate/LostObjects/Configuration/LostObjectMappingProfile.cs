using AutoMapper;
using LostAndLocate.LostObjects.Models;

namespace LostAndLocate.LostObjects.Configuration;

/// <summary>
/// Lost Object Profile class for AutoMapper
/// </summary>
public sealed class LostObjectMappingProfile : Profile
{
    public LostObjectMappingProfile()
    {
        // Setup AutoMapper Type conversion
        CreateMap<LostObject, LostObjectDto>();
    }
}