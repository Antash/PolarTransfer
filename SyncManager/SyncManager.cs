using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportTrackerManager.Core;

namespace SyncManager
{
    public class SyncManager : ISyncManager
    {
        private readonly IList<ISportTrackerManager> sources;

        public SyncManager(IEnumerable<ISportTrackerManager> sources)
        {
            this.sources = sources.ToList();
        }

        public void AddSource(ISportTrackerManager source)
        {
            sources.Add(source);
        }
        /*
        public DiffCalculator Fetch(DateTime start, DateTime end)
        {
            var trainingDictionary = new Dictionary<string, IEnumerable<TrainingData>>();
            foreach (var source in sources)
            {
                trainingDictionary[source.Name] = source.GetTrainingListAsync(start, end);
            }
            return new DiffCalculator(trainingDictionary);
        }*/

        public void Sync()
        {

        }
    }
}
