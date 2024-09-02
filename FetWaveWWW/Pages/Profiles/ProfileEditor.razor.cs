using MeetWave.Data.DTOs.Profile;
using MeetWave.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Radzen;
using static MeetWave.Pages.Events.RegionSelector;

namespace MeetWave.Pages.Profiles
{
    public partial class ProfileEditor : ComponentBase
    {
        [Parameter]
        public UserProfile Profile { get; set; }

#nullable disable
        [Inject]
        public ProfilesService Profiles { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
#nullable enable

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        public void GetRegion(OnRegionChangeCallbackArgs args)
            => Profile.DefaultRegionId = int.TryParse(args.region, out var regionId) && regionId > 0 ? regionId : null;

        public async void GetPronouns(int id)
        {
            Profile.Pronouns = null;
            Profile.PronounsId = id > 0 ? id : null;
        }

        private async Task AddEditProfile()
        {
            var existing = await Profiles.GetProfile(Guid.Parse(Profile.UserId));
            if (Profile.Id == 0 && existing != null)
            {
                return;
            }
            await Profiles.UpsertProfile(Profile);
            if (existing == null)
                Navigation.NavigateTo("/profile", true);
            StateHasChanged();
        }
    }
}
