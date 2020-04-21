using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TeamCityLogParserWeb.Services;
using static System.String;

namespace TeamCityLogParserWeb.Components
{
    public class OutputComponentBase : ComponentBase, IDisposable
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

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                AppState.ProcessingState = ProcessingStatus.Done;
                AppState.InputStateDisplay = "successfully parsed log file see build report below";
            }

            base.OnAfterRender(firstRender);
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
