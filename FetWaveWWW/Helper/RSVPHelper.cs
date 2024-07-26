using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Services;
using Microsoft.IdentityModel.Abstractions;
using System.Diagnostics.Metrics;

namespace FetWaveWWW.Helper
{
    public class RSVPHelper
    {
        private static async Task<int?> RSVPChange(EventsService events, RsvpStateEnum status, EventRSVP? rsvp, int? eventId, string? userId)
        {
            var rsvpState = (await events.GetRSVPStates() ?? []).FirstOrDefault(s => s.Name!.Equals(status.ToString(), StringComparison.OrdinalIgnoreCase));
            if (rsvp == null)
            {
                rsvp = new EventRSVP()
                {
                    CreatedUserId = userId,
                    UserId = userId,
                    EventId = eventId,
                    RSVPStateId = rsvpState?.Id,
                };
            }
            else
            {
                rsvp.RSVPStateId = rsvpState?.Id;
                rsvp.State = rsvpState;
                rsvp.UpdatedTS = DateTime.UtcNow;
                rsvp.UpdatedUserId = userId;
                rsvp.DeletedTS = null;
            }

            return await events.UpsertRSVP(rsvp);
        }

        public static async Task<bool> RemoveRSVP(EventsService events, EventRSVP rsvp, string userId)
        {
            rsvp.UpdatedUserId = userId;
            rsvp.DeletedTS = DateTime.UtcNow;
            return await events.UpsertRSVP(rsvp) > 0;
        }

        public static async Task<int?> RSVPGoing(EventsService events, EventRSVP? rsvp = null, int? eventId = null, string? userId = null)
            => await RSVPChange(events, RsvpStateEnum.Going, rsvp, eventId, userId);

        public static async Task<int?> RSVPInterested(EventsService events, EventRSVP? rsvp = null, int? eventId = null, string? userId = null)
            => await RSVPChange(events, RsvpStateEnum.Interested, rsvp, eventId, userId);
    }
}
