using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using TeamCityLogParserWeb.Services;

namespace TeamCityLogParserWeb.Components
{
    public class ProjectErrorDisplayComponentBase : ComponentBase
    {
        [Inject] protected IParserService Parser { get; set; }

        [Inject] protected IApplicationState ApplicationState { get; set; }

        [Parameter] public uint FailedProjectId { get; set; }

        public List<Tuple<uint, string, string, string, string>> FailedData { get; set; }

        public string ProjectName { get; set; }

        public string Configuration { get; set; }

        protected Dictionary<uint, int> LineNumberToErrorNumber { get; set; } = new Dictionary<uint, int>();

        protected override void OnInitialized()
        {
            FailedData = Parser?.GetCodeBuildErrorsOutputForProject(FailedProjectId).ToList();
            ProjectName = FailedData?.FirstOrDefault()?.Item2;
            Configuration = FailedData?.FirstOrDefault()?.Item3;

            var i = 0;
            foreach (var data in FailedData)
            {
                LineNumberToErrorNumber[data.Item1] = ++i;
            }
        }

        public void NavigateToProjectErrors(uint lineNumber)
        {
            ApplicationState.ErrorsProjectId = (int) FailedProjectId;
            ApplicationState.ErrorsLineNumber = (int)lineNumber;
            ApplicationState.ShowLogPage = true;
        }
    }
}
