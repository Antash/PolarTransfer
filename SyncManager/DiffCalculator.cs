using SportTrackerManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncManager
{
    public class DiffCalculator
    {
        private Dictionary<string, IEnumerable<TrainingData>> trainingDictionary;

        public DiffCalculator(Dictionary<string, IEnumerable<TrainingData>> trainingDictionary)
        {
            this.trainingDictionary = trainingDictionary;
        }
    }
}
