using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Services;
using System.Web;

namespace FetWaveWWW.Helper
{
    public class HtmlHelper
    {
        public static async Task<string> GetRsvpMemberList(EventsService events, int eventId, bool isOrganizer, RsvpStateEnum? status = null)
        {
            
            IEnumerable<EventRSVP> rsvps;
            if (status != null)
            {
                rsvps = (status.Value == RsvpStateEnum.Going
                    ? await events.GetGoingsForEvent(eventId)
                    : await events.GetInterestedsForEvent(eventId)) ?? [];
            }
            else
            {
                rsvps = (await events.GetRSVPsForEvent(eventId)) ?? [];
            }
            var members = rsvps.Where(r => !(r.Private ?? false) || isOrganizer)
                 .Select(e =>
                 {
                     var profileId = e.User.Id;
                     var profileName = HttpUtility.HtmlEncode(e.User.UserName);
                     return $"<a href='/member/{profileId}'><span class='badge rounded-pill bg-primary'><i class='bi bi-person'></i>{profileName}</span></a>";
                 });
            return string.Join(" ", members);
        }
    }
}
