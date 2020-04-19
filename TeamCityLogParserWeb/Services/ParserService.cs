using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCityLogParser;
using TeamCityLogParser.Extractors;
using TeamCityLogParser.Parsers;
using TeamCityLogParserWeb.Pages;

namespace TeamCityLogParserWeb.Services
{
    public class ParserService : IParserService
    {
        private BuildLogParser buildLogParser;
        
        public async Task Run(string payload, Action<string> progress)
        {
            var sync = new object();
            buildLogParser = new BuildLogParser(payload);
            await buildLogParser.Parse((update) =>
            {
                lock (sync)
                {
                    progress?.Invoke(update);
                }
            });
        }

        public bool IsFailedBuild()
        {
            return buildLogParser != null && buildLogParser.HasFailedStages();
        }

        public bool IsCodeBuildCompleted()
        {
            return buildLogParser != null && buildLogParser.IsCodeBuildCompleted();
        }

        public int GetErrorCount()
        {
            return buildLogParser?.GetLastStageErrorCount() ?? 0;
        }

        public Tuple<bool, string> GetBuildStatement()
        {
            return buildLogParser != null ? buildLogParser.GetStatement() : Tuple.Create(false, string.Empty);
        }

        public TimeSpan? GetBuildTimeTaken()
        {
            return buildLogParser?.GetLastStageTimeTaken();
        }

        public bool IsSvnStageFailure()
        {
            return buildLogParser != null && buildLogParser.GetLastFailedStageGroupType() == StageGroupType.SvnUpdate;
        }

        public bool IsVerifyPackagesStageFailure()
        {
            return buildLogParser != null && buildLogParser.GetLastFailedStageGroupType() == StageGroupType.VerifyPackages;
        }

        public bool IsCodeBuildFailure()
        {
            return buildLogParser != null && buildLogParser.GetLastFailedStageGroupType() == StageGroupType.CodeBuild;

        }

        public IEnumerable<uint> GetCodeBuildFailedProjectList()
        {
            var codeResults = buildLogParser?.CodeResults;
            return codeResults != null ? codeResults.GetFailedProjectList() : Enumerable.Empty<uint>();
        }

        public IEnumerable<Tuple<uint, string, string, string, string>> GetCodeBuildErrorsOutputForProject(uint projectId)
        {
            if (buildLogParser == null)
            {
                return Enumerable.Empty<Tuple<uint, string, string, string, string>>();
            }

            var codeBuildResults = buildLogParser.CodeResults;

            return (from entry in codeBuildResults.GetProjectLineErrors().Where( x => x.ProjectEntry.ProjectId == projectId)
                let lineNumber = entry.ProjectEntry.LineNumber
                let project = codeBuildResults.GetProjectDefinitions().FirstOrDefault(x => x.Id == entry.ProjectEntry.ProjectId)?.Name
                let configuration = codeBuildResults.GetProjectDefinitions().FirstOrDefault(x => x.Id == entry.ProjectEntry.ProjectId)?.Configuration
                let errorCategory = entry.Error
                let error = entry.ProjectEntry.Data
                select Tuple.Create(lineNumber, project, configuration, errorCategory, error)).ToList();
        }

        
        public IEnumerable<Tuple<uint, string>> GetProjectData(uint projectId)
        {
            if (buildLogParser == null)
            {
                return new List<Tuple<uint, string>>();
            }
            
            var codeBuildResults = buildLogParser.CodeResults;

            return codeBuildResults.GetProjectEntries().Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.LineNumber)
                .Select(x => Tuple.Create(x.LineNumber, x.Data));
        }

        public string GetSortedProjectData()
        {
            if (buildLogParser == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(6000000);
            var codeBuildResults = buildLogParser.CodeResults;

            foreach (var projectDefinition in codeBuildResults.GetProjectDefinitions().OrderBy(x => x.Id))
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


        public IEnumerable<Tuple<uint, string>> GetDefaultStageErrors()
        {
            if (buildLogParser == null || !(IsSvnStageFailure() || IsVerifyPackagesStageFailure()))
            {
                return Enumerable.Empty<Tuple<uint, string>>();
            }
            var defaultParserResults =
                IsSvnStageFailure() ? buildLogParser.SvnUpdateResults : buildLogParser.VerifyPackageResults;
            
            return defaultParserResults != null ? defaultParserResults.GetErrors() : Enumerable.Empty<Tuple<uint, string>>();
        }
    }
}
