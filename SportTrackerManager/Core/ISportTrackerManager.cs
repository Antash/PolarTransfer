using System;
using System.Collections.Generic;

namespace SportTrackerManager.Core
{
    public interface ISportTrackerManager
    {
        bool Login(string login, string password);
        string GetTrainingFileTcx(TrainingData data);
        void AddTrainingResult(TrainingData data);
        void RemoveTraining(TrainingData data);
        void AddTrainingTarget();
        void UpdateTrainingData(TrainingData data);
        IEnumerable<TrainingData> GetExercises(DateTime date);
    }
}
