using Dozday.Core.DTOs;
using Dozday.Core.Models;

namespace Dozday.Core.Interfaces;

public interface ICalendarEventService
{
    Task<IReadOnlyList<EventDto>> GetSubscribedTeacherEventsAsync(Guid userId, DateTime from, DateTime to);
}
