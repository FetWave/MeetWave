using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using static FetWaveWWW.Pages.Events.RegionSelector;

namespace FetWaveWWW.Pages.Events
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
        }

        private int? RegionId { get; set; }
        private string? StateCode { get; set; }

        public async void GetEventsForRegion(OnRegionChangeCallbackArgs args)
        {
            CalendarEvents = (args.region?.Equals("all", StringComparison.OrdinalIgnoreCase) ?? false) && !string.IsNullOrEmpty(args.state)
                ? await Events.GetEventsForState(CalendarStartDate, CalendarEndDate, args.state)
                : await Events.GetEventsForRegion(CalendarStartDate, CalendarEndDate, int.TryParse(args.region, out var regionId) ? regionId : throw new Exception("Invalid region selection"));
            StateHasChanged();
        }
        
        private Guid? UserId { get; set; }

        private DateTime CalendarStartDate { get; set; } = DateTime.Now.AddDays(-2).Date;
        private DateTime CalendarEndDate { get; set; } = DateTime.Now.AddMonths(1).Date;

        private IEnumerable<CalendarEvent>? CalendarEvents { get; set; }

        private void NavCreateEvent()
        {
            Navigation.NavigateTo("/event/new");
        }

        private void GotoEditEvent(string eventGuid)
        {
            Navigation.NavigateTo($"/event/edit/{eventGuid}");
        }
    }
}
