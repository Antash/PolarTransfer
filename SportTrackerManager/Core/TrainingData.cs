using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public enum Excercise
    {
        Running,
        Swimming,
        IndoorCycling,
        Cycling,
        Gym,
        OPA,
        OWS,
        Triathlon,
        ClassicSkiing,
        ScateSkiing,
        OtherSport
    }

    public class TrainingData
    {
        public TrainingData(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public Excercise ActivityType { get; set; }
        public DateTime Start { get; set; }
    }
}
