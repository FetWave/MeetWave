using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Helper;
using FetWaveWWW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FetWaveWWW.Pages.Events
{
    [Authorize]
    public partial class EventContact : ComponentBase
    {
        [Parameter]
        public string? eventGuid { get; set; }

        [Parameter]
        public CalendarEvent? ContextEvent { get; set; }

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

                if (userId.ToString().Equals(ContextEvent?.CreatedUserId, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedEvent = ContextEvent;
                }
                else if (ContextEvent != null)
                {
                    Navigation.NavigateTo("/events");
                }
                else if (!string.IsNullOrEmpty(eventGuid))
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
                    Navigation.NavigateTo("/events");
                }
            }
            else
            {
                Navigation.NavigateTo("/events");
            }

            if (SelectedEvent != null)
            {
                RSVPs = await Events.GetRSVPsForEvent(SelectedEvent.Id);
                SelectedRSVPs = RSVPs?.Select(r => r.Id).ToDictionary(x => x!, _ => false) ?? [];
            }

            DressCodes = await Events.GetDressCodes();
            Categories = await Events.GetCategories();

            DressCodeValues = SelectedEvent!.DressCodes?.Select(d => d.Id) ?? [];
            CategoryValues = SelectedEvent.Categories?.Select(d => d.Id) ?? [];
        }

        private Dictionary<int, bool>? SelectedRSVPs { get; set; }

        private EmailListEnum? EmailList { get; set; }

        private void EmailListOnChange(ChangeEventArgs args)
        {
            EmailList = Enum.TryParse<EmailListEnum>(args.Value?.ToString(), out var value) ? value : null;
        }

        private string EmailSubject { get; set; } = string.Empty;
        private string EmailBody { get; set; } = string.Empty;

        private Guid UserId { get; set; }

        private CalendarEvent? SelectedEvent { get; set; }

        private IEnumerable<EventRSVP>? RSVPs { get; set; }

        private IEnumerable<DressCode>? DressCodes { get; set; }
        private IEnumerable<int> DressCodeValues { get; set; } = [];
        private IEnumerable<Category>? Categories { get; set; }
        private IEnumerable<int> CategoryValues { get; set; } = [];

        private async Task ApproveRSVP(int id)
        {
            var r = RSVPs?.FirstOrDefault(r => r.Id == id);
            if (r == null)
                return;
            r.ApprovedByUserId = UserId.ToString();
            r.ApprovedTS = DateTime.UtcNow;
            await Events.UpsertRSVP(r);
        }

        private async Task UnapproveRSVP(int id)
        {
            var r = RSVPs?.FirstOrDefault(r => r.Id == id);
            if (r == null)
                return;
            r.UpdatedUserId = UserId.ToString();
            r.ApprovedByUserId = null;
            r.ApprovedTS = null;
            await Events.UpsertRSVP(r);
        }

    }
}
