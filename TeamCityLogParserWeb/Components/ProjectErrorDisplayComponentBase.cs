using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TeamCityLogParserWeb.Services;

namespace TeamCityLogParserWeb.Components
{
    public class ProjectErrorDisplayComponentBase : ComponentBase
    {
        [Inject] protected IParserService Parser { get; set; }

        [Parameter] public uint FailedProjectId { get; set; }

        public List<Tuple<uint, string, string, string, string>> FailedData { get; set; }

        public string ProjectName { get; set; }

        public string Configuration { get; set; }

        protected override void OnInitialized()
        {
            FailedData = Parser?.GetCodeBuildErrorsOutputForProject(FailedProjectId).ToList();
            ProjectName = FailedData?.FirstOrDefault()?.Item2;
            Configuration = FailedData?.FirstOrDefault()?.Item3;
        }
    }
}
