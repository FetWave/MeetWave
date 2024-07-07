using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using static FetWaveWWW.Pages.Events.RegionSelector;

namespace FetWaveWWW.Pages.Events
{
    [Authorize]
    public partial class EditEvent : ComponentBase
    {
        [Parameter]
        public string? eventGuid { get; set; }

        [Inject]
        public EventsService Events { get; set; }
        [Inject]
        public AuthHelperService Auth { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }

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
                        CreatedUserId = UserId.ToString()
                    };
                }
            }
            else
            {
                Navigation.NavigateTo("/events");
            }

            editContext = new EditContext(SelectedEvent);

            messageStore = new(editContext);
        }

        private Guid UserId { get; set; }

        private CalendarEvent? SelectedEvent { get; set; }

        private EditContext? editContext;
        private ValidationMessageStore? messageStore;


        private async void SetRegionId(OnRegionChangeCallbackArgs args)
        {
            SelectedEvent!.RegionId = int.Parse(args.region!);
        }
        private async void SaveEvent()
        {
            await Events.UpsertEvent(SelectedEvent!);
        }
    }
}
