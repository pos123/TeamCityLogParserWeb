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
        private bool showLogPage = false;
        private int badgeErrorCount;
        private int selectedTab = 0;
        private string inputDisplay = string.Empty;

        public event Action OnChange;
        public string Filename { get; set; }
        
        public string InputStateDisplay
        {
            get => inputDisplay;
            set
            {
                inputDisplay = value;
                NotifyStateChanged();
            }
        }

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

        public bool ShowLogPage
        {
            get => showLogPage;
            set
            {
                showLogPage = value;
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

        public int ErrorsProjectId { get; set; }
        public int ErrorsLineNumber { get; set; }


        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}