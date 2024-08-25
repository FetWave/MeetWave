using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Data.DTOs.Profile;
using FetWaveWWW.Helper;
using FetWaveWWW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Drawing;
using System.Drawing.Text;
using static FetWaveWWW.Pages.Events.RegionSelector;

namespace FetWaveWWW.Pages.Profiles
{
    [Authorize]
    public partial class ProfileView : ComponentBase
    {
#nullable disable
        [Inject]
        public EventsService Events { get; set; }
        [Inject]
        public ProfilesService Profiles { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public AuthHelperService Auth { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
#nullable enable

        [Parameter]
        public string? userName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(await Auth.GetUserId(), out var userId))
            {
                UserId = userId;
            }
            if (UserId == null)
            {
                Navigation.NavigateTo("/");
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(userName))
                {
                    var profile = await Profiles.GetProfile(UserId.Value);
                    if (profile != null)
                    {
                        Navigation.NavigateTo($"/profile/{profile.User.UserName}", true);
                        return;
                    }
                    Profile = new() { UserId = UserId.ToString()! };
                    NewUser = true;
                }
                else
                {
                    var profile = await Profiles.GetProfile(userName);
                    if (profile == null)
                    {
                        Navigation.NavigateTo("/");
                        return;
                    }
                    else if (profile.PrivateProfile && profile.UserId != UserId.ToString())
                    {
                        Profile = ProfilesService.PrivateProfile;
                    }
                    else
                    {
                        Profile = profile;
                    }
                    
                }
            }
        }

        public void GetRegion(OnRegionChangeCallbackArgs args)
            => Profile.DefaultRegionId = int.TryParse(args.region, out var regionId) && regionId > 0 ? regionId : null;

        public void GetPronouns(int id)
            => Profile.PronounsId = id > 0 ? id : null;

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

        private bool NewUser { get; set; }
        private Guid? UserId { get; set; }
        private UserProfile? Profile { get; set; }
    }
}
