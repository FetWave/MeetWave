using MeetWave.Data.DTOs.Events;
using MeetWave.Data.DTOs.Payments;
using MeetWave.Helper;
using MeetWave.Services;
using MeetWave.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace MeetWave.Pages.Events
{
    [Authorize]
    public partial class EventView : ComponentBase
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
        [Inject]
        public GoogleService Google { get; set; }
        [Inject]
        private OrdersService Orders { get; set; }
        [Inject]
        public MessagesService Messages { get; set; }
#nullable enable

        [Parameter]
        public string? eventGuid { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(await Auth.GetUserId(), out var userId))
            {
                UserId = userId;
            }

            if (Guid.TryParse(eventGuid, out var eventId))
            {
                calendarEvent = await Events.GetEventById(guid: eventId);
            }

            if (calendarEvent == null)
            {
                Navigation.NavigateTo("/events");
            }

            if (userId.ToString().Equals(calendarEvent?.CreatedUserId, StringComparison.OrdinalIgnoreCase))
            {
                Organizer = true;
                EventOrders = await Orders.GetOrdersByEventId(calendarEvent.Id);
                RSVPs = await Events.GetRSVPsForEvent(calendarEvent.Id);
                SelectedRSVPs = RSVPs?.Select(r => r.Id).ToDictionary(x => x!, _ => false) ?? [];
            }

            GoingHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent!.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Going);
            InterestedHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Interested);

            UserRsvp = (await Events.GetRSVPsForEvent(calendarEvent.Id) ?? []).FirstOrDefault(r => r.UserId == UserId.ToString());
            UserOrders = await Orders.GetOrdersByUserAndEventId(userId, calendarEvent.Id);
        }

        private bool Organizer { get; set; } = false;

        private CalendarEvent? calendarEvent { get; set; }

        private string? GoingHTML { get; set; }
        private string? InterestedHTML { get; set; }
        
        private Guid? UserId { get; set; }

        private EventRSVP? UserRsvp { get; set; }

        private IList<Order>? UserOrders { get; set; }
        private IList<Order>? EventOrders { get; set; }

        private IEnumerable<EventRSVP>? RSVPs { get; set; }
        private Dictionary<int, bool>? SelectedRSVPs { get; set; }
        private EmailListEnum? EmailList { get; set; } = EmailListEnum.All;
        private IEnumerable<IdentityUser>? InvoiceRecipients { get; set; }

        private string EmailSubject { get; set; } = string.Empty;
        private string EmailBody { get; set; } = string.Empty;

        private string? EmailFeedback { get; set; }

        private IList<PaymentWrapper.LineItem>? LineItems { get; set; }

        private void EmailListOnChange(ChangeEventArgs args)
        {
            EmailList = Enum.TryParse<EmailListEnum>(args.Value?.ToString(), out var value) ? value : null;
            UpdateRecipients();
        }

        private void UpdateRecipients()
        {
            InvoiceRecipients = EmailList switch
            {
                EmailListEnum.All => RSVPs?.Select(r => r.User).ToList(),
                EmailListEnum.Approved => RSVPs?.Where(r => r.ApprovedTS != null).Select(r => r.User).ToList(),
                EmailListEnum.Selected => RSVPs?.Where(r => (SelectedRSVPs?.TryGetValue(r.Id, out var selected) ?? false) ? selected : false).Select(r => r.User).ToList(),
                _ => null
            };
        }
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

        private async Task UpdateRsvp(RsvpStateEnum? state, EventRSVP? rsvp, int? eventId)
        {
            if (state == null && rsvp != null)
            {
                await RSVPHelper.RemoveRSVP(Events, rsvp, UserId.ToString()!);
            }
            else if (state == RsvpStateEnum.Going)
            {
                await RSVPHelper.RSVPGoing(Events, rsvp, eventId, UserId.ToString());
            }
            else if (state == RsvpStateEnum.Interested)
            {
                await RSVPHelper.RSVPInterested(Events, rsvp, eventId, UserId.ToString());
            }
            else
            {
                //log error
            }
            UserRsvp = (await Events.GetRSVPsForEvent(calendarEvent!.Id) ?? []).FirstOrDefault(r => r.UserId == UserId.ToString());
            GoingHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Going);
            InterestedHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Interested);
            StateHasChanged();
        }

        private async Task SendEmail()
        {
            if (!ValidateLineItems())
            {
                EmailFeedback = "Must have line items to send invoice";
            }

            foreach (var user in InvoiceRecipients ?? [])
            {
                await Orders.CreateOrder(calendarEvent!.Id, LineItems!, Guid.Parse(user.Id), UserId);
            }

            var toEmails = InvoiceRecipients?.Select(u => u.Email).Where(e => !string.IsNullOrEmpty(e));
            var startTime = calendarEvent!.StartDate;
            var formattedBody = !string.IsNullOrEmpty(EmailBody)
                ? "<br/>"
                    + "~~~~~"
                    + "<br/>"
                    + "~~~~~"
                    + "<br/>"
                    + EmailBody
                : string.Empty;
            var contextBody = $"You are receiving this invoice from {calendarEvent!.CreatedUser!.UserName} for the event {calendarEvent.Title}."
                + "<br/>"
                + $"Happening on {startTime?.DayOfWeek} {startTime:MM/dd/yyyy} at {startTime:hh:mm tt}"
                + formattedBody
                + "<br/>"
                + "~~~~~"
                + "<br/>"
                + "~~~~~"
                + "<br/>"
                + FormatLineItems();
            var contextSubject =   $"Fetwave event Invoice - {(!string.IsNullOrEmpty(EmailSubject) ? (" - " + EmailSubject) : string.Empty)}";

            await Messages.StartGroupMessageBCC(UserId.ToString()!, InvoiceRecipients?.Select(r => r.Id) ?? [], contextSubject, contextBody);

            if (toEmails?.Any() ?? false)
            {
                await Google.EmailListAsync(toEmails!, contextSubject, contextBody);
                EmailFeedback = "Email sent";
            }

        }

        private string FormatLineItems()
        {
            return string.Empty;
        }

        private bool ValidateLineItems()
        {
            return true;
        }

        private void GotoEditEvent()
        {
            Navigation.NavigateTo($"/event/manage/{eventGuid}");
        }
    }
}
