using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class TrainingData
    {
        public TrainingData(string id)
        {
            Id = id;
        }

        public string Description { get; set; }
        public string Id { get; private set; }
    }
}
