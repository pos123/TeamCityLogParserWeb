﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamCityLogParserWeb.Components;


namespace TeamCityLogParserWeb.Services
{
    public interface IApplicationState
    {
        event Action OnChange;
        string Filename { get; set; }
        string InputFileStateDisplay { get; set; }
        ProcessingStatus ProcessingState { get; set; }
        bool ShowReport { get; set; }
        int BadgeErrorCount { get; set; }
        int SelectedTab { get; set; }
    }
}
