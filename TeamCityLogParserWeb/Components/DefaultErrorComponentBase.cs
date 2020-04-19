using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TeamCityLogParserWeb.Services;

namespace TeamCityLogParserWeb.Components
{
    public class DefaultErrorComponentBase : LayoutComponentBase
    {
        [Inject] protected IParserService ParserService { get; set; }

        public string ErrorDescription { get; set; }

        protected override void OnInitialized()
        {
            ErrorDescription = ParserService.IsSvnStageFailure()
                ? "SVN"
                : "Nuget Package Verification";
        }
    }
}
