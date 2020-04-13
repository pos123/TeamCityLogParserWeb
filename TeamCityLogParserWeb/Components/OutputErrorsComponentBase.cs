using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TeamCityLogParserWeb.Services;

namespace TeamCityLogParserWeb.Components
{
    public class OutputErrorsComponentBase : ComponentBase
    {
        [Inject] protected IParserService Parser { get; set; }
    }
}
