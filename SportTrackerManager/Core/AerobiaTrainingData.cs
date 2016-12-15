using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class AerobiaTrainingData : TrainingData
    {
        public AerobiaTrainingData()
        {
        }

        public AerobiaTrainingData(string id) : base(id)
        {
        }

        public string NoteId { get; set; }
        public string Title { get; set; }
    }
}
