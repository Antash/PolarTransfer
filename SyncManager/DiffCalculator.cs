using SportTrackerManager.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncManager
{
    public class DiffCalculator
    {
        private readonly Dictionary<string, IEnumerable<TrainingData>> trainingDictionary;

        public IReadOnlyDictionary<string, IEnumerable<TrainingData>> TrainingDataDictionary =>
            new ReadOnlyDictionary<string, IEnumerable<TrainingData>>(trainingDictionary);

        public DiffCalculator(Dictionary<string, IEnumerable<TrainingData>> trainingDictionary)
        {
            this.trainingDictionary = trainingDictionary;
        }
    }
}
