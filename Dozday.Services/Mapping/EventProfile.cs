using AutoMapper;
using Dozday.Core.DTOs;
using Dozday.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dozday.Services.Mapping
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventDto, Event>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.OrganizatorId, opt => opt.MapFrom(src => Guid.Parse(src.OrganizatorId)))
                .ForMember(dest => dest.Organizator, opt => opt.Ignore())
                .ForMember(dest => dest.Subscribers, opt => opt.Ignore());

            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.OrganizatorId, opt => opt.MapFrom(src => src.OrganizatorId.ToString()))
                .ForMember(dest => dest.OrganizatorName, opt => opt.Ignore())
                .ForMember(dest => dest.Subscribers, opt => opt.Ignore());
        }
    }
}
