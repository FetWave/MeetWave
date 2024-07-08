using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FetWaveWWW.Pages.Events
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
        }

        private CalendarEvent? calendarEvent { get; set; }
        
        private Guid? UserId { get; set; }

        private void GotoEditEvent()
        {
            Navigation.NavigateTo($"/event/edit/{eventGuid}");
        }
    }
}
