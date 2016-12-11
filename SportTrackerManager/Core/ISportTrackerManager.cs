using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public interface ISportTrackerManager
    {
        bool Login(string login, string password);
        string GetTrainingFileTcx(TrainingData data);
        bool AddTrainingResult(TrainingData data);
        bool AddTrainingTarget();
        bool UpdateTrainingData(TrainingData data);
        IEnumerable<TrainingData> GetExercises(DateTime start, DateTime end);
    }
}
