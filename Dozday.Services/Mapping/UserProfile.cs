using AutoMapper;
using Dozday.Core.DTOs;
using Dozday.Core.Models;

namespace Dozday.Services.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserIdNameAvatarDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

        CreateMap<UserIdNameAvatarDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));
    }
}