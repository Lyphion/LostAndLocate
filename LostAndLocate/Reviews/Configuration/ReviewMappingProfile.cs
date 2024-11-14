using AutoMapper;
using LostAndLocate.Reviews.Models;

namespace LostAndLocate.Reviews.Configuration;

/// <summary>
/// Review Profile class for AutoMapper
/// </summary>
public sealed class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        // Setup AutoMapper Type conversion
        CreateMap<Review, ReviewDto>();
    }
}