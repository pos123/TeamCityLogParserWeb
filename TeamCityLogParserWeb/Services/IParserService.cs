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
        bool IsCodeBuildCompleted();
        bool IsFailedBuild();
        int GetErrorCount();
        Tuple<bool, string> GetBuildStatement();
        TimeSpan? GetBuildTimeTaken();
        bool IsSvnStageFailure();
        bool IsVerifyPackagesStageFailure();
        bool IsCodeBuildFailure();
        public IEnumerable<uint> GetCodeBuildFailedProjectList();
        IEnumerable<Tuple<uint, string, string, string, string>> GetCodeBuildErrorsOutputForProject(uint projectId);
        IEnumerable<Tuple<uint, string>> GetProjectData(uint projectId);
        string GetSortedProjectData();
        IEnumerable<Tuple<uint, string>> GetDefaultStageErrors();
    }
}
