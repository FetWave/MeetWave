﻿using MeetWave.Data.DTOs.Events;
using MeetWave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.UI;
using System.Runtime.CompilerServices;
using static MeetWave.Pages.Events.DatetimePicker;
using static MeetWave.Pages.Events.RegionSelector;

namespace MeetWave.Pages.Events
{
    [Authorize]
    public partial class EditEvent : ComponentBase
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

            if (SelectedEvent != null)
            {
                editContext = new EditContext(SelectedEvent);
                messageStore = new(editContext);

                DressCodeValues = SelectedEvent.DressCodes?.Select(d => d.Id) ?? [];
                CategoryValues = SelectedEvent.Categories?.Select(d => d.Id) ?? [];
            }
            DressCodes = await Events.GetDressCodes();
            Categories = await Events.GetCategories();
        }

        private Guid UserId { get; set; }

        private CalendarEvent? SelectedEvent { get; set; }

        private EditContext? editContext;
        private ValidationMessageStore? messageStore;

        private IEnumerable<DressCode>? DressCodes { get; set; }
        private IEnumerable<int> DressCodeValues { get; set; } = [];
        private IEnumerable<Category>? Categories { get; set; }
        private IEnumerable<int> CategoryValues { get; set; } = [];


        private async void SetRegionId(OnRegionChangeCallbackArgs args)
        {
            SelectedEvent!.RegionId = int.Parse(args.region!);
        }

		private void StartDateChange(OnDatetimePickerChangeCallbackArgs args)
		{
			SelectedEvent!.StartDate = args.DateTime;
		}

		private void EndDateChange(OnDatetimePickerChangeCallbackArgs args)
		{
			SelectedEvent!.EndDate = args.DateTime;
		}
		private async void SaveEvent()
        {
            if (SelectedEvent!.Id > 0)
            {
                SelectedEvent.UpdatedTS = DateTime.UtcNow;
                SelectedEvent.UpdatedUserId = UserId.ToString();
            }

            SelectedEvent.DressCodes = DressCodes?.Where(d => DressCodeValues.Contains(d.Id)).ToList() ?? [];
            SelectedEvent.Categories = Categories?.Where(c => CategoryValues.Contains(c.Id)).ToList() ?? [];
            var eventGuid = await Events.UpsertEvent(SelectedEvent);
            Navigation.NavigateTo($"/event/{eventGuid}");
        }
    }
}
