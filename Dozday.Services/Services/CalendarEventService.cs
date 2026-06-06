using AutoMapper;
using Dozday.Core.DTOs;
using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Dozday.Core.Utils;
using Dozday.Data;
using Microsoft.EntityFrameworkCore;

namespace Dozday.Services.Services;

public class CalendarEventService(IEventService eventService, DozdayDbContext context, IMapper mapper, IUserEventSubscriptionService userEventSubscriptionService) : ICalendarEventService
{
    private readonly DozdayDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IUserEventSubscriptionService _userEventSubscriptionService = userEventSubscriptionService;
    private readonly IEventService _eventService = eventService;

    public async Task<IReadOnlyList<EventDto>> GetSubscribedTeacherEventsAsync(Guid userId, DateTime from, DateTime to)
    {
        from = DateUtils.NormalizeDateTime(from);
        to = DateUtils.NormalizeDateTime(to);

        var teacherIds = await _context.UserSubscriptions
            .Where(s => s.UserId == userId)
            .Select(s => s.TeacherId)
            .ToListAsync();

        //if (teacherIds.Count == 0)
        //{
        //    return [];
        //}

        //var events = await _context.Events
        //    .AsNoTracking()
        //    .Include(e => e.Organizator)
        //    .Where(e => teacherIds.Contains(e.OrganizatorId))
        //    .Where(e => e.End > from && e.Start < to)
        //    .OrderBy(e => e.Start)
        //    .ToListAsync();

        var events = await _eventService.GetEventsOnlyAsync(e => teacherIds.Contains(e.OrganizatorId), -1, -1);
        Console.WriteLine($"Found {events.Items.Count} events");

        //var timeZone = DateUtils.GetDisplayTimeZone();
        //foreach (var calendarEvent in events)
        //{
        //    calendarEvent.Start = DateUtils.ConvertToTimeZone(calendarEvent.Start, timeZone);
        //    calendarEvent.End = DateUtils.ConvertToTimeZone(calendarEvent.End, timeZone);
        //}

        return _mapper.Map<IReadOnlyList<EventDto>>(events.Items);
    }
}
