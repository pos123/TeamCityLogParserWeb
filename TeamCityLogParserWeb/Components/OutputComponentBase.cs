using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TeamCityLogParserWeb.Services;
using static System.String;

namespace TeamCityLogParserWeb.Components
{
    public class OutputComponentBase : ComponentBase
    {
        [Inject] protected IApplicationState AppState { get; set; }

        public string SummaryActive { get; set; } = "active";

        public string BuildErrorsActive { get; set; } = Empty;

        public string DownloadsActive { get; set; } = Empty;

        protected override void OnInitialized()
        {
            AppState.OnChange += StateHasChanged;

            SummaryActive = AppState.SelectedTab == 0 ? "active" : Empty;
            BuildErrorsActive = AppState.SelectedTab == 1 ? "active" : Empty;
            DownloadsActive = AppState.SelectedTab == 2 ? "active" : Empty;
        }

        public void Dispose()
        {
            AppState.OnChange -= StateHasChanged;
        }

        public void SelectedTab(int tabId)
        {
            AppState.SelectedTab = tabId;
        }
    }
}
