using MeetWave.Data.DTOs.Events;
using MeetWave.Data.DTOs.Profile;
using MeetWave.Services;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.Contracts;

namespace MeetWave.Pages.Profiles
{
    public partial class PronounSelector : ComponentBase
    {

#nullable disable
        [Parameter]
        public EventCallback<int> OnPronounChange { get; set; }
        [Parameter]
        public int? SelectedPronounId { get; set; }
        [Inject]
        public ProfilesService Profiles { get; set; }
#nullable enable

        protected override async Task OnInitializedAsync()
        {
            Pronouns = await Profiles.GetPronounsList();
        }

        private IList<UserPronouns>? Pronouns { get; set; }

        async void OnPronounSelectChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value!.ToString(), out var pronounId))
            {
                await OnPronounChange.InvokeAsync(pronounId);
            }
            else
                await OnPronounChange.InvokeAsync(-1);
        }
    }
}
