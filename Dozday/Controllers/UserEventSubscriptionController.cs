using Dozday.Core.DTOs;
using Dozday.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dozday.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserEventSubscriptionController : ControllerBase
    {
        private readonly IUserEventSubscriptionService _subscriptionService;

        public UserEventSubscriptionController(IUserEventSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("status")]
        public async Task<ActionResult<bool>> IsSubed([FromQuery] string eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(eventId, out _) || !Guid.TryParse(userId, out _))
            {
                return BadRequest("Невірний формат ID події або користувача");
            }

            var result = await _subscriptionService.IsSubed(eventId, userId);
            return Ok(result);
        }

        [HttpPost("toggle")]
        public async Task<ActionResult<bool>> ToggleSubscription([FromQuery] string eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(eventId, out var eventGuid) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest("Невірний формат ID події або користувача");
            }

            var isSubscribed = await _subscriptionService.IsSubed(eventId, userId);
            if (isSubscribed)
            {
                var existing = await _subscriptionService.GetAsync(
                    s => s.Id,
                    s => s.EventId == eventGuid && s.UserId == userGuid,
                    1,
                    1);

                var subscriptionId = existing.Items.FirstOrDefault();
                if (subscriptionId != Guid.Empty)
                {
                    await _subscriptionService.DeleteAsync(subscriptionId.ToString());
                }

                return Ok(false);
            }

            await _subscriptionService.AddAsync(new UserEventSubscriptionDto
            {
                Id = Guid.NewGuid().ToString(),
                EventId = eventId,
                UserId = userId
            });

            return Ok(true);
        }
    }
}
