using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TeamCityLogParserWeb.Services;

namespace TeamCityLogParserWeb.Components
{
    public class OutputSummaryComponentBase : ComponentBase
    {
        [Inject] protected IParserService Parser { get; set; }

        public string SummaryMessage { get; set; } = string.Empty;

        public string BuildTime;

        public bool BuildCompleted { get; set; } = false;

        public bool BuildSucceeded { get; set; } = false;

        protected override void OnInitialized()
        {
            (BuildSucceeded, SummaryMessage) = Parser.GetFinalBuildStatement();
            BuildTime = Parser.GetBuildTimeTaken().HasValue ? $"{Parser.GetBuildTimeTaken()?.Hours}hrs {Parser.GetBuildTimeTaken()?.Minutes}min {Parser.GetBuildTimeTaken()?.Seconds}sec" : String.Empty;
            BuildCompleted = Parser.IsSolutionBuildCompleted();
        }
    }
}
