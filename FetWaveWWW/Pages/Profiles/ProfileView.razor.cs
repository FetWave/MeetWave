using MeetWave.Data.DTOs.Profile;
using MeetWave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace MeetWave.Pages.Profiles
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
        public NavigationManager Navigation { get; set; }
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
                    SameUser = profile?.UserId == UserId.ToString();

                    if (profile == null)
                    {
                        Navigation.NavigateTo("/");
                        return;
                    }
                    else if (profile.PrivateProfile && !SameUser)
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

        private bool SameUser { get; set; }

        private bool NewUser { get; set; }
        private Guid? UserId { get; set; }
        private UserProfile? Profile { get; set; }
    }
}
