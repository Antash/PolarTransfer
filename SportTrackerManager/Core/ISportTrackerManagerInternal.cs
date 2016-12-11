using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    internal interface ISportTrackerManagerInternal
    {
        string ServiceUrl { get; }
        string LoginUrl { get; }
        string LoginPostDataTemplate { get; }
        string ExportTcxUrlTemplate { get; }
        string AddTrainingUrl { get; }
    }
}
