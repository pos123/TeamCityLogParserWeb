using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamCityLogParserWeb.Services
{
    internal struct ErrorMap
    {
        public int ErrorNumber { get; set; }
        public uint Next { get; set; }
        public uint Previous { get; set; }
    }
    
    
    public class ProjectErrorMap
    {
        private readonly List<uint> errors;
        private readonly Dictionary<uint, ErrorMap> errorMap = new Dictionary<uint, ErrorMap>();

        public ProjectErrorMap(IEnumerable<uint> errors)
        {
            this.errors = errors.OrderBy(x => x).ToList();
            BuildErrorsMap();
        }

        public Tuple<bool, uint, uint, int> IsErrorLine(uint lineNumber)
        {
            return !errorMap.ContainsKey(lineNumber) 
                ? Tuple.Create(false, (uint)0, (uint)0, 0) 
                : Tuple.Create(true, errorMap[lineNumber].Previous, errorMap[lineNumber].Next, errorMap[lineNumber].ErrorNumber);
        }

        private void BuildErrorsMap()
        {
            for (var i = 0; i < errors.Count; i++)
            {
                var previous = i == 0 ? 0 : errors[i - 1];
                var next = i < errors.Count - 1 ? errors[i + 1] : 0;
                errorMap.Add(errors[i], new ErrorMap()
                {
                    ErrorNumber = i+1,
                    Next = (uint)next,
                    Previous = previous
                });
            }
        }

    }
}
