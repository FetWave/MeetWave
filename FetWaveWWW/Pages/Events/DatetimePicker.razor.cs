using Microsoft.AspNetCore.Components;

namespace FetWaveWWW.Pages.Events
{
    public partial class DatetimePicker : ComponentBase
    {

        [Parameter]
        public EventCallback<OnDatetimePickerChangeCallbackArgs> OnDatetimeChangeCallback { get; set; }
        [Parameter]
        public DateTime InitialDate { get; set; } = DateTime.Now;

        protected override async Task OnInitializedAsync()
        {
            Date = DateOnly.FromDateTime(InitialDate);
            Time = TimeOnly.FromDateTime(InitialDate);
        }

        public DateOnly Date { get; set; }
        private string dateValue
            => Date.ToString("yyyy-MM-dd");
        public TimeOnly Time { get; set; }
        private string timeValue
            => Time.ToString("HH:mm");

        public class OnDatetimePickerChangeCallbackArgs
        {
            public DateOnly Date { get; set; }
            public TimeOnly Time { get; set; }
            public DateTime DateTime
                => Date.ToDateTime(Time);
        }

        private async Task OnDateChange(ChangeEventArgs e)
        {
            Date = DateOnly.Parse(e.Value!.ToString());
            await OnDatetimeChange();
        }

        private async Task OnTimeChange(ChangeEventArgs e)
        {
            Time = TimeOnly.Parse(e.Value!.ToString());
            await OnDatetimeChange();
        }

        private async Task OnDatetimeChange()
        {
            await OnDatetimeChangeCallback.InvokeAsync(new() { Date = Date, Time = Time });
        }
    }
}
