using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using TeamCityLogParserWeb.Services;


namespace TeamCityLogParserWeb.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] protected IApplicationState ApplicationState { get; set; }

        protected override void OnInitialized()
        {
            ApplicationState.OnChange += StateHasChanged;
        }

        public void Dispose()
        {
            ApplicationState.OnChange -= StateHasChanged;
        }
    }
}
