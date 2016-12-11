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
        string LoginUrl { get; }
        string GetLoginPostData(string login, string password);
        string GetExportTcxUrl(TrainingData data);
        string AddTrainingUrl { get; }
        string GetAddTrainingPostData(TrainingData data);
        string GetDiaryUrl(DateTime date);
    }
}
