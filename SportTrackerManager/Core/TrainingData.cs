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
        Walking,
        OtherSport,
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

        public Excercise ActivityType { get; set; }
        public DateTime Start { get; set; }

        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public int AvgHr { get; set; }
        public int MaxHr { get; set; }
        public int AvgCadence { get; set; }
        public int Calories { get; set; }
    }
}
