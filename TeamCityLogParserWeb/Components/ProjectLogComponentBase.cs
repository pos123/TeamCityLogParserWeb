using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TeamCityLogParserWeb.Services;

namespace TeamCityLogParserWeb.Components
{
    public class ProjectLogComponentBase :ComponentBase
    {
        [Inject] protected IParserService Parser { get; set; }

        [Inject] protected NavigationManager NavMan { get; set; }

        [Inject] protected IJSRuntime JsRuntime { get; set; }

        [Inject] protected IApplicationState ApplicationState { get; set; }

        protected string ProjectName { get; set; }

        protected string Configuration { get; set; }

        protected int ErrorCount { get; set; }

        protected List<uint> ErrorLineNumbers { get; set; }

        protected ProjectErrorMap ErrorMap { get; set; }

        protected override void OnInitialized()
        {
            var errors = Parser?.GetCodeBuildErrorsOutputForProject((uint)ApplicationState.ErrorsProjectId);
            ErrorLineNumbers = errors?.Select(x => x.Item1).OrderBy(x => x).ToList();
            ErrorCount = errors?.Count() ?? 0;
            ProjectName = errors?.FirstOrDefault()?.Item2;
            Configuration = errors?.FirstOrDefault()?.Item3;
            ErrorMap = new ProjectErrorMap(ErrorLineNumbers);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var link = $"error_link_{ApplicationState.ErrorsLineNumber}";
                await JsRuntime.InvokeVoidAsync("siteJsFunctions.scroll_to_view_error_link", link);
            }
        }

        public async Task previous_action(uint previousLineNumberError)
        {
            var link = $"error_link_{previousLineNumberError}";
            await JsRuntime.InvokeVoidAsync("siteJsFunctions.scroll_to_view_error_link", link);
        }

        public async Task next_action(uint nextLineNumberError)
        {
            var link = $"error_link_{nextLineNumberError}";
            await JsRuntime.InvokeVoidAsync("siteJsFunctions.scroll_to_view_error_link", link);
        }

        public void NavigateBack()
        {
            ApplicationState.ShowLogPage = false;
        }
    }
}
