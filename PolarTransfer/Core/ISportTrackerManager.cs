using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PolarTransfer.Core
{
    interface ISportTrackerManager
    {
        CookieCollection Login();
        string GetTrainingFileTcx();
        bool AddTrainingResult();
        bool AddTrainingTarget();
        bool UpdateTrainingData();
        TrainingData GetTrainingData();
    }
}
