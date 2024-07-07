using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Services;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.Contracts;

namespace FetWaveWWW.Pages.Events
{
    public partial class RegionSelector
    {

#nullable disable
        [Parameter]
        public EventCallback<OnRegionChangeCallbackArgs> OnRegionChange { get; set; }
        [Parameter]
        public bool ShowAllRegion { get; set; }
        [Parameter]
        public int? SelectedRegionId { get; set; }
        [Inject]
        public EventsService Events { get; set; }
#nullable enable

        protected override async Task OnInitializedAsync()
        {
            Regions = await Events.GetRegions();
            var statesList = new List<string>()
            {
                "Online"
            };
            statesList.AddRange(
                Regions
                ?.Where(r => !r.StateCode?.Equals("Online", StringComparison.OrdinalIgnoreCase) ?? false)
                .Select(r => r.StateCode)
                .Distinct()
                .Order()
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c => c!)
                ?? []);

            States = statesList;

            if (SelectedRegionId.HasValue)
            {
                var region = Regions?.FirstOrDefault(r => r.Id == SelectedRegionId.Value);
                if (region != null)
                {
                    StartStateValue = region.StateCode!;
                    StateCode = region.StateCode;
                    GetRegionsForState();
                    StartRegionValue = region.Id.ToString();
                    RegionId = region.Id;
                }
            }
        }

        private string StartStateValue { get; set; } = string.Empty;
        private string StartRegionValue { get; set; } = string.Empty;

        private IEnumerable<Region>? Regions { get; set; }
        private IEnumerable<string>? States { get; set; }
        public string? StateCode { get; set; }
        public int? RegionId { get; set; }

        private IEnumerable<Region>? regionsForState { get; set; }

        void OnStateSelectChange(ChangeEventArgs e)
        {
            StateCode = e.Value!.ToString();
            GetRegionsForState();
        }

        private IEnumerable<Region>? GetRegionsForState()
           => regionsForState = StateCode == null
               ? Regions ?? []
               : Regions?.Where(r => r.StateCode?.Equals(StateCode, StringComparison.OrdinalIgnoreCase) ?? false) ?? [];

        public class OnRegionChangeCallbackArgs
        {
            public string? state { get; set; }
            public string? region { get; set; }
        }
        async void OnRegionSelectChange(ChangeEventArgs e)
        {
            var p = new OnRegionChangeCallbackArgs();
            if (e.Value?.ToString()?.Equals("all", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                p.state = StateCode;
                p.region = "all";
                await OnRegionChange.InvokeAsync(p);
            }
            if (int.TryParse(e.Value!.ToString(), out var regionId))
            {
                p.region = regionId.ToString();
                await OnRegionChange.InvokeAsync(p);
            }
            
        }
    }
}
