using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamCityLogParser;
using TeamCityLogParser.interfaces;
using static System.String;

namespace TeamCityLogParserWeb.Services
{
    public class ProjectLineError
    {
        public IProjectEntry ProjectEntry { get; set; }
        public string Error { get; set; }
    }
    
    
    public class ParserProcessor
    {
        private readonly Task[] taskList;
        public List<INoiseEntry> Noise;
        public ISolutionStartEntry SolutionStart;
        public ISolutionEndBuildFailedEntry SolutionFailedEntry;
        public ISolutionEndBuildSucceededEntry SolutionBuildSucceeded;
        public ISolutionEndRebuildSucceededEntry SolutionRebuildSucceeded;
        public List<IProjectDefinitionEntry> ProjectDefinitions;
        public List<IProjectEmptyEntry> ProjectEmptyEntries;
        public List<IProjectEndBuildFailedEntry> ProjectBuildFailedEntries;
        public List<IProjectEndBuildSucceededEntry> ProjectBuildSucceededEntries;
        public List<IProjectEntry> ProjectEntries;
        public List<IProjectEndEntry> ProjectEnd;
        public List<ProjectLineError> ProjectLineErrors;

        public ParserProcessor(Parser parser)
        {
            ProjectLineErrors = new List<ProjectLineError>();

            taskList = new Task[]
            {
                new Task(() => { Noise = parser.Noise; }),
                new Task(() => { SolutionStart = parser.SolutionStart; }),
                new Task(() => { SolutionFailedEntry = parser.SolutionFailedEntry; }),
                new Task(() => { SolutionBuildSucceeded = parser.SolutionBuildSucceeded; }),
                new Task(() => { SolutionRebuildSucceeded = parser.SolutionRebuildSucceeded; }),
                new Task(() => { ProjectDefinitions = parser.ProjectDefinitions; }),
                new Task(() => { ProjectEmptyEntries = parser.ProjectEmptyEntries; }),
                new Task(() => { ProjectBuildFailedEntries = parser.ProjectBuildFailedEntries; }),
                new Task(() => { ProjectBuildSucceededEntries = parser.ProjectBuildSucceededEntries; }),
                new Task(() => { ProjectEntries = parser.ProjectEntries; }),
                new Task(() => { ProjectEnd = parser.ProjectEnd; }),
            };
        }

        public Task Run(Action<string> statusUpdate)
        {
            return Task.Run(async () =>
            {
                // parse the log in to categories
                statusUpdate("parsing build log ...");
                await Task.Delay(500);

                foreach (var task in taskList)
                {
                    task.Start();
                }
                
                Task.WaitAll(taskList);

                statusUpdate("finished parsing build log ...");
                await Task.Delay(500);

                // identify errors into groups
                statusUpdate("starting error identification ...");
                await Task.Delay(500);

                foreach (var entry in ProjectBuildFailedEntries.SelectMany(failedProject => ProjectEntries.Where(x => x.ProjectId == failedProject.Id && !IsNullOrEmpty(x.ErrorType))))
                {
                    ProjectLineErrors.Add(new ProjectLineError()
                    {
                        ProjectEntry = entry,
                        Error = entry.ErrorType
                    });
                }

                statusUpdate("finished error identification ...");
                await Task.Delay(500);
            });
        }
    }
}
