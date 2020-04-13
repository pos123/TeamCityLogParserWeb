using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCityLogParserWeb.Services
{
    public interface IParserService
    {
        Task Run(string payload, Action<string> progress);
        Tuple<bool, string> GetFinalBuildStatement();
        TimeSpan? GetBuildTimeTaken();
        bool IsSolutionBuildCompleted();
        public IEnumerable<uint> GetFailedProjectList();
        List<Tuple<uint, string, string, string, string>> GetBuildErrorsOutput();
        List<Tuple<uint, string, string, string, string>> GetBuildErrorsOutputForProject(uint projectId);
        IEnumerable<Tuple<uint, string>> GetProjectData(uint projectId);
        string GetSortedProjectData();
    }
}
