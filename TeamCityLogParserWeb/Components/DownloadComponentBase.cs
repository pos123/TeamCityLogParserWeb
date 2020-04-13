using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TeamCityLogParserWeb.Services;

namespace TeamCityLogParserWeb.Components
{
    public class DownloadComponentBase : ComponentBase
    {
        [Inject] protected IParserService Parser { get; set; }

        [Inject] protected IJSRuntime JsRuntime { get; set; }

        public bool Downloading { get; set; } = false;

        public async void Download()
        {
            Downloading = true;
            StateHasChanged();
            var now = DateTime.Now.ToString("yyyy_MM_ddd_HH_mm_ss");
            var filename = $"{now}_sorted_log.txt";
            await Task.Delay(1000);
            await JsRuntime.SaveAs(filename, Encoding.ASCII.GetBytes(Parser.GetSortedProjectData()));
            Downloading = false;
            StateHasChanged();
        }
    }
}
