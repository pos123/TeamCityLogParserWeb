using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;
using TeamCityLogParserWeb.Data;
using Microsoft.AspNetCore.Components;
using TeamCityLogParser;
using TeamCityLogParserWeb.Services;
using static System.String;


namespace TeamCityLogParserWeb.Components
{
    public enum ProcessingStatus
    {
        None,
        FileUploading,
        FileUploaded,
        FileUploadError,
        Parsing,
        ParsingFailure,
        ParsingFinished,
        CompilingReport,
        Done,
    }

    public class InputComponentBase : ComponentBase, IDisposable
    {
        [Inject] private IParserService ParserService { get; set;}

        [Inject] protected IApplicationState ApplicationState { get; set; }

        
        private const int MaxFileSize = 10 * 1024 * 1024;
        private const int Delay = 500;
        private readonly List<string> validExtensionList = new List<string>()
        {
            ".log"
        };


        public bool IsProcessing => ApplicationState.ProcessingState == ProcessingStatus.FileUploading ||
                                    ApplicationState.ProcessingState == ProcessingStatus.FileUploaded ||
                                    ApplicationState.ProcessingState == ProcessingStatus.Parsing ||
                                    ApplicationState.ProcessingState == ProcessingStatus.CompilingReport;


        protected override void OnInitialized()
        {
            ApplicationState.OnChange += StateHasChanged;
        }

        public void Dispose()
        {
            ApplicationState.OnChange -= StateHasChanged;
        }

        public async Task UploadFile(IFileListEntry[] files)
        {
            var file = files.FirstOrDefault();
            var (state, stateDisplay, validFile, filename) = CheckFile(file);

            ApplicationState.ProcessingState = state;
            ApplicationState.Filename = filename;
            ApplicationState.InputStateDisplay = stateDisplay;
            ApplicationState.ShowReport = false;
            ApplicationState.BadgeErrorCount = 0;
            ApplicationState.SelectedTab = 0;


            if (validFile)
            {
                var (processingStateProcess, processedPayload) = await ProcessFile(file, (processState, processDisplay) =>
                {
                    ApplicationState.ProcessingState = processState;
                    ApplicationState.InputStateDisplay = processDisplay;
                });

                if (processingStateProcess == ProcessingStatus.ParsingFinished)
                {
                    ApplicationState.ProcessingState = ProcessingStatus.CompilingReport;
                    ApplicationState.InputStateDisplay = "Compiling report ...";

                    ApplicationState.BadgeErrorCount = ParserService.GetErrorCount();
                    ApplicationState.SelectedTab = 0;
                    ApplicationState.ShowReport = true;
                }
            }
        }

        private Tuple<ProcessingStatus, string, bool, string> CheckFile(IFileListEntry file)
        {
            if (file == null)
            {
                return Tuple.Create(ProcessingStatus.FileUploadError, "no file selected", false, string.Empty);
            }

            if (!validExtensionList.Contains(Path.GetExtension(file.Name).ToLower()))
            {
                return Tuple.Create(ProcessingStatus.FileUploadError, $"the file extension has to be .log not {Path.GetExtension(file.Name)} ", false, file.Name);
            }

            if (file.Size > MaxFileSize)
            {
                return Tuple.Create(ProcessingStatus.FileUploadError, $"the maximum build log size: {MaxFileSize} bytes. Input file size is {file.Size} bytes", false, file.Name);
            }

            return Tuple.Create(ProcessingStatus.None, string.Empty, true, file.Name);
        }

        private async Task<Tuple<ProcessingStatus, string>> ProcessFile(IFileListEntry file, Action<ProcessingStatus, string> updateAction)
        {
            updateAction(ProcessingStatus.FileUploading, $"uploading {ApplicationState.Filename} ...");
            
            await Task.Delay(Delay);
            using var reader = new StreamReader(file.Data);
            var payload = await reader.ReadToEndAsync();
            await Task.Delay(Delay);

            updateAction(ProcessingStatus.FileUploaded, "uploaded file");
            await Task.Delay(Delay);
            updateAction(ProcessingStatus.Parsing, string.Empty);

            try
            {
                await ParserService.Run(payload, async(progress) =>
                {
                    await InvokeAsync(() =>
                    {
                        updateAction(ProcessingStatus.Parsing, $"{progress}");
                        StateHasChanged();
                    });
                });
            }
            catch (Exception e)
            {
                updateAction(ProcessingStatus.ParsingFailure, $"failed to parse the log file: {e.Message}");
                return Tuple.Create(ProcessingStatus.ParsingFailure, string.Empty);
            }
            
            return Tuple.Create(ProcessingStatus.ParsingFinished, payload);
        }
    }
}
