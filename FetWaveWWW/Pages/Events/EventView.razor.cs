using MeetWave.Data.DTOs.Events;
using MeetWave.Helper;
using MeetWave.Services;
using MeetWave.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace MeetWave.Pages.Events
{
    [Authorize]
    public partial class EventView : ComponentBase
    {
#nullable disable
        [Inject]
        public EventsService Events { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public AuthHelperService Auth { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private IPaymentsService Payments { get; set; }
#nullable enable

        [Parameter]
        public string? eventGuid { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(await Auth.GetUserId(), out var userId))
            {
                UserId = userId;
            }

            if (Guid.TryParse(eventGuid, out var eventId))
            {
                calendarEvent = await Events.GetEventById(guid: eventId);
            }

            if (calendarEvent == null)
            {
                Navigation.NavigateTo("/events");
            }

            GoingHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent!.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Going);
            InterestedHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Interested);

            UserRsvp = (await Events.GetRSVPsForEvent(calendarEvent.Id) ?? []).FirstOrDefault(r => r.UserId == UserId.ToString());
        }

        private CalendarEvent? calendarEvent { get; set; }

        private string? GoingHTML { get; set; }
        private string? InterestedHTML { get; set; }
        
        private Guid? UserId { get; set; }

        private EventRSVP? UserRsvp { get; set; }

        private async Task UpdateRsvp(RsvpStateEnum? state, EventRSVP? rsvp, int? eventId)
        {
            if (state == null && rsvp != null)
            {
                await RSVPHelper.RemoveRSVP(Events, rsvp, UserId.ToString()!);
            }
            else if (state == RsvpStateEnum.Going)
            {
                await RSVPHelper.RSVPGoing(Events, rsvp, eventId, UserId.ToString());
            }
            else if (state == RsvpStateEnum.Interested)
            {
                await RSVPHelper.RSVPInterested(Events, rsvp, eventId, UserId.ToString());
            }
            else
            {
                //log error
            }
            UserRsvp = (await Events.GetRSVPsForEvent(calendarEvent!.Id) ?? []).FirstOrDefault(r => r.UserId == UserId.ToString());
            GoingHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Going);
            InterestedHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Interested);
            StateHasChanged();
        }

        private void GotoEditEvent()
        {
            Navigation.NavigateTo($"/event/manage/{eventGuid}");
        }
    }
}
