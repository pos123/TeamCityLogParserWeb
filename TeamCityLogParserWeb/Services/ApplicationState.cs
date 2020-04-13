using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamCityLogParserWeb.Components;
using static System.String;

namespace TeamCityLogParserWeb.Services
{
    public class ApplicationState : IApplicationState
    {
        private ProcessingStatus internalProcessingState = ProcessingStatus.None;
        private bool showReport = false;
        private int badgeErrorCount;
        private int selectedTab = 0;

        public event Action OnChange;
        public string Filename { get; set; }
        public string InputFileStateDisplay { get; set; } = Empty;

        public ProcessingStatus ProcessingState
        {
            get => internalProcessingState;

            set
            {
                internalProcessingState = value;
                NotifyStateChanged();
            }
        }

        public bool ShowReport
        {
            get => showReport;
            set 
            { 
                showReport = value;
                NotifyStateChanged();
            }
        }

        public int BadgeErrorCount
        {
            get => badgeErrorCount;
            set
            {
                badgeErrorCount = value;
                NotifyStateChanged();
            }
        }

        public int SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}