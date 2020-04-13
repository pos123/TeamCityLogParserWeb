using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCityLogParser;
using TeamCityLogParser.Extractors;
using TeamCityLogParserWeb.Pages;

namespace TeamCityLogParserWeb.Services
{
    public class ParserService : IParserService
    {
        private ParserProcessor parserProcessor;
        private DataService dataService;
        
        public async Task Run(string payload, Action<string> progress)
        {
            var sync = new object();

            var valueExtractor = new ValueExtractor(new DataDictionary());
            dataService = new DataService(payload);
            var parser = new Parser(dataService, valueExtractor);

            parserProcessor = new ParserProcessor(parser);
            await parserProcessor.Run((statusUpdate) =>
            {
                lock (sync)
                {
                    progress?.Invoke(statusUpdate);
                }
            });
        }

        public Tuple<bool, string> GetFinalBuildStatement()
        {
            if (!IsSolutionBuildCompleted())
            {
                return Tuple.Create(false, "Unable to identify errors as build log is not complete - requires a solution start and solution end");
            }

            var solutionBuild = parserProcessor.SolutionBuildSucceeded;
            if (solutionBuild != null)
            {
                return Tuple.Create(true, $"Build: {solutionBuild.Succeeded} succeeded, {solutionBuild.Failed} failed, {solutionBuild.Succeeded} up-to-date, {solutionBuild.Failed} skipped");
            }

            var solutionRebuild = parserProcessor.SolutionRebuildSucceeded;
            if (solutionRebuild != null)
            {
                return Tuple.Create(true, $"Rebuild All: {solutionRebuild.Succeeded} succeeded, {solutionRebuild.Failed} failed, {solutionRebuild.Failed} skipped");
            }

            var failedMessage = $"Solution build failed: {parserProcessor.ProjectBuildFailedEntries.Count} failed project(s), {parserProcessor.ProjectLineErrors.Count} error instance(s)";
            return Tuple.Create(false, failedMessage);
        }

        public TimeSpan? GetBuildTimeTaken()
        {
            if (!IsSolutionBuildCompleted())
            {
                return null;
            }

            if (parserProcessor.SolutionBuildSucceeded != null)
            {
                return parserProcessor.SolutionBuildSucceeded.Time - parserProcessor.SolutionStart.Time;
            }

            if (parserProcessor.SolutionRebuildSucceeded != null)
            {
                return parserProcessor.SolutionRebuildSucceeded.Time - parserProcessor.SolutionStart.Time;
            }

            return parserProcessor.SolutionFailedEntry.Time - parserProcessor.SolutionStart.Time;
        }

        public bool IsSolutionBuildCompleted() =>
            parserProcessor?.SolutionStart != null && (parserProcessor.SolutionFailedEntry != null ||
                                                       parserProcessor.SolutionBuildSucceeded != null ||
                                                       parserProcessor.SolutionRebuildSucceeded != null);

        public IEnumerable<uint> GetFailedProjectList()
        {
            return parserProcessor.ProjectBuildFailedEntries.Select(x => x.Id);
        }

        public List<Tuple<uint, string, string, string, string>> GetBuildErrorsOutputForProject(uint projectId)
        {
            if (parserProcessor == null)
            {
                return new List<Tuple<uint, string, string, string, string>>();
            }
            
            return (from entry in parserProcessor.ProjectLineErrors.Where( x => x.ProjectEntry.ProjectId == projectId)
                let lineNumber = entry.ProjectEntry.LineNumber
                let project = parserProcessor.ProjectDefinitions.FirstOrDefault(x => x.Id == entry.ProjectEntry.ProjectId)?.Name
                let configuration = parserProcessor.ProjectDefinitions.FirstOrDefault(x => x.Id == entry.ProjectEntry.ProjectId)?.Configuration
                let errorCategory = entry.Error
                let error = entry.ProjectEntry.Data
                select Tuple.Create(lineNumber, project, configuration, errorCategory, error)).ToList();
        }

        public List<Tuple<uint,string,string,string,string>> GetBuildErrorsOutput()
        {
            if (parserProcessor == null)
            {
                return new List<Tuple<uint, string, string, string, string>>();
            }

            return (from entry in parserProcessor.ProjectLineErrors.OrderBy(x => x.ProjectEntry.ProjectId).ThenBy(x => x.ProjectEntry.LineNumber)
                let lineNumber = entry.ProjectEntry.LineNumber
                let project = parserProcessor.ProjectDefinitions.FirstOrDefault(x => x.Id == entry.ProjectEntry.ProjectId)?.Name
                let configuration = parserProcessor.ProjectDefinitions.FirstOrDefault(x => x.Id == entry.ProjectEntry.ProjectId)?.Configuration
                let errorCategory = entry.Error
                let error = entry.ProjectEntry.Data
                select Tuple.Create(lineNumber, project, configuration, errorCategory, error)).ToList();
        }
        
        public IEnumerable<Tuple<uint, string>> GetProjectData(uint projectId)
        {
            if (parserProcessor == null)
            {
                return new List<Tuple<uint, string>>();
            }

            return parserProcessor.ProjectEntries.Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.LineNumber)
                .Select(x => Tuple.Create(x.LineNumber, x.Data));
        }

        public string GetSortedProjectData()
        {
            var builder = new StringBuilder(6000000);
            foreach (var projectDefinition in parserProcessor.ProjectDefinitions.OrderBy(x => x.Id))
            {
                builder.AppendLine("========================================================================");
                builder.AppendLine($" {projectDefinition.Name}, {projectDefinition.Configuration}");
                builder.AppendLine("========================================================================");
                foreach (var lineData in GetProjectData(projectDefinition.Id))
                {
                    builder.AppendLine(lineData.Item2);
                }
                builder.AppendLine(Environment.NewLine);
                builder.AppendLine(Environment.NewLine);
            }
            return builder.ToString();
        }

        private string GetData(uint lineNumber) => dataService?.Data(lineNumber);
    }
}
