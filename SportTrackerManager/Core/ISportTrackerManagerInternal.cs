using System;

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
