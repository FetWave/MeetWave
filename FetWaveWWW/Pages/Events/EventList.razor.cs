using MeetWave.Data.DTOs.Events;
using MeetWave.Helper;
using MeetWave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using static MeetWave.Pages.Events.DatetimePicker;
using static MeetWave.Pages.Events.RegionSelector;

namespace MeetWave.Pages.Events
{
    [Authorize]
    public partial class EventList : ComponentBase
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
#nullable enable

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(await Auth.GetUserId(), out var userId))
            {
                UserId = userId;
            }

            if (UserId != null)
            {
                OrganizedEvents = await Events.GetOrganizingEvents(UserId.ToString()!, CalendarStartDate, CalendarEndDate);
                RsvpedEvents = await Events.GetRsvpedEvents(UserId.ToString()!, CalendarStartDate, CalendarEndDate);
            }
            loading = false;
        }

        private bool loading { get; set; } = true;

        private int? RegionId { get; set; }
        private string? StateCode { get; set; }
        private bool? AllRegionsForState { get; set; }

        public async void GetEventsForRegion(OnRegionChangeCallbackArgs args)
        {
            if ((args.region?.Equals("all", StringComparison.OrdinalIgnoreCase) ?? false) && !string.IsNullOrEmpty(args.state))
            {
                AllRegionsForState = true;
                StateCode = args.state;
            }
            else
            {
                AllRegionsForState = false;
                StateCode = null;
                if (int.TryParse(args.region, out var regionId))
                    RegionId = regionId;
            }
            await UpdateCalendarEvents();
        }

        private async Task UpdateCalendarEvents()
        {
            if (UserId != null)
            {
                OrganizedEvents = await Events.GetOrganizingEvents(UserId.ToString()!, CalendarStartDate, CalendarEndDate);
                RsvpedEvents = await Events.GetRsvpedEvents(UserId.ToString()!, CalendarStartDate, CalendarEndDate);
                foreach (var e in OrganizedEvents ?? [])
                {
                    EventRsvps[e.Id] = await Events.GetRSVPsForEvent(e.Id) ?? [];
                }
                foreach (var e in RsvpedEvents ?? [])
                {
                    EventRsvps[e.Id] = await Events.GetRSVPsForEvent(e.Id) ?? [];
                }
            }

            if (!string.IsNullOrEmpty(StateCode) || RegionId.HasValue)
            {

                CalendarEvents = (AllRegionsForState ?? false) && !string.IsNullOrEmpty(StateCode)
                    ? await Events.GetEventsForState(CalendarStartDate, CalendarEndDate, StateCode)
                    : await Events.GetEventsForRegion(CalendarStartDate, CalendarEndDate, RegionId ?? throw new Exception("Invalid region selection"));

                foreach(var e in CalendarEvents ?? [])
                {
                    EventRsvps[e.Id] = await Events.GetRSVPsForEvent(e.Id) ?? [];
                }

                
            }

            StateHasChanged();
        }
        
        private Guid? UserId { get; set; }

        private DateTime CalendarStartDate { get; set; } = DateTime.Now.Date;
        private DateTime CalendarEndDate { get; set; } = DateTime.Now.AddMonths(1).Date;

        private async Task StartDateChange(OnDatetimePickerChangeCallbackArgs args)
        {
            CalendarStartDate = args.DateTime;
            await UpdateCalendarEvents();
        }

        private async Task EndDateChange(OnDatetimePickerChangeCallbackArgs args)
        {
            CalendarEndDate = args.DateTime;
            await UpdateCalendarEvents();
        }

        private IEnumerable<CalendarEvent>? CalendarEvents { get; set; }
        private IEnumerable<CalendarEvent>? OrganizedEvents { get; set; }
        private IEnumerable<CalendarEvent>? RsvpedEvents { get; set; }
        private Dictionary<int, IEnumerable<EventRSVP>> EventRsvps { get; set; } = [];

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
            eventId ??= rsvp?.EventId;
            EventRsvps[eventId.Value] = await Events.GetRSVPsForEvent(eventId.Value) ?? [];
            StateHasChanged();
        }

        private void NavCreateEvent()
        {
            Navigation.NavigateTo("/event/manage");
        }

        private void GotoEditEvent(string eventGuid)
        {
            Navigation.NavigateTo($"/event/manage/{eventGuid}");
        }
    }
}
