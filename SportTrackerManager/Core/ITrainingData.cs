using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public interface ITrainingData
    {
        string Id { get; }
        string UserId { get; set; }
        string Description { get; set; }

        Exercise ActivityType { get; set; }
        DateTime Start { get; set; }

        double Distance { get; set; }
        TimeSpan Duration { get; set; }
        int AvgHr { get; set; }
        int MaxHr { get; set; }
        int AvgCadence { get; set; }
        int Calories { get; set; }
    }
}
