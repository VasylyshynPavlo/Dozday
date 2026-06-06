using AutoMapper;
using Dozday.Core.DTOs;
using Dozday.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dozday.Services.Mapping
{
    public class UserEventSubscriptionProfile : Profile
    {
        public UserEventSubscriptionProfile()
        {
            CreateMap<UserEventSubscription, UserEventSubscriptionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId.ToString()));

            CreateMap<UserEventSubscriptionDto, UserEventSubscription>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => Guid.Parse(src.EventId)));
        }
    }
}
