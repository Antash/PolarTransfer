using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SportTrackerManager.Core;

namespace SyncManager
{
    public class SyncManager : ISyncManager
    {
        private readonly List<ISportTrackerManager> sources = new List<ISportTrackerManager>();

        public void AddSource(ISportTrackerManager source)
        {
            sources.Add(source);
        }

        public IReadOnlyList<ISportTrackerManager> Sources =>
            sources.AsReadOnly();

        public async Task<DiffCalculator> Fetch(DateTime start, DateTime end)
        {
            var trainingDictionary = new Dictionary<string, IEnumerable<TrainingData>>();
            foreach (var source in sources)
            {
                trainingDictionary[source.Name] = await source.GetTrainingListAsync(start, end);
            }
            return new DiffCalculator(trainingDictionary);
        }

        public void Sync()
        {

        }
    }
}
