using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace FetWaveWWW.Pages.Events
{
    public partial class Edit
    {
        [Parameter]
        public string? eventGuid { get; set; }

#nullable disable

        [Inject]
        public EventsService Events { get; set; }
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

                if (!string.IsNullOrEmpty(eventGuid))
                {
                    if (Guid.TryParse(eventGuid, out var eventId))
                    {
                        var tmpEvent = await Events.GetEventById(guid: eventId);
                        if (userId.ToString().Equals(tmpEvent?.CreatedUserId, StringComparison.OrdinalIgnoreCase))
                        {
                            SelectedEvent = tmpEvent;
                        }
                        else
                        {
                            Navigation.NavigateTo("/events");
                        }
                    }
                    else
                    {
                        Navigation.NavigateTo("/events");
                    }
                }
                else
                {
                    SelectedEvent = new CalendarEvent()
                    {
                        CreatedUserId = UserId.ToString(),
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddHours(3),
                    };
                }
            }
            else
            {
                Navigation.NavigateTo("/events");
            }
        }

        private Guid UserId { get; set; }

        private CalendarEvent? SelectedEvent { get; set; }
    }
}
