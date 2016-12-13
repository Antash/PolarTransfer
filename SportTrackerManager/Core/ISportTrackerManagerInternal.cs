using System;

namespace SportTrackerManager.Core
{
    internal interface ISportTrackerManagerInternal
    {
        string LoginUrl { get; }
        string GetLoginPostData(string login, string password);
        string GetExportTcxUrl(string trainingId);
        string AddTrainingUrl { get; }
        string GetAddTrainingPostData(TrainingData data);
        string GetDiaryUrl(DateTime date);
        string GetDiaryUrl(DateTime start, DateTime end);
        string GetTrainingUrl(string trainingId);
    }
}
