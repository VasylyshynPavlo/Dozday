using AutoMapper;
using Dozday.Core.DTOs;
using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Dozday.Data;
using Dozday.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dozday.Services.Services
{
    public class EventService(IEventRepository eventRepository, IMapper mapper, IUserRepository userRepository, IUserEventSubscriptionService userEventSubscriptionService) : IEventService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUserEventSubscriptionService _userEventSubscriptionService = userEventSubscriptionService;

        public async Task AddAsync(CreateEventDto entity)
        {
            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Summary = entity.Summary,
                Description = entity.Description,
                Location = entity.Location,
                Start = entity.Start,
                End = entity.End,
                OrganizatorId =  Guid.TryParse(entity.OrganizatorId, out var organizatorId) ? organizatorId : Guid.Empty,
                MeetingLink = entity.MeetingLink,
                Type = entity.Type
            };
            await _eventRepository.AddAsync(eventEntity);
        }

        public async Task DeleteAsync(string id, string userId)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                throw new ArgumentException("Невірний формат ID користувача");
            }
            if (!Guid.TryParse(id, out var parsedEventId))
            {
                throw new ArgumentException("Невірний формат ID події");
            }
            if (!await _eventRepository.IsOwnerOf(parsedUserId, parsedEventId) && !await _userRepository.IsAdmin(parsedUserId))
            {
                throw new UnauthorizedAccessException("Ви не є організатором цієї події або не є адміністратором");
            }
            await _eventRepository.DeleteAsync(parsedEventId);
        }

        public async Task<PagedResult<TResult>> GetEventsAsync<TResult>(
        Expression<Func<Event, TResult>> selector,
        Expression<Func<Event, bool>> predicate,
        int page = 1, int pageSize = 10,
        Dictionary<string, bool>? orderBy = null)
        { 
            return await _eventRepository.GetEventAsync(selector, predicate, page, pageSize, orderBy);
        }

        public async Task<PagedResult<EventDto>> GetEventsOnlyAsync(
        Expression<Func<Event, bool>> predicate,
        int page = 1,
        int pageSize = 10,
        Dictionary<string, bool>? orderBy = null)
        {
            var events = await _eventRepository.GetEventAsync(
                e => e,
                predicate,
                page,
                pageSize,
                orderBy);

            var newPagedResultDto = new PagedResult<EventDto>
            {
                Items = _mapper.Map<List<EventDto>>(events.Items),
                TotalCount = events.TotalCount,
                Page = events.Page,
                PageSize = events.PageSize,
                TotalPages = events.TotalPages
            };

            var eventIds = newPagedResultDto.Items.Select(x => x.Id).ToList();

            var subscriptions =
                await _userEventSubscriptionService.GetByRangeWithUniqueEventId(eventIds);

            foreach (var item in newPagedResultDto.Items)
            {
                item.Subscribers =
                    subscriptions.GetValueOrDefault(item.Id, 0);
            }

            return newPagedResultDto;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<PagedResult<EventDto>> GetAllByOrganiserIdAsync(string organiserId)
        {
            var parsedOrganiserId = Guid.TryParse(organiserId, out var guidOrganiserId) ? guidOrganiserId : Guid.Empty;
            var data = await _eventRepository.GetEventAsync<Event>(e => new Event(e), e => e.OrganizatorId == guidOrganiserId, 1, 999);

            return new PagedResult<EventDto>
            {
                Items = _mapper.Map<List<EventDto>>(data.Items),
                TotalCount = data.TotalCount,
                Page = data.Page,
                PageSize = data.PageSize,
                TotalPages = data.TotalPages
            };
        }

        public async Task<EventDto?> GetByIdAsync(string id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(Guid.TryParse(id, out var guidId) ? guidId : Guid.Empty);
            return _mapper.Map<EventDto?>(eventEntity);
        }

        public async Task<IEnumerable<EventDto>> GetPagedListAsync(int page, int pageSize)
        {
            var events = await _eventRepository.GetPagedListAsync(page, pageSize);
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<IEnumerable<EventDto>> GetRangeListAsync(DateTime from, DateTime to)
        {
            var events = await _eventRepository.GetRangeListAsync(from, to);
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task UpdateAsync(EventDto entity, string userId)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                throw new ArgumentException("Невірний формат ID користувача");
            }
            if (!Guid.TryParse(entity.Id, out var guidId))
            {
                throw new ArgumentException("Невірний формат ID події");
            }
            if (!await _eventRepository.IsOwnerOf(parsedUserId, guidId))
            {
                throw new UnauthorizedAccessException("Ви не є організатором цієї події");
            }
            await _eventRepository.UpdateAsync(new Event
            {
                Id = guidId,
                Summary = entity.Summary,
                Description = entity.Description,
                Location = entity.Location,
                Start = entity.Start,
                End = entity.End,
                OrganizatorId = Guid.TryParse(entity.OrganizatorId, out var organizatorId) ? organizatorId : Guid.Empty,
                MeetingLink = entity.MeetingLink,
                Type = entity.Type
            });
        }

        public async Task<List<Event>> SearchAsync(string text)
        {
            return _mapper.Map<List<Event>>(await _eventRepository.SearchAsync(text));
        }
    }
}
