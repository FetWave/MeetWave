using MeetWave.Data.DTOs.Messages;
using MeetWave.Helper;
using MeetWave.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;

namespace MeetWave.Pages.Messages
{
    public partial class Messages : ComponentBase
    {
#nullable disable
        [Inject]
        public MessagesService MessagesService { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public AuthHelperService Auth { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }

        private Popup popup;
        private IList<RadzenButton> buttons = [];
#nullable enable

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(await Auth.GetUserId(), out var userId))
            {
                UserId = userId;
            }
            else
            {
                Navigation.NavigateTo("/");
            }

            if (UserId != null)
            {
                UserMessages = await MessagesService.GetMessages(UserId.ToString()!);
                buttons = UserMessages?.Select(_ => new RadzenButton()).ToList();

                foreach(var message in UserMessages ?? [])
                {
                    if ((message?.Thread?.Id ?? 0) == 0 || (!message?.Lines?.Any() ?? true))
                    {
                        continue;
                    }
                    var recent = message!.Lines!.FirstOrDefault();
                    var isNew = recent != null && recent.Author.Id != UserId.ToString() && !recent.Reads.Any(r => r.Recipient.RecipientUserId == UserId.ToString());
                    IsThreadNew[message!.Thread!.Id] = isNew;
                }
                
            }
        }

        private Guid? UserId { get; set; }

        private Dictionary<long, bool> IsThreadNew { get; set; } = [];

        private IEnumerable<MessageWrapper?>? UserMessages { get; set; }
        RadzenDataList<MessageLine> dataList;
        IEnumerable<MessageLine> lines;
        private long ActiveThreadId { get; set; } = 0;
        private string newMessage = string.Empty;


        private async Task MarkAsRead(long threadId)
        {
            await MessagesService.MarkRead(UserId.ToString()!, UserMessages?.FirstOrDefault(m => m?.Thread?.Id == threadId)?.Lines?.FirstOrDefault()?.Id ?? 0);
            UserMessages = await MessagesService.GetMessages(UserId.ToString()!);
            StateHasChanged();
        }

        private async Task RefreshCurrentChat(long threadId)
        {
            UserMessages = await MessagesService.GetMessages(UserId.ToString()!); 
            lines = (UserMessages?.FirstOrDefault(m => m?.Thread?.Id == threadId)?.Lines ?? [])?
                .Where(l => l != null).Select(l => l!) ?? [];
        }

        private async Task OnMessageOpen()
        {
            lines = (UserMessages?.FirstOrDefault(m => m?.Thread?.Id == ActiveThreadId)?.Lines ?? [])?
                .Where(l => !string.IsNullOrEmpty(l?.LineText)).Select(l => l!) ?? [];
        }

        private async Task SendMessage()
        {
            await MessagesService.SendMessage(UserId.ToString()!, newMessage, threadId: ActiveThreadId);
            await RefreshCurrentChat(ActiveThreadId);
            newMessage = string.Empty;
            StateHasChanged();
        }
    }
}
